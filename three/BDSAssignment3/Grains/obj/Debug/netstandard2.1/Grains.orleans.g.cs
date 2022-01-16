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

[assembly: global::Orleans.Metadata.FeaturePopulatorAttribute(typeof(OrleansGeneratedCode.OrleansCodeGen5c914b08e0FeaturePopulator))]
[assembly: global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Grains, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"), global::Orleans.CodeGeneration.OrleansCodeGenerationTargetAttribute("Microsoft.Extensions.Logging.Abstractions, Version=3.1.0.0, Culture=neutral, PublicKey=0024000004800000940000000602000000240000525341310004000001000100f33a29044fa9d740c9b3213a93e57c84b472c84e0b8a0e1ae48e67a9f8f6de9d5f7f3d52ac23e48ac51801f1dc950abe901da34d2a9e3baadb141a17c77ef3c565dd5ee5054b91cf63bb3c6ab83f72ab3aafe93d0fc3c2348b764fafb0b1c0733de51459aeab46580384bf9d74c4e28164b7cde247f891ba07891c9d872ad2bb")]
namespace OrleansGeneratedCode
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("OrleansCodeGen", "2.0.0.0")]
    internal sealed class OrleansCodeGen5c914b08e0FeaturePopulator : global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainInterfaceFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Metadata.GrainClassFeature>, global::Orleans.Metadata.IFeaturePopulator<global::Orleans.Serialization.SerializerFeature>
    {
        public void Populate(global::Orleans.Metadata.GrainInterfaceFeature feature)
        {
        }

        public void Populate(global::Orleans.Metadata.GrainClassFeature feature)
        {
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.AggregateGrain<>)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.AverageLongitudeAggregate)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.FilterGrain<>)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.OddNumberFilter)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.FlatMapGrain<>)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.AddMap)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.SinkGrain)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.ConsoleSink)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.FileSink)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.SourceGrain)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.WindowJoinGrain<>)));
            feature.Classes.Add(new global::Orleans.Metadata.GrainClassMetadata(typeof(global::GrainStreamProcessing.GrainImpl.SimpleWindowJoin)));
        }

        public void Populate(global::Orleans.Serialization.SerializerFeature feature)
        {
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.AggregateGrain`1,Grains", "GrainStreamProcessing.GrainImpl.AggregateGrain`1'1");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.AverageLongitudeAggregate,Grains", "GrainStreamProcessing.GrainImpl.AverageLongitudeAggregate");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.FilterGrain`1,Grains", "GrainStreamProcessing.GrainImpl.FilterGrain`1'1");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.OddNumberFilter,Grains", "GrainStreamProcessing.GrainImpl.OddNumberFilter");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.FlatMapGrain`1,Grains", "GrainStreamProcessing.GrainImpl.FlatMapGrain`1'1");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.AddMap,Grains", "GrainStreamProcessing.GrainImpl.AddMap");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.SinkGrain,Grains", "GrainStreamProcessing.GrainImpl.SinkGrain");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.ConsoleSink,Grains", "GrainStreamProcessing.GrainImpl.ConsoleSink");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.FileSink,Grains", "GrainStreamProcessing.GrainImpl.FileSink");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.SourceGrain,Grains", "GrainStreamProcessing.GrainImpl.SourceGrain");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.WindowJoinGrain`1,Grains", "GrainStreamProcessing.GrainImpl.WindowJoinGrain`1'1");
            feature.AddKnownType("GrainStreamProcessing.GrainImpl.SimpleWindowJoin,Grains", "GrainStreamProcessing.GrainImpl.SimpleWindowJoin");
            feature.AddKnownType("Microsoft.CodeAnalysis.EmbeddedAttribute,Microsoft.Extensions.Logging.Abstractions", "Microsoft.CodeAnalysis.EmbeddedAttribute");
            feature.AddKnownType("Microsoft.Extensions.Internal.TypeNameHelper,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Internal.TypeNameHelper");
            feature.AddKnownType("Microsoft.Extensions.Internal.TypeNameHelper+DisplayNameOptions,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Internal.DisplayNameOptions");
            feature.AddKnownType("Microsoft.Extensions.Logging.EventId,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.EventId");
            feature.AddKnownType("Microsoft.Extensions.Logging.FormattedLogValues,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.FormattedLogValues");
            feature.AddKnownType("Microsoft.Extensions.Logging.IExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.IExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILogger,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILogger");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILogger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILogger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILoggerFactory,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILoggerFactory");
            feature.AddKnownType("Microsoft.Extensions.Logging.ILoggerProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ILoggerProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.ISupportExternalScope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.ISupportExternalScope");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LoggerExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerExternalScopeProvider+Scope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Scope");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`2,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`2'2");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`3,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`3'3");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`4,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`4'4");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`5,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`5'5");
            feature.AddKnownType("Microsoft.Extensions.Logging.LoggerMessage+LogValues`6,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValues`6'6");
            feature.AddKnownType("Microsoft.Extensions.Logging.Logger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Logger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.LogLevel,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogLevel");
            feature.AddKnownType("Microsoft.Extensions.Logging.LogValuesFormatter,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.LogValuesFormatter");
            feature.AddKnownType("Microsoft.Extensions.Logging.NullExternalScopeProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.NullExternalScopeProvider");
            feature.AddKnownType("Microsoft.Extensions.Logging.NullScope,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.NullScope");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLogger,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLogger");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLogger`1,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLogger`1'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLogger`1+NullDisposable,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullDisposable'1");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLoggerFactory");
            feature.AddKnownType("Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider,Microsoft.Extensions.Logging.Abstractions", "Microsoft.Extensions.Logging.Abstractions.NullLoggerProvider");
            feature.AddKnownType("System.Runtime.CompilerServices.IsReadOnlyAttribute,Microsoft.Extensions.Logging.Abstractions", "IsReadOnlyAttribute");
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
