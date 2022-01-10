using Orleans;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Orleans.Streams;
using System.Collections.Generic;

namespace GrainStreamProcessing.GrainImpl
{
    public abstract class WindowJoinGrain<T> : Grain, IWindowJoin, IWindowJoinFunction<T>
    {
       // private IStreamProvider streamProvider;

        public abstract IList<string> Apply(T e);

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.

        private string inStream1;
        private string inStream2;
        private string outStream;

        public async Task Process(object e)
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var window = Apply((T)e);
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(window);

        }
        public Task Init(string in1, string in2, string out1)
        {
            Console.WriteLine($"WindowGrain of streams {in1}, {in2}, and output to {out1} starts.");
            inStream1 = in1;
            inStream2 = in2;
            outStream = out1;

            return Task.CompletedTask;
        }
                
        public override async Task OnActivateAsync()
        {
            // Hardcoded atm for streamlining the development of the join algorithm
            // Potential fix is to subscribe in another function (like Init perhaps)
            inStream1 = Constants.WindowJoinOneNameSpace;
            inStream2 = Constants.WindowJoinTwoNameSpace;

            var streamProvider = GetStreamProvider("SMSProvider");
            var stream1 = streamProvider.GetStream<DataTuple>(Constants.StreamGuid, inStream1);
            await stream1.SubscribeAsync(OnNextMessage1);
            var stream2 = streamProvider.GetStream<DataTuple>(Constants.StreamGuid, inStream2);
            await stream2.SubscribeAsync(OnNextMessage2);
            /*
            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream1.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles)
                {
                    await subscriptionHandle.ResumeAsync(OnNextMessage1);
                }
            }
            var subscriptionHandles2 = await stream2.GetAllSubscriptionHandles();
            if (subscriptionHandles2.Count > 0)
            {
                foreach (var subscriptionHandle in subscriptionHandles2)
                {
                    await subscriptionHandle.ResumeAsync(OnNextMessage2);
                }
            }*/


        }
        private async Task OnNextMessage1(DataTuple message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage1 in Window: {message}");
            

            await Process(message);
        }
        private async Task OnNextMessage2(DataTuple message, StreamSequenceToken sequenceToken)
        {
            Console.WriteLine($"OnNextMessage2 in Window: {message}");


            await Process(message);
        }

    }


    public class SimpleWindowJoin : WindowJoinGrain<DataTuple>
    {
        public override IList<string> Apply(DataTuple e) // Implements the Apply method, filtering odd numbers
        {
            var res = new List<string> { e.ToString() };
            return res;
        }
    }
}
