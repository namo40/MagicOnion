using System.Buffers;
using System.Collections.Concurrent;
using System.Diagnostics;
using MagicOnion.Internal;
using MagicOnion.Server.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MagicOnion.Server.Hubs;

internal interface IStreamingHubHeartbeatManager : IDisposable
{
    TimeProvider TimeProvider { get; }

    StreamingHubHeartbeatHandle Register(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext);
    void Unregister(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext);
}

internal class StreamingHubHeartbeatHandle : IDisposable
{
    readonly IStreamingHubHeartbeatManager manager;
    readonly CancellationTokenSource timeoutToken;
    readonly TimeSpan timeoutDuration;
    bool disposed;
    short waitingSequence;
    bool timeoutTimerIsRunning;
    DateTimeOffset lastSentAt;
    DateTimeOffset lastReceivedAt;

    /// <summary>
    /// Gets the last received time.
    /// </summary>
    public DateTimeOffset LastReceivedAt => lastReceivedAt;

    /// <summary>
    /// Gets the latency between client and server. Returns <see cref="TimeSpan.Zero"/> if not sent or received.
    /// </summary>
    public TimeSpan Latency => (lastSentAt == default || lastReceivedAt == default)
        ? TimeSpan.Zero
        : lastReceivedAt - lastSentAt;

    public IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> ServiceContext { get; }
    public CancellationToken TimeoutToken => timeoutToken.Token;

    public StreamingHubHeartbeatHandle(IStreamingHubHeartbeatManager manager, IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext, TimeSpan timeoutDuration)
    {
        this.manager = manager;
        this.ServiceContext = serviceContext;
        this.timeoutDuration = timeoutDuration;
        this.timeoutToken = new CancellationTokenSource(Timeout.InfiniteTimeSpan
#if NET8_0_OR_GREATER
            , this.manager.TimeProvider
#endif
        );
    }

    public void RestartTimeoutTimer(short sequence, DateTimeOffset sentAt)
    {
        if (disposed || timeoutDuration == Timeout.InfiniteTimeSpan) return;
        waitingSequence = sequence;
        lastSentAt = sentAt;

        if (!timeoutTimerIsRunning)
        {
            timeoutToken.CancelAfter(timeoutDuration);
            timeoutTimerIsRunning = true;
        }
    }

    public void Ack(short sequence)
    {
        if (disposed || timeoutDuration == Timeout.InfiniteTimeSpan) return;

        if (waitingSequence != sequence) return;
        lastReceivedAt = manager.TimeProvider.GetUtcNow();
        timeoutToken.CancelAfter(Timeout.InfiniteTimeSpan);
        timeoutTimerIsRunning = false;
    }

    public void Dispose()
    {
        if (disposed) return;
        disposed = true;
        manager.Unregister(ServiceContext);
        timeoutToken.Dispose();
    }
}

internal class NopStreamingHubHeartbeatManager : IStreamingHubHeartbeatManager
{
    public static IStreamingHubHeartbeatManager Instance { get; } = new NopStreamingHubHeartbeatManager();

    public TimeProvider TimeProvider => TimeProvider.System;

    NopStreamingHubHeartbeatManager() {}

    public StreamingHubHeartbeatHandle Register(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext)
        => new(this, serviceContext, Timeout.InfiniteTimeSpan);
    public void Unregister(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext) { }
    public void Dispose() { }
}

internal class StreamingHubHeartbeatManager : IStreamingHubHeartbeatManager
{
    static ReadOnlySpan<byte> Nil => [0xc0];

    readonly object timerGate = new();
    readonly IStreamingHubHeartbeatMetadataProvider? heartbeatMetadataProvider;
    readonly TimeSpan heartbeatInterval;
    readonly TimeSpan timeoutDuration;
    readonly ILogger logger;

    PeriodicTimer? timer;
    int registeredCount;
    ConcurrentDictionary<Guid, StreamingHubHeartbeatHandle> contexts = new();
    short sequence;

    public TimeProvider TimeProvider { get; }

    public StreamingHubHeartbeatManager(TimeSpan heartbeatInterval, TimeSpan timeoutDuration, IStreamingHubHeartbeatMetadataProvider? heartbeatMetadataProvider, TimeProvider timeProvider, ILogger<StreamingHubHeartbeatManager> logger)
    {
        this.heartbeatInterval = heartbeatInterval;
        this.timeoutDuration = timeoutDuration;
        this.heartbeatMetadataProvider = heartbeatMetadataProvider;
        this.logger = logger;

        TimeProvider = timeProvider;
    }

    public StreamingHubHeartbeatHandle Register(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext)
    {
        var method = serviceContext.CallContext.Method;
        var handle = new StreamingHubHeartbeatHandle(this, serviceContext, timeoutDuration);
        if (contexts.TryAdd(serviceContext.ContextId, handle))
        {
            if (Interlocked.Increment(ref registeredCount) == 1)
            {
                lock (timerGate)
                {
                    if (timer is null)
                    {
                        timer = new PeriodicTimer(heartbeatInterval
#if NET8_0_OR_GREATER
                            , TimeProvider
#endif
                        );
                        MagicOnionServerLog.BeginHeartbeatTimer(this.logger, method, heartbeatInterval, timeoutDuration);
                        _ = StartHeartbeatAsync(timer, method);
                    }
                }
            }

            handle.TimeoutToken.UnsafeRegister(_ => MagicOnionServerLog.HeartbeatTimedOut(logger, method, serviceContext.ContextId), null);

            return handle;
        }

        return contexts[serviceContext.ContextId];
    }

    async Task StartHeartbeatAsync(PeriodicTimer runningTimer, string method)
    {
        Debug.Assert(runningTimer != null);

        var writer = new ArrayBufferWriter<byte>();
        while (await runningTimer.WaitForNextTickAsync())
        {
            var now = TimeProvider.GetUtcNow();
            StreamingHubMessageWriter.WriteServerHeartbeatMessageHeader(writer, sequence, now);
            if (!(heartbeatMetadataProvider?.TryWriteMetadata(writer) ?? false))
            {
                writer.Write(Nil);
            }

            MagicOnionServerLog.SendHeartbeat(this.logger, method);
            try
            {
                foreach (var (contextId, handle) in contexts)
                {
                    handle.RestartTimeoutTimer(sequence, now);
                    handle.ServiceContext.QueueResponseStreamWrite(StreamingHubPayloadPool.Shared.RentOrCreate(writer.WrittenSpan));
                }
            }
            catch { /* Ignore */ }

            writer.Clear();
            sequence++;
        }
    }

    public void Unregister(IStreamingServiceContext<StreamingHubPayload, StreamingHubPayload> serviceContext)
    {
        if (contexts.TryRemove(serviceContext.ContextId, out _))
        {
            if (Interlocked.Decrement(ref registeredCount) == 0)
            {
                lock (timerGate)
                {
                    if (Volatile.Read(ref registeredCount) == 0 && timer is not null)
                    {
                        MagicOnionServerLog.ShutdownHeartbeatTimer(this.logger, serviceContext.CallContext.Method);
                        timer.Dispose();
                        timer = null;
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}

public interface IStreamingHubHeartbeatMetadataProvider
{
    bool TryWriteMetadata(IBufferWriter<byte> writer);
}
