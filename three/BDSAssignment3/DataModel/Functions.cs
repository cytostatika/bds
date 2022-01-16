using System.Collections.Generic;

namespace GrainStreamProcessing.Functions
{
    public interface IFilterFunction<T>
    {
        bool Apply((string, T, long) e);
    }

    public interface IFlatMapFunction<T>
    {
        IList<(string, T, long)> Apply((string, T, long) e);
        IList<(string, T, long)> Apply(IList<(string, T, long)> e);

    }

    public interface IAggregateFunction<T>
    {
        (string, T, long) Apply((string, T, long) e);
    }

    public interface IWindowJoinFunction<T>
    {
        IList<(string, T, long)> Apply();
    }
}