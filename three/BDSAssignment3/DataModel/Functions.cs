using System;
using System.Collections.Generic;

namespace GrainStreamProcessing.Functions
{
    public interface IFilterFunction<T>
    {
        bool Apply(T e);
    }
}
