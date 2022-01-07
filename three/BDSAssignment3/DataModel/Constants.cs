using System;

namespace GrainStreamProcessing.Functions
{
    public static class Constants
    {
        public const string FilterNameSpace = "Filter";
        public const string FlatMapNameSpace = "FlatMap";
        public const string SinkNameSpace = "Sink";

        public static Guid StreamGuid = Guid.NewGuid();
    }
}