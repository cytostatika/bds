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
        public Task Init()
        {
            Console.WriteLine($"SourceGrain of stream {streamName} starts.");
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

        private Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"Stream {streamName} receives: {message}.");
            //Add your logic here to process received data






            return Task.CompletedTask;
        }
    }
}
