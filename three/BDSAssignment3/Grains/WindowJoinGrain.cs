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
    public abstract class WindowJoinGrain<T> : Grain, IWindowJoin, IWindowJoinFunction<T>
    {
        private readonly string inStream1 = Constants.WindowJoinOneNameSpace;
        private readonly string inStream2 = Constants.WindowJoinTwoNameSpace;
        protected Dictionary<string, DataTuple> DictStream1;
        protected Dictionary<string, DataTuple> DictStream2;
        protected long StartTime;
        protected long WindowSize;
        private string OutStream { get; set; }

        // TODO: remove parameters, its already in state lol
        public async Task Process()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var window = Apply();
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, OutStream);

            await stream.OnNextAsync(window);
        }

        public Task Init(string in1, string in2, string out1, long wdSize)
        {
            OutStream = out1;
            WindowSize = wdSize;
            StartTime = 0;
            DictStream1 = new Dictionary<string, DataTuple>();
            DictStream2 = new Dictionary<string, DataTuple>();
            return Task.CompletedTask;
        }

        public abstract Task OnNextMessage1((string, DataTuple, long) message, StreamSequenceToken sequenceToken);

        public abstract Task OnNextMessage2((string, DataTuple, long) message, StreamSequenceToken sequenceToken);
        // private IStreamProvider streamProvider;

        public abstract List<(string, T, long)> Apply();

        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream1 = streamProvider.GetStream<(string, DataTuple, long)>(Constants.StreamGuid, inStream1);
            await stream1.SubscribeAsync(OnNextMessage1);
            var stream2 = streamProvider.GetStream<(string, DataTuple, long)>(Constants.StreamGuid, inStream2);
            await stream2.SubscribeAsync(OnNextMessage2);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream1.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage1);
            var subscriptionHandles2 = await stream2.GetAllSubscriptionHandles();
            if (subscriptionHandles2.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles2)
                    await subscriptionHandle.ResumeAsync(OnNextMessage2);
        }
    }


    public class SimpleWindowJoin : WindowJoinGrain<DataTuple>
    {
        public override List<(string, DataTuple, long)> Apply()
        {
            var s1 = DictStream1;
            var s2 = DictStream2;
            var matches = s1.Keys.Intersect(s2.Keys);

            var res = new List<(string, DataTuple, long)>();
            foreach (var m in matches)
            {
                var tuple1 = s1[m];
                var tuple2 = s2[m];

                res.Add(("UserId", new MergeTuple(tuple1, tuple2, "UserId"), StartTime));
            }

            DictStream1.Clear();
            DictStream2.Clear();

            return res;
        }

        public override async Task OnNextMessage1((string, DataTuple, long) message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine("OnNextMessage1");
            var payload = message.Item2;
            var key = payload.UserId.First().ToString();
            var ts = message.Item3;
            DictStream1[key] = payload;
            if (StartTime == 0)
            {
                StartTime = ts;
            }
            else if (ts - StartTime > WindowSize)
            {
                await Process();
                StartTime = 0;
            }
        }

        public override async Task OnNextMessage2((string, DataTuple, long) message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine("OnNextMessage2");
            var payload = message.Item2;
            var key = payload.UserId.First().ToString();
            var ts = message.Item3;
            DictStream2[key] = payload;
            if (StartTime == 0)
            {
                StartTime = ts;
            }
            else if (ts - StartTime > WindowSize)
            {
                await Process();
                StartTime = 0;
            }
        }
    }
}