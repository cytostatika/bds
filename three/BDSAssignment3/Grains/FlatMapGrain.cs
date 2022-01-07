using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T> : Grain, IFlatMap, IFlatMapFunction<T>
    {
        public async Task Process(object e) // Implements the Process method from IFilter
        {
            var res = Apply((T) e);

            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, Constants.SinkNameSpace);

            await stream.OnNextAsync(res);
        }
        
        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream FlatMap starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }

        public abstract IList<DataTuple> Apply(T e);

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<DataTuple>(Constants.StreamGuid,Constants.FlatMapNameSpace);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                {
                    await subscriptionHandle.ResumeAsync(OnNextMessage);
                }
            }

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage(DataTuple message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage in FlatMap: {message}");


            await Process(message);
        }
    }
    
    public class AddMap : FlatMapGrain<DataTuple>
    {
        public override IList<DataTuple> Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            var res = new List<DataTuple> {e};

            foreach (var dataTuple in res) dataTuple.UserId += 10;
            return res;
        }
    }
}