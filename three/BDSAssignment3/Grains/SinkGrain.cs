using System;
using System.Threading.Tasks;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    [ImplicitStreamSubscription("Sink")]
    public class SinkGrain : Grain, ISink
    {
        public Task Process(object e)
        {
            Console.WriteLine($"Processed in Sink: {e}");

            return Task.CompletedTask;
        }
        
        public override async Task OnActivateAsync()
        {
            Console.WriteLine("OnActivateAsync in Sink");

            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(this.GetPrimaryKey(), "Sink");
            // To resume stream in case of stream deactivation
            // var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            //
            // if (subscriptionHandles.Count > 0)
            // {
            //     foreach (var subscriptionHandle in subscriptionHandles)
            //     {
            //         await subscriptionHandle.ResumeAsync(OnNextMessage);
            //     }
            // }

            await stream.SubscribeAsync(OnNextMessage);
        }
        
        private Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            Process(message);
            return Task.CompletedTask;
        }
    }
    
    
}
