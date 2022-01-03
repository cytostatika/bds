using Orleans;
using Orleans.Providers;
using Orleans.Streams;
using GrainStreamProcessing.GrainInterfaces;
using System.Threading.Tasks;
using System;

namespace GrainStreamProcessing.GrainImpl
{
    public class SourceGrain : Grain, ISource
    {
        string streamName;
        private Guid _guid;

        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream {streamName} starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }
        public override async Task OnActivateAsync()
        {
            var primaryKey = this.GetPrimaryKey(out streamName);
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<String>(primaryKey, streamName);

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
            //Console.WriteLine($"Stream {streamName} receives: {message}.");
            //Add your logic here to process received data
            //Get one of the providers which we defined in our config
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<long>(_guid, "Filter");

            var filterGrain = GrainFactory.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.OddNumberFilter");
            //Pick a GUID for a chat room grain and chat room stream


            await stream.OnNextAsync(1);

            
        }
    }
}
