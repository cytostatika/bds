using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using GrainStreamProcessing.GrainInterfaces;
using System.Threading.Tasks;
using System;
using System.Runtime.CompilerServices;

namespace GrainStreamProcessing.GrainImpl
{
    public class SourceGrain : Grain, ISource
    {
        private string _streamName;
        private Guid _filterGuid;

        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream {_streamName} starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }
        public override async Task OnActivateAsync()
        {
            _filterGuid = Guid.NewGuid();
            var primaryKey = this.GetPrimaryKey(out _streamName);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(primaryKey, _streamName);

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

        private async Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"Stream {_streamName} receives: {message}.");
            //Add your logic here to process received data
            //Get one of the providers which we defined in our config
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream

            var stream = streamProvider.GetStream<ITuple>(_filterGuid, "Filter");

            var parsedMessage = ParseStream(message, _streamName);
            
            await stream.OnNextAsync(parsedMessage);
        }

            private ITuple ParseStream(string message, string streamNameParse)
            {
                var numbers = message.Split();

                return streamNameParse switch
                {
                    "Photo" => new Tuple<int, int, float, float>(int.Parse(numbers[0]), int.Parse(numbers[1]),
                        float.Parse(numbers[2]), float.Parse(numbers[3])),
                    "GPS" => new Tuple<int, float, float>(int.Parse(numbers[0]), float.Parse(numbers[1]),
                        float.Parse(numbers[2])),
                    _ => new Tuple<int, int>(int.Parse(numbers[0]), int.Parse(numbers[1]))
                };
            }
    }
}
