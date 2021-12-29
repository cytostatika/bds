using System;
using System.Threading.Tasks;
using GrainStreamProcessing.GrainInterfaces;

namespace GrainStreamProcessing.GrainImpl
{
    public class SinkGrain : Orleans.Grain, ISink
    {
        public Task Process(object e)
        {
            Console.WriteLine(e);// Output received data to Silo console
            return Task.CompletedTask;
        }
    }
}
