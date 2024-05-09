﻿// <auto-generated />
#pragma warning disable CS0618 // 'member' is obsolete: 'text'
#pragma warning disable CS0612 // 'member' is obsolete
#pragma warning disable CS0414 // The private field 'field' is assigned but its value is never used
#pragma warning disable CS8019 // Unnecessary using directive.
#pragma warning disable CS1522 // Empty switch block

namespace TempProject
{
    partial class MagicOnionInitializer
    {
        static partial class MagicOnionGeneratedClient
        {
            [global::MagicOnion.Ignore]
            public class TempProject_MyHubClient : global::MagicOnion.Client.StreamingHubClientBase<global::TempProject.IMyHub, global::TempProject.IMyHubReceiver>, global::TempProject.IMyHub
            {
                readonly global::MagicOnion.Client.IStreamingHubDiagnosticHandler diagnosticHandler;

                public TempProject_MyHubClient(global::Grpc.Core.CallInvoker callInvoker, global::System.String host, global::Grpc.Core.CallOptions options, global::MagicOnion.Serialization.IMagicOnionSerializerProvider serializerProvider, global::MagicOnion.Client.IMagicOnionClientLogger logger, global::MagicOnion.Client.IStreamingHubDiagnosticHandler diagnosticHandler)
                    : base("IMyHub", callInvoker, host, options, serializerProvider, logger)
                {
                    this.diagnosticHandler = diagnosticHandler;
                }

                global::System.Threading.Tasks.Task<TResponse> WriteMessageWithResponseDiagnosticAsync<TRequest, TResponse>(int methodId, TRequest message, [global::System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = default!)
                {
                    if (diagnosticHandler is null)
                    {
                        return base.WriteMessageWithResponseAsync<TRequest, TResponse>(methodId, message);
                    }

                    return diagnosticHandler.OnMethodInvoke(this, methodId, callerMemberName, message, isFireAndForget: true, base.WriteMessageWithResponseAsync<TRequest, TResponse>);
                }

                global::System.Threading.Tasks.Task<TResponse> WriteMessageFireAndForgetDiagnosticAsync<TRequest, TResponse>(int methodId, TRequest message, [global::System.Runtime.CompilerServices.CallerMemberName] string callerMemberName = default!)
                {
                    if (diagnosticHandler is null)
                    {
                        return base.WriteMessageFireAndForgetAsync<TRequest, TResponse>(methodId, message);
                    }

                    return diagnosticHandler.OnMethodInvoke(this, methodId, callerMemberName, message, isFireAndForget: true, base.WriteMessageFireAndForgetAsync<TRequest, TResponse>);
                }

                public global::System.Threading.Tasks.Task MethodA()
                    => this.WriteMessageWithResponseDiagnosticAsync<global::MessagePack.Nil, global::MessagePack.Nil>(1497325507, global::MessagePack.Nil.Default);
                public global::System.Threading.Tasks.Task<global::System.Int32> MethodB()
                    => this.WriteMessageWithResponseDiagnosticAsync<global::MessagePack.Nil, global::System.Int32>(1514103126, global::MessagePack.Nil.Default);
                public global::System.Threading.Tasks.ValueTask MethodC()
                    => new global::System.Threading.Tasks.ValueTask(this.WriteMessageWithResponseDiagnosticAsync<global::MessagePack.Nil, global::MessagePack.Nil>(1530880745, global::MessagePack.Nil.Default));
                public global::System.Threading.Tasks.ValueTask<global::System.Int32> MethodD()
                    => new global::System.Threading.Tasks.ValueTask<global::System.Int32>(this.WriteMessageWithResponseDiagnosticAsync<global::MessagePack.Nil, global::System.Int32>(1547658364, global::MessagePack.Nil.Default));
                public global::System.Threading.Tasks.Task MethodE(global::System.Int32 arg0, global::System.String arg1)
                    => this.WriteMessageWithResponseDiagnosticAsync<global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String>, global::MessagePack.Nil>(1564435983, new global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String>(arg0, arg1));
                public global::System.Threading.Tasks.Task<global::System.String> MethodF(global::System.Int32 arg0, global::System.String arg1, global::System.Boolean arg2)
                    => this.WriteMessageWithResponseDiagnosticAsync<global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String, global::System.Boolean>, global::System.String>(1581213602, new global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String, global::System.Boolean>(arg0, arg1, arg2));

                public global::TempProject.IMyHub FireAndForget()
                    => new FireAndForgetClient(this);
                    
                [global::MagicOnion.Ignore]
                class FireAndForgetClient : global::TempProject.IMyHub
                {
                    readonly TempProject_MyHubClient parent;

                    public FireAndForgetClient(TempProject_MyHubClient parent)
                        => this.parent = parent;

                    public global::TempProject.IMyHub FireAndForget() => this;
                    public global::System.Threading.Tasks.Task DisposeAsync() => throw new global::System.NotSupportedException();
                    public global::System.Threading.Tasks.Task WaitForDisconnect() => throw new global::System.NotSupportedException();

                    public global::System.Threading.Tasks.Task MethodA()
                        => parent.WriteMessageFireAndForgetDiagnosticAsync<global::MessagePack.Nil, global::MessagePack.Nil>(1497325507, global::MessagePack.Nil.Default);
                    public global::System.Threading.Tasks.Task<global::System.Int32> MethodB()
                        => parent.WriteMessageFireAndForgetDiagnosticAsync<global::MessagePack.Nil, global::System.Int32>(1514103126, global::MessagePack.Nil.Default);
                    public global::System.Threading.Tasks.ValueTask MethodC()
                        => new global::System.Threading.Tasks.ValueTask(parent.WriteMessageFireAndForgetDiagnosticAsync<global::MessagePack.Nil, global::MessagePack.Nil>(1530880745, global::MessagePack.Nil.Default));
                    public global::System.Threading.Tasks.ValueTask<global::System.Int32> MethodD()
                        => new global::System.Threading.Tasks.ValueTask<global::System.Int32>(parent.WriteMessageFireAndForgetDiagnosticAsync<global::MessagePack.Nil, global::System.Int32>(1547658364, global::MessagePack.Nil.Default));
                    public global::System.Threading.Tasks.Task MethodE(global::System.Int32 arg0, global::System.String arg1)
                        => parent.WriteMessageFireAndForgetDiagnosticAsync<global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String>, global::MessagePack.Nil>(1564435983, new global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String>(arg0, arg1));
                    public global::System.Threading.Tasks.Task<global::System.String> MethodF(global::System.Int32 arg0, global::System.String arg1, global::System.Boolean arg2)
                        => parent.WriteMessageFireAndForgetDiagnosticAsync<global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String, global::System.Boolean>, global::System.String>(1581213602, new global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String, global::System.Boolean>(arg0, arg1, arg2));

                }

                protected override void OnBroadcastEvent(global::System.Int32 methodId, global::System.ReadOnlyMemory<global::System.Byte> data)
                {
                    switch (methodId)
                    {
                        case -1005848884: // Void A()
                            {
                                var value = base.Deserialize<global::MessagePack.Nil>(data);
                                diagnosticHandler?.OnBroadcastEvent(this, "A", value);
                                receiver.A();
                            }
                            break;
                        case -955516027: // Void B(global::System.Int32 arg0, global::System.String arg1, global::System.Boolean arg2)
                            {
                                var value = base.Deserialize<global::MagicOnion.DynamicArgumentTuple<global::System.Int32, global::System.String, global::System.Boolean>>(data);
                                diagnosticHandler?.OnBroadcastEvent(this, "B", value);
                                receiver.B(value.Item1, value.Item2, value.Item3);
                            }
                            break;
                    }
                }

                protected override void OnResponseEvent(global::System.Int32 methodId, global::System.Object taskCompletionSource, global::System.ReadOnlyMemory<global::System.Byte> data)
                {
                    switch (methodId)
                    {
                        case 1497325507: // Task MethodA()
                            base.SetResultForResponse<global::MessagePack.Nil>(taskCompletionSource, data);
                            break;
                        case 1514103126: // Task<Int32> MethodB()
                            base.SetResultForResponse<global::System.Int32>(taskCompletionSource, data);
                            break;
                        case 1530880745: // ValueTask MethodC()
                            base.SetResultForResponse<global::MessagePack.Nil>(taskCompletionSource, data);
                            break;
                        case 1547658364: // ValueTask<Int32> MethodD()
                            base.SetResultForResponse<global::System.Int32>(taskCompletionSource, data);
                            break;
                        case 1564435983: // Task MethodE(global::System.Int32 arg0, global::System.String arg1)
                            base.SetResultForResponse<global::MessagePack.Nil>(taskCompletionSource, data);
                            break;
                        case 1581213602: // Task<String> MethodF(global::System.Int32 arg0, global::System.String arg1, global::System.Boolean arg2)
                            base.SetResultForResponse<global::System.String>(taskCompletionSource, data);
                            break;
                    }
                }

            }
        }
    }
}
