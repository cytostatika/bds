using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FlatMapGrain<T> : Grain, IFlatMap, IFlatMapFunction<T>
    {
        private Guid _sinkGuid;
        public abstract IList<DataTuple> Apply(T e); 
        public async Task Process(object e) // Implements the Process method from IFilter
        {
            var res = Apply((T) e);
            
            var streamProvider = GetStreamProvider("SMSProvider");
            //Get the reference to a stream
            var stream = streamProvider.GetStream<string>(_sinkGuid, "Sink");
        
            await stream.OnNextAsync(e.ToString());
        }
        
        public override async Task OnActivateAsync()
        {
            Console.WriteLine("OnActivateAsync in FlatMap");
            _sinkGuid = Guid.NewGuid();
            
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<DataTuple>(this.GetPrimaryKey(), "FlatMap");

            await stream.SubscribeAsync(OnNextMessage);
        }
        private async Task OnNextMessage(DataTuple message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage in Filter: {message}");
            

            await Process(message);
        }
    }
    
    
    [ImplicitStreamSubscription("FlatMap")]
    public class AddMap : FlatMapGrain<DataTuple>
    {
        public override IList<DataTuple> Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            var res = new List<DataTuple>() {e, e};

            foreach (var dataTuple in res)
            {
                dataTuple.Lat += 10;
            }
            return res;
        }
    }
}