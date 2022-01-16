using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using GrainStreamProcessing.Model;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T> : Grain, IFlatMap, IFlatMapFunction<T>
    {
        private IStreamProvider streamProvider;

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.
        private string MyInStream { get; } = Constants.FlatMapNameSpace;
        private string MyOutStream { get; set; }

        public async Task Process(object e) // Implements the Process method from IFilter
        {
            var res = Apply((T) e);
            var outStream = MyOutStream;
            //Get the reference to a stream
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(res);
        }

        public Task Init(string nextStream)
        {
            Console.WriteLine("SourceGrain of stream FlatMap starts.");
            Guid.NewGuid();
            MyOutStream = nextStream;
            return Task.CompletedTask;
        }

        // public abstract List<(string, T, long)> Apply((string, T, long) e);
        public abstract List<(string, DataTuple, long)> Apply(T e);

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream;
            var stream = streamProvider.GetStream<T>(Constants.StreamGuid, inStream);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage(T message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"OnNextMessage in FlatMap: {message}");
            await Process(message);
        }
    }

    public class AddListMap : FlatMapGrain<List<(string, DataTuple, long)>>
    {
        public override List<(string, DataTuple, long)>
            Apply(List<(string, DataTuple, long)> valueTuple) // Implements the Apply method, filtering odd numbers
        {
            // var res = new List<(string, DataTuple, long)> {valueTuple};
            //
            // foreach (var x in res) x.Item2.UserId = x.Item2.UserId.Select(y => y + 10).ToList();
            //

            return valueTuple;
        }
    }

    public class AddMap : FlatMapGrain<(string, DataTuple, long)>
    {
        public override List<(string, DataTuple, long)> Apply((string, DataTuple, long) e)
        {
            var res = new List<(string, DataTuple, long)> {e};

            foreach (var x in res) x.Item2.UserId = x.Item2.UserId.Select(y => y + 10).ToList();


            return res;
        }
    }
}