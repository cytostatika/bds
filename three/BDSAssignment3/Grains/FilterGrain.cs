using Orleans;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using System;
using System.Linq;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {
        private Guid _sinkGuid;
        public abstract bool Apply(T e); 
        public async Task Process(object e) // Implements the Process method from IFilter
        {
            // if (Apply((T)e)) // If the function returns true, send the element to SinkGrain
            //     {
            //         await this.GrainFactory.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.SinkGrain").Process(e);
            //     } // Otherwise, skip it
            //
            if (Apply((T) e))
            {
                var streamProvider = GetStreamProvider("SMSProvider");
                //Get the reference to a stream
                var stream = streamProvider.GetStream<string>(_sinkGuid, "Sink");
            
                await stream.OnNextAsync(e.ToString());
            }
        }

        public override async Task OnActivateAsync()
        {
            Console.WriteLine("OnActivateAsync in Filter");
            _sinkGuid = Guid.NewGuid();
            
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<string>(this.GetPrimaryKey(), "Filter");
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
        private async Task OnNextMessage(string message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage in Filter: {message}");

            await Process(long.Parse(message));
        }
    }
    
    
    public class LargerThanTenFilter : FilterGrain<long>
    {
        public override bool Apply(long e) // Implements the Apply method, filtering numbers larger than 10
        {
             if (e > 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
    }
    
    [ImplicitStreamSubscription("Filter")]
    public class OddNumberFilter : FilterGrain<long>
    {
        public override bool Apply(long e) // Implements the Apply method, filtering odd numbers
        {
            if (e % 2 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
