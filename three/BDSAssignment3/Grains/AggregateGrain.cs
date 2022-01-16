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
    public abstract class AggregateGrain<T> : Grain, IAggregate, IAggregateFunction<T>
    {
        private const int WindowSize = 6;
        private const int SlideSize = 1;

        protected readonly Dictionary<int, T> Tuples = new Dictionary<int, T>();
        private IStreamProvider _streamProvider;
        private int _tupleNumber;

        private string MyInStream { get; } = Constants.AggregateNameSpace;
        private string MyOutStream { get; set; }

        public async Task Process(object e) // Implements the Process
        {
            var res = Apply((T) e);
            var outStream = MyOutStream;
            //Get the reference to a stream
            var stream = _streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(res);
        }

        public Task Init(string nextStream)
        {
            Console.WriteLine("SourceGrain of stream Aggregate starts.");
            Guid.NewGuid();
            MyOutStream = nextStream;
            return Task.CompletedTask;
        }

        public abstract (string, DataTuple, long) Apply(T e);

        public override async Task OnActivateAsync()
        {
            _streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream;
            var stream = _streamProvider.GetStream<T>(Constants.StreamGuid, inStream);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage(T message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"OnNextMessage in Aggregate: {message}");
            await Process(message);
        }

        protected void HandleTuples(T tuple)
        {
            Tuples.Add(_tupleNumber++, tuple);

            if (Tuples.Count <= WindowSize) return;

            for (var i = 0; i < SlideSize; i++) Tuples.Remove(Tuples.Keys.Min());
        }
    }

    public class AverageLongitudeAggregate : AggregateGrain<(string, DataTuple, long)>
    {
        public override (string, DataTuple, long) Apply((string, DataTuple, long) e)
        {
            HandleTuples(e);

            var res = new AggregateTuple<float>
            {
                AggregateValue = 0
            };
            var matches = 0;

            var eventKey = e.Item2.GetType().GetProperties().First(x => x.Name == e.Item1).GetValue(e.Item2, null);

            foreach (var (key, (mes, pay, time)) in Tuples)
            {
                var dictItemKey = pay.GetType().GetProperties().First(x => x.Name == mes).GetValue(pay, null);

                if (!eventKey.ToString().Equals(dictItemKey.ToString())) continue;

                res.AggregateValue += pay.Long.Sum();
                matches += pay.Long.Count;
            }

            res.AggregateValue = matches == 0 ? res.AggregateValue : res.AggregateValue / matches;

            var timeStamp = Tuples.Values.Min(x => x.Item3);

            return (e.Item1, res, timeStamp);
        }
    }
}