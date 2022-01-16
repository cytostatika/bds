using System;
using System.Collections;
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
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {
        private IStreamProvider streamProvider;
        private string MyInStream { get; } = Constants.FilterNameSpace;
        private string MyOutStream { get; set; }

        public async Task Process(object e) // Implements the Process method from IFilter
        {
            var list = e as IList;
            if (list != null)
            {
                foreach (var tup in list)
                {
                    if (!ApplyOne(((string, DataTuple, long)) tup)) continue;
                    //Get the reference to a stream
                    var outStream = MyOutStream;
                    var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

                    await stream.OnNextAsync(tup);
                }
            }
            else
            {
                if (Apply((T) e))
                {
                    //Get the reference to a stream
                    var outStream = MyOutStream;
                    var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

                    await stream.OnNextAsync(e);
                }
            }
        }

        public Task Init(string nextStream)
        {
            Console.WriteLine("SourceGrain of stream Filter starts.");
            Guid.NewGuid();
            MyOutStream = nextStream;
            return Task.CompletedTask;
        }

        public abstract bool Apply(T e);

        public abstract bool
            ApplyOne((string, DataTuple, long) e); // Implements the Apply method, filtering odd numbers

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
            //Console.WriteLine($"OnNextMessage in Filter: {message}");
            await Process(message);
        }
    }

    public class OddNumberFilter : FilterGrain<(string, DataTuple, long)>
    {
        public override bool Apply((string, DataTuple, long) e) // Implements the Apply method, filtering odd numbers
        {
            return e.Item2.UserId.Any(x => x % 2 == 1);
        }

        public override bool ApplyOne((string, DataTuple, long) e) // Implements the Apply method, filtering odd numbers
        {
            return e.Item2.UserId.Any(x => x % 2 == 1);
        }
    }

    public class OddNumberListFilter : FilterGrain<List<(string, DataTuple, long)>>
    {
        public override bool Apply(List<(string, DataTuple, long)> e)
        {
            foreach (var tup in e) return tup.Item2.UserId.Any(x => x % 2 == 1);
            return false;
        }

        public override bool ApplyOne((string, DataTuple, long) e) // Implements the Apply method, filtering odd numbers
        {
            return e.Item2.UserId.Any(x => x % 2 == 1);
        }
    }
}