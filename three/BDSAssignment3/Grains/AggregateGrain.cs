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

        protected readonly Dictionary<int, (string, T, long)> Tuples = new Dictionary<int, (string, T, long)>();
        private IStreamProvider _streamProvider;
        private int _tupleNumber;

        private string MyInStream { get; } = Constants.AggregateNameSpace;
        private string MyOutStream { get; set; }

        public async Task Process(object e) // Implements the Process
        {
            var res = Apply(((string, T, long)) e);
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

        public abstract (string, T, long) Apply((string, T, long) e);

        public override async Task OnActivateAsync()
        {
            _streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream;
            var stream = _streamProvider.GetStream<(string, T, long)>(Constants.StreamGuid, inStream);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage((string, T, long) message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"OnNextMessage in Aggregate: {message}");
            await Process(message);
        }

        protected void HandleTuples((string, T, long) tuple)
        {
            Tuples.Add(_tupleNumber++, tuple);

            if (Tuples.Count <= WindowSize) return;

            for (var i = 0; i < SlideSize; i++) Tuples.Remove(Tuples.Keys.Min());
        }
    }

    public class AverageLongitudeAggregate : AggregateGrain<DataTuple>
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