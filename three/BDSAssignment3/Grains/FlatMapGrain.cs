using System.Collections.Generic;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T> : Grain, IFlatMap, IFlatMapFunction<T>
    {
        public abstract List<T> Apply(T e);

        public Task Process(object e)
        {
            this.GrainFactory.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(e);

            return Task.CompletedTask;
        }
    }


}