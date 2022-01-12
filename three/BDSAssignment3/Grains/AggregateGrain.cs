using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class AggregateGrain<T> : Grain, IAggregate, IAggregateFunction<T>
    {
        private IStreamProvider streamProvider;
        private const int windowSize = 6000;
        private const int slideSize = 3000;
        private long _curTime;
        
        protected readonly Dictionary<Guid, DataTuple> _tuples = new Dictionary<Guid, DataTuple>();

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
            Console.WriteLine("SourceGrain of stream Aggregate starts.");
            Guid.NewGuid();
            MyOutStream = nextStream;
            return Task.CompletedTask;
        }

        public abstract DataTuple Apply(T e);

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.
        private string MyInStream { get; set; } = Constants.AggregateNameSpace;
        private string MyOutStream { get; set; }

        public override async Task OnActivateAsync()
        {
            _curTime = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream;
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
            Console.WriteLine($"OnNextMessage in Aggregate: {message}");
            HandleTuples(message);

            await Process(message);
        }

        private void HandleTuples(DataTuple tuple)
        {
            _tuples.Add(Guid.NewGuid(), tuple);

            if (tuple.TimeStamp > _curTime + windowSize)
            {
                _curTime += slideSize;
            }


            foreach (KeyValuePair<Guid, DataTuple> pair in _tuples)
            {
                if (pair.Value.TimeStamp < _curTime-windowSize)
                {
                    _tuples.Remove(pair.Key);
                } 
            }
        }
    }

    public class AverageAggregate : AggregateGrain<DataTuple>
    {
        public override DataTuple Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            var res = new AggregateTuple<float>
            {
                AggregateValue = 0
            };
            
            foreach (KeyValuePair<Guid, DataTuple> tuple in _tuples)
            {
                if (e.UserId == tuple.Value.UserId)
                {
                    res.AggregateValue += tuple.Value.Long ?? 0;
                }
            }
            
            var matches = _tuples.Count(x => x.Value.UserId == e.UserId);

            res.AggregateValue /= matches;

            res.TimeStamp = _tuples.Values.Min(x => x.TimeStamp);
            
            return res;
        }
    }
}