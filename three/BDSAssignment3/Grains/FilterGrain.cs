using System;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {
        private IStreamProvider streamProvider;

        public async Task Process(object e) // Implements the Process method from IFilter
        {
            if (Apply((T) e))
            {
                //Get the reference to a stream
                var outStream = MyOutStream();
                var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

                await stream.OnNextAsync(e);
            }
        }

        public Task Init()
        {
            Console.WriteLine("SourceGrain of stream Filter starts.");

            return Task.CompletedTask;
        }

        public abstract bool Apply(T e);

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
            Console.WriteLine($"OnNextMessage in Filter: {message}");


            await Process(message);
        }
    }


    public class OddNumberFilter : FilterGrain<DataTuple>
    {
        public override bool Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            if (e.UserId % 2 == 1)
                return true;
            return false;
        }

        public override string MyInStream()
        {
            return Constants.FilterNameSpace;
        }

        public override string MyOutStream()
        {
            return Constants.SinkNameSpace;
        }
    }
}