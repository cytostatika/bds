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

[assembly: global::Orleans.Metadata.FeaturePopulatorAttribute(typeof(OrleansGeneratedCode.OrleansCodeGenConcurrencyFeaturePopulator))]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Concurrency, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"), global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Microsoft.Extensions.Logging.Abstractions, Version=6.0.0.0, Culture=neutral, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f33a29044fa9d740c9b3213a93e57c84b472c84e0b8a0e1ae48e67a9f8f6de9d5f7f3d52ac23e48ac51801f1dc950abe901da34d2a9e3baadb141a17c77ef3c565dd5ee5054b91cf63bb3c6ab83f72ab3aafe93d0fc3c2348b764fafb0b1c0733de51459aeab46580384bf9d74c4e28164b7cde247f891ba07891c9d872ad2bb")]
namespace OrleansGeneratedCode
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0")]
    internal sealed class OrleansCodeGenConcurrencyFeaturePopulator : global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainInterfaceFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainClassFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Serialization.SerializerFeature>
    {
        public void Populate(global::Orleans.Metadata.GrainInterfaceFeature feature)
        {
        }

        public void Populate(global::Orleans.Metadata.GrainClassFeature feature)
        {
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::Concurrency.Coordinator)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::Concurrency.TransactionalActor<>)));
        }

        public void Populate(global::Orleans.Serialization.SerializerFeature feature)
        {
            feature.AddKnownType("Concurrency.Coordinator,Concurrency", "Concurrency.Coordinator");
            feature.AddKnownType("Concurrency.TransactionalActor`1,Concurrency", "Concurrency.TransactionalActor`1'1");
            feature.AddKnownType("Microsoft.CodeAnalysis.EmbeddedAttribute,Microsoft.Extensions.Logging.Abstractions", "Microsoft.CodeAnalysis.EmbeddedAttribute");
            feature.AddKnownType("Microsoft.Extensions.Internal.TypeNameHelper+DisplayNameOptions,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Internal.DisplayNameOptions");
            feature.AddKnownType("Microsoft.Extensions.Logging.EventId,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.EventId");
            feature.AddKnownType("Microsoft.Extensions.Logging.FormattedLogValues,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.FormattedLogValues");
            feature.AddKnownType("Microsoft.Extensions.Logging.IExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.IExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILogger,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILogger");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILogger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILogger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILoggerFactory,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILoggerFactory");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILoggerProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILoggerProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.ISupportExternalScope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ISupportExternalScope");
            feature.AddKnownType("Microsoft.Extensions.Logging.LogDefineOptions,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogDefineOptions");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LoggerExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerExternalScopeProvider+Scope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Scope");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`2,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`2'2");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`3,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`3'3");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`4,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`4'4");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`5,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`5'5");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`6,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`6'6");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessageAttribute,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LoggerMessageAttribute");
            feature.AddKnownType("Microsoft.Extensions.Logging.Logger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Logger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.LogLevel,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogLevel");
            feature.AddKnownType("Microsoft.Extensions.Logging.LogValuesFormatter,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValuesFormatter");
            feature.AddKnownType("Microsoft.Extensions.Logging.NullExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.NullExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.NullScope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.NullScope");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.LogEntry`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.LogEntry`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLogger,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLogger");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLogger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLogger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.AllowNullAttribute", "AllowNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.DisallowNullAttribute", "DisallowNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.MaybeNullAttribute", "MaybeNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.NotNullAttribute", "NotNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute", "MaybeNullWhenAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.NotNullWhenAttribute", "NotNullWhenAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute", "NotNullIfNotNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.DoesNotReturnAttribute", "DoesNotReturnAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.DoesNotReturnIfAttribute", "DoesNotReturnIfAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.MemberNotNullAttribute", "MemberNotNullAttribute");
            feature.AddKnownType("System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute", "MemberNotNullWhenAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.IsReadOnlyAttribute", "IsReadOnlyAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.IsByRefLikeAttribute", "IsByRefLikeAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.NullableAttribute", "NullableAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.NullableContextAttribute", "NullableContextAttribute");
            feature.AddKnownType("System.Runtime.CompilerServices.NullablePublicOnlyAttribute", "NullablePublicOnlyAttribute");
            feature.AddKnownType("System.Text.ValueStringBuilder", "ValueStringBuilder");
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
