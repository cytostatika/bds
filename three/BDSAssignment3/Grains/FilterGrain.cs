﻿using System;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class FilterGrain<T> : Grain, IFilter, IFilterFunction<T>
    {
        private IStreamProvider streamProvider;
        private string MyInStream { get; } = Constants.FilterNameSpace;
        private string MyOutStream { get; set; }

        public async Task Process(object e) // Implements the Process method from IFilter
        {
            if (Apply(((string, T, long)) e))
            {
                //Get the reference to a stream
                var outStream = MyOutStream;
                var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

                await stream.OnNextAsync(e);
            }
        }

        public Task Init(string nextStream)
        {
            Console.WriteLine("SourceGrain of stream FlatMap starts.");
            Guid.NewGuid();
            MyOutStream = nextStream;
            return Task.CompletedTask;
        }

        public abstract bool Apply((string, T, long) e);

        public override async Task OnActivateAsync()
        {
            streamProvider = GetStreamProvider("SMSProvider");
            var inStream = MyInStream;
            var stream = streamProvider.GetStream<(string, T, long)>(Constants.StreamGuid, inStream);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private async Task OnNextMessage((string, T, long) message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage in FlatMap: {message}");

            await Process(message);
        }
    }

    public class OddNumberFilter : FilterGrain<DataTuple>
    {
        public override bool Apply((string, DataTuple, long) e) // Implements the Apply method, filtering odd numbers
        {
            if (e.Item2.UserId % 2 == 1)
                return true;
            return false;
        }
    }
}