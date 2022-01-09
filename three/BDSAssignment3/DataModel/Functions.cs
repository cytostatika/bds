using System;
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

    public interface IWindowJoinFunction<T>
    {
        IList<string> Apply(T e); // TODO: Change string into MergeTuple
    }
}
