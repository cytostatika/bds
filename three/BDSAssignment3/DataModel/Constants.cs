using System;

namespace GrainStreamProcessing.Functions
{
    public static class Constants
    {
        public const string FilterNameSpace = "Filter";
        public const string FlatMapNameSpace = "FlatMap";
        public const string SinkNameSpace = "Sink";
        public const string AggregateNameSpace = "Aggregate";
        public const string WindowJoinOneNameSpace = "Window1";
        public const string WindowJoinTwoNameSpace = "Window2";

        public static Guid StreamGuid = Guid.NewGuid();
    }
}