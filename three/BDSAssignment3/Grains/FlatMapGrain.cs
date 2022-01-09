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
        private IStreamProvider streamProvider;

        public async Task Process(object e) // Implements the Process method from IFilter
        {
            var res = Apply((T) e);
            var outStream = MyOutStream();
            //Get the reference to a stream
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(res);
        }

        public Task Init()
        {
            Console.WriteLine("SourceGrain of stream FlatMap starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }

        public abstract IList<DataTuple> Apply(T e);

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.
        public abstract string MyInStream();
        public abstract string MyOutStream();

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream();
            var stream = streamProvider.GetStream<DataTuple>(Constants.StreamGuid, inStream);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

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

        public override string MyInStream()
        {
            return Constants.FlatMapNameSpace;
        }

        public override string MyOutStream()
        {
            return Constants.SinkNameSpace;
        }
    }
}