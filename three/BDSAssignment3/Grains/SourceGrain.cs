using System;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public class SourceGrain : Grain, ISource
    {
        private string _streamName;


        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream {_streamName} starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            var primaryKey = this.GetPrimaryKey(out _streamName);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(primaryKey, _streamName);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();

            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"Stream {_streamName} receives: {message}.");
            //Add your logic here to process received data
            //Get one of the providers which we defined in our config
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream

            var stream =
                streamProvider.GetStream<(string, DataTuple, long)>(Constants.StreamGuid, Constants.AggregateNameSpace);

            var parsedMessage = ParseStream(message, _streamName);

            await stream.OnNextAsync(parsedMessage);
        }

        private (string, DataTuple, long) ParseStream(string message, string streamNameParse)
        {
            var numbers = message.Split();
            return streamNameParse switch
            {
                "Photo" => ("", new PhotoTuple(numbers), long.Parse(numbers[4])),
                "GPS" => ("", new GpsTuple(numbers), long.Parse(numbers[3])),
                _ => ("", new TagTuple(numbers), long.Parse(numbers[2]))
            };
        }
    }
}