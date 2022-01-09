using System.Collections.Generic;

namespace GrainStreamProcessing.Functions
{
    public interface IFilterFunction<T>
    {
        bool Apply(T e);
    }

    public interface IFlatMapFunction<T>
    {
        IList<DataTuple> Apply(T e);
    }

    public interface IAggregateFunction<T>
    {
        DataTuple Apply(T e);
    }
}