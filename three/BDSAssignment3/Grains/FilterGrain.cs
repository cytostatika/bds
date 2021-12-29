using Orleans;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using System;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {
        public abstract bool Apply(T e); 
        public Task Process(object e) // Implements the Process method from IFilter
        {
            if (Apply((T)e)) // If the function returns true, send the element to SinkGrain
                {
                    this.GrainFactory.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(e);
                } // Otherwise, skip it
            return Task.CompletedTask;
        }
    }
    public class LargerThanTenFilter : FilterGrain<long>
    {
        public override bool Apply(long e) // Implements the Apply method, filtering numbers larger than 10
        {
             if (e > 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
    }

    public class OddNumberFilter : FilterGrain<long>
    {
        public override bool Apply(long e) // Implements the Apply method, filtering odd numbers
        {
            if (e % 2 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
