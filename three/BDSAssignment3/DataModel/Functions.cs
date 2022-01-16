using System.Collections.Generic;
using GrainStreamProcessing.Model;

namespace GrainStreamProcessing.Functions
{
    public interface IFilterFunction<in T>
    {
        bool Apply(T e);
    }

    public interface IFlatMapFunction<in T>
    {
        List<(string, DataTuple, long)> Apply(T e);
    }

    public interface IAggregateFunction<T>
    {
        (string, DataTuple, long) Apply(T e);
    }

    public interface IWindowJoinFunction<T>
    {
        List<(string, T, long)> Apply();
    }
}