using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public class SinkGrain : Grain, ISink
    {
        public Task Process(object e)
        {
            if  (e is IEnumerable enumerable)
            {
                foreach (var tup in enumerable)
                {
                    Console.WriteLine($"Processed in Sink as enumerable: {tup}");
                }
            }
            else
            {
                Console.WriteLine($"Processed in Sink: {e}");
            }

            return Task.CompletedTask;
        }
        
        public Task Init()
        {
            Console.WriteLine($"Sink starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }
        
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid,Constants.SinkNameSpace);

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
        
        private Task OnNextMessage(object message, StreamSequenceToken sequenceToken)
        {
            Process(message);
            return Task.CompletedTask;
        }
    }
    
    
}
