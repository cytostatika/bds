// <auto-generated />
#if !EXCLUDE_GENERATED_CODE
#pragma warning disable 162
#pragma warning disable 219
#pragma warning disable 414
#pragma warning disable 618
#pragma warning disable 649
#pragma warning disable 693
#pragma warning disable 1591
#pragma warning disable 1998
using global::Orleans;

[assembly: global::Orleans.Metadata.FeaturePopulatorAttribute(typeof(OrleansGeneratedCode.OrleansCodeGenGrain_InterfaceFeaturePopulator))]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Grain.Interface, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null")]
namespace Grain.Interface
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0"), global::Orleans.CodeGeneration.MethodInvokerAttribute(typeof(global::Grain.Interface.IAccountActor), (int)0x485CAB3), global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute]
    internal class OrleansCodeGenAccountActorMethodInvoker : global::Orleans.CodeGeneration.IGrainMethodInvoker
    {
        public async global::System.Threading.Tasks.Task<object> Invoke(global::Orleans.Runtime.IAddressable grain, global::Orleans.CodeGeneration.InvokeMethodRequest request)
        {
            int interfaceId = request.InterfaceId;
            int methodId = request.MethodId;
            var arguments = request.Arguments;
            switch (interfaceId)
            {
                case (int)0x485CAB3:
                {
                    var casted = ((global::Grain.Interface.IAccountActor)grain);
                    switch (methodId)
                    {
                        case (int)0x41A87E1B:
                            return await casted.Init((global::Utilities.TransactionContext)arguments[0], (object)arguments[1]);
                        case (int)0x251D8A74:
                            return await casted.GetBalance((global::Utilities.TransactionContext)arguments[0], (object)arguments[1]);
                        case (int)0x3515AF4B:
                            return await casted.Transfer((global::Utilities.TransactionContext)arguments[0], (object)arguments[1]);
                        case (int)0x271A7AE0:
                            return await casted.Deposit((global::Utilities.TransactionContext)arguments[0], (object)arguments[1]);
                        case (int)0x5B8D58B3:
                            return await casted.SubmitTransaction((string)arguments[0], (object)arguments[1], (global::System.Collections.Generic.List<int>)arguments[2]);
                        case unchecked((int)0xC8F2DBDF):
                            await casted.ReceiveBatch((global::Utilities.Batch)arguments[0]);
                            return null;
                        case (int)0x7C9500C:
                            return await casted.Execute((global::Utilities.FunctionCall)arguments[0], (global::Utilities.TransactionContext)arguments[1]);
                        case unchecked((int)0xA8876204):
                            await casted.BatchCommit((int)arguments[0]);
                            return null;
                        case (int)0x79B3DA57:
                            await casted.CheckGarbageCollection();
                            return null;
                        default:
                            ThrowMethodNotImplemented(interfaceId, methodId);
                            return null;
                    }
                }

                case (int)0xDB6716B:
                {
                    var casted = ((global::Concurrency.Interface.ITransactionalActor)grain);
                    switch (methodId)
                    {
                        case (int)0x5B8D58B3:
                            return await casted.SubmitTransaction((string)arguments[0], (object)arguments[1], (global::System.Collections.Generic.List<int>)arguments[2]);
                        case unchecked((int)0xC8F2DBDF):
                            await casted.ReceiveBatch((global::Utilities.Batch)arguments[0]);
                            return null;
                        case (int)0x7C9500C:
                            return await casted.Execute((global::Utilities.FunctionCall)arguments[0], (global::Utilities.TransactionContext)arguments[1]);
                        case unchecked((int)0xA8876204):
                            await casted.BatchCommit((int)arguments[0]);
                            return null;
                        case (int)0x79B3DA57:
                            await casted.CheckGarbageCollection();
                            return null;
                        default:
                            ThrowMethodNotImplemented(interfaceId, methodId);
                            return null;
                    }
                }

                default:
                    ThrowInterfaceNotImplemented(interfaceId);
                    return null;
            }

            void ThrowInterfaceNotImplemented(int i) => throw new global::System.NotImplementedException($"InterfaceId: 0x{i:X}");
            void ThrowMethodNotImplemented(int i, int m) => throw new global::System.NotImplementedException($"InterfaceId: 0x{i:X}, MethodId: 0x{m:X}");
        }

        public int InterfaceId => (int)0x485CAB3;
        public ushort InterfaceVersion => 0;
    }

    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0"), global::System.SerializableAttribute, global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute, global::Orleans.CodeGeneration.GrainReferenceAttribute(typeof(global::Grain.Interface.IAccountActor))]
    internal class OrleansCodeGenAccountActorReference : global::Orleans.Runtime.GrainReference, global::Grain.Interface.IAccountActor
    {
        OrleansCodeGenAccountActorReference(global::Orleans.Runtime.GrainReference other): base(other)
        {
        }

        OrleansCodeGenAccountActorReference(global::Orleans.Runtime.GrainReference other, global::Orleans.CodeGeneration.InvokeMethodOptions invokeMethodOptions): base(other, invokeMethodOptions)
        {
        }

        OrleansCodeGenAccountActorReference(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context): base(info, context)
        {
        }

        public override int InterfaceId => (int)0x485CAB3;
        public override ushort InterfaceVersion => 0;
        public override string InterfaceName => "IAccountActor";
        public override bool IsCompatible(int interfaceId) => interfaceId == (int)0x485CAB3 || interfaceId == (int)0xDB6716B;
        public override string GetMethodName(int interfaceId, int methodId)
        {
            switch (interfaceId)
            {
                case (int)0x485CAB3:
                {
                    switch (methodId)
                    {
                        case (int)0x41A87E1B:
                            return "Init";
                        case (int)0x251D8A74:
                            return "GetBalance";
                        case (int)0x3515AF4B:
                            return "Transfer";
                        case (int)0x271A7AE0:
                            return "Deposit";
                        case (int)0x5B8D58B3:
                            return "SubmitTransaction";
                        case unchecked((int)0xC8F2DBDF):
                            return "ReceiveBatch";
                        case (int)0x7C9500C:
                            return "Execute";
                        case unchecked((int)0xA8876204):
                            return "BatchCommit";
                        case (int)0x79B3DA57:
                            return "CheckGarbageCollection";
                        default:
                            ThrowMethodNotImplemented(interfaceId, methodId);
                            return null;
                    }
                }

                case (int)0xDB6716B:
                {
                    switch (methodId)
                    {
                        case (int)0x5B8D58B3:
                            return "SubmitTransaction";
                        case unchecked((int)0xC8F2DBDF):
                            return "ReceiveBatch";
                        case (int)0x7C9500C:
                            return "Execute";
                        case unchecked((int)0xA8876204):
                            return "BatchCommit";
                        case (int)0x79B3DA57:
                            return "CheckGarbageCollection";
                        default:
                            ThrowMethodNotImplemented(interfaceId, methodId);
                            return null;
                    }
                }

                default:
                    ThrowInterfaceNotImplemented(interfaceId);
                    return null;
            }

            void ThrowInterfaceNotImplemented(int i) => throw new global::System.NotImplementedException($"InterfaceId: 0x{i:X}");
            void ThrowMethodNotImplemented(int i, int m) => throw new global::System.NotImplementedException($"InterfaceId: 0x{i:X}, MethodId: 0x{m:X}");
        }

        global::System.Threading.Tasks.Task<object> global::Grain.Interface.IAccountActor.Init(global::Utilities.TransactionContext context0, object funcInput1)
        {
            return base.InvokeMethodAsync<object>((int)0x41A87E1B, new object[]{context0, funcInput1});
        }

        global::System.Threading.Tasks.Task<object> global::Grain.Interface.IAccountActor.GetBalance(global::Utilities.TransactionContext context0, object funcInput1)
        {
            return base.InvokeMethodAsync<object>((int)0x251D8A74, new object[]{context0, funcInput1});
        }

        global::System.Threading.Tasks.Task<object> global::Grain.Interface.IAccountActor.Transfer(global::Utilities.TransactionContext context0, object funcInput1)
        {
            return base.InvokeMethodAsync<object>((int)0x3515AF4B, new object[]{context0, funcInput1});
        }

        global::System.Threading.Tasks.Task<object> global::Grain.Interface.IAccountActor.Deposit(global::Utilities.TransactionContext context0, object funcInput1)
        {
            return base.InvokeMethodAsync<object>((int)0x271A7AE0, new object[]{context0, funcInput1});
        }

        global::System.Threading.Tasks.Task<object> global::Concurrency.Interface.ITransactionalActor.SubmitTransaction(string funcName0, object funcInput1, global::System.Collections.Generic.List<int> actorAccessInfo2)
        {
            return base.InvokeMethodAsync<object>((int)0x5B8D58B3, new object[]{funcName0, funcInput1, actorAccessInfo2});
        }

        global::System.Threading.Tasks.Task global::Concurrency.Interface.ITransactionalActor.ReceiveBatch(global::Utilities.Batch batch0)
        {
            return base.InvokeMethodAsync<object>(unchecked((int)0xC8F2DBDF), new object[]{batch0});
        }

        global::System.Threading.Tasks.Task<object> global::Concurrency.Interface.ITransactionalActor.Execute(global::Utilities.FunctionCall call0, global::Utilities.TransactionContext context1)
        {
            return base.InvokeMethodAsync<object>((int)0x7C9500C, new object[]{call0, context1});
        }

        global::System.Threading.Tasks.Task global::Concurrency.Interface.ITransactionalActor.BatchCommit(int bid0)
        {
            return base.InvokeMethodAsync<object>(unchecked((int)0xA8876204), new object[]{bid0});
        }

        global::System.Threading.Tasks.Task global::Concurrency.Interface.ITransactionalActor.CheckGarbageCollection()
        {
            return base.InvokeMethodAsync<object>((int)0x79B3DA57, null);
        }
    }
}

namespace OrleansGeneratedCode
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0")]
    internal sealed class OrleansCodeGenGrain_InterfaceFeaturePopulator : global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainInterfaceFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainClassFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Serialization.SerializerFeature>
    {
        public void Populate(global::Orleans.Metadata.GrainInterfaceFeature feature)
        {
            feature.Interfaces.Add(new global::Orleans.Metadata.GrainInterfaceMetadata(typeof(global::Grain.Interface.IAccountActor), typeof(Grain.Interface.OrleansCodeGenAccountActorReference), typeof(Grain.Interface.OrleansCodeGenAccountActorMethodInvoker), (int)0x485CAB3));
        }

        public void Populate(global::Orleans.Metadata.GrainClassFeature feature)
        {
        }

        public void Populate(global::Orleans.Serialization.SerializerFeature feature)
        {
            feature.AddKnownType("Grain.Interface.IAccountActor,Grain.Interface", "Grain.Interface.IAccountActor");
            feature.AddKnownType("System.Threading.Tasks.Task`1", "Task`1'1");
            feature.AddKnownType("System.Threading.Tasks.Task", "Task");
            feature.AddKnownType("Utilities.TransactionContext,Utilities", "Utilities.TransactionContext");
        }
    }
}
#pragma warning restore 162
#pragma warning restore 219
#pragma warning restore 414
#pragma warning restore 618
#pragma warning restore 649
#pragma warning restore 693
#pragma warning restore 1591
#pragma warning restore 1998
#endif
