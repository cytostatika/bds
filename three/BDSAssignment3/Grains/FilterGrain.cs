using Orleans;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {

        
        private IStreamProvider streamProvider;
        private Guid _outGuid;
        public abstract bool Apply(T e);
        public abstract string MyInStream();
        public abstract string MyOutStream();
        public async Task Process(object e) // Implements the Process method from IFilter
        { 
            if (Apply((T) e))
            {
                //Get the reference to a stream
                var outStream = MyOutStream();
                var stream = streamProvider.GetStream<string>(_outGuid, outStream);
            
                await stream.OnNextAsync(e.ToString());
            }
        }

        public override async Task OnActivateAsync()
        {
            Console.WriteLine("OnActivateAsync in Filter");
            _outGuid = Guid.NewGuid();
            
            streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream();
            var stream = streamProvider.GetStream<DataTuple>(this.GetPrimaryKey(), inStream);
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
        private async Task OnNextMessage(DataTuple message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage in Filter: {message}");
            

            await Process(message);
        }
    }
    
    
    public class LargerThanTenFilter : FilterGrain<DataTuple>
    {
        public override bool Apply(DataTuple e) // Implements the Apply method, filtering numbers larger than 10
        {
             if (e.UserId > 10)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        public override string MyInStream()
        {
            return "Filter";
        }

        public override string MyOutStream()
        {
            return "Sink";
        }
    }
    
    [ImplicitStreamSubscription("Filter")]
    public class OddNumberFilter : FilterGrain<DataTuple>
    {
        public override bool Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            if (e.UserId % 2 == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public override string MyInStream()
        {
            return "Filter";
        }

        public override string MyOutStream()
        {
            return "Sink";
        }
    }
}
