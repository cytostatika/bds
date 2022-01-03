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
        List<T> Apply(T e);
    }
}
