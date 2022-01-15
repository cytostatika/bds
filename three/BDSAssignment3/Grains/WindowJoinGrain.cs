﻿using Orleans;
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

        public abstract IList<(string, T, long)> Apply();

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.
        private string inStream1= Constants.WindowJoinOneNameSpace;
        private string inStream2= Constants.WindowJoinTwoNameSpace;
        private string outStream { get; set; }

        public long windowSize;
        public long startTime;
        public Dictionary<string, DataTuple> dictStream1;
        public Dictionary<string, DataTuple> dictStream2;

        // TODO: remove parameters, its already in state lol
        public async Task Process()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var window = Apply();
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(window);

        }
        public Task Init(string in1, string in2, string out1, long wdSize)
        {
            outStream = out1;
            windowSize = wdSize;
            startTime = 0;
            dictStream1 = new Dictionary<string, DataTuple>();
            dictStream2 = new Dictionary<string, DataTuple>();
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {

            var streamProvider = GetStreamProvider("SMSProvider");
            var stream1 = streamProvider.GetStream<(string, DataTuple, long)>(Constants.StreamGuid, inStream1);
            await stream1.SubscribeAsync(OnNextMessage1);
            var stream2 = streamProvider.GetStream<(string, DataTuple, long)>(Constants.StreamGuid, inStream2);
            await stream2.SubscribeAsync(OnNextMessage2);
            
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
            }


        }

        public abstract Task OnNextMessage1((string, DataTuple, long) message, StreamSequenceToken sequenceToken);

        public abstract Task OnNextMessage2((string, DataTuple, long) message, StreamSequenceToken sequenceToken);


        //public abstract void Purge(long ts, ref Dictionary<string, T> streamdict);



    }



    // Tag as stream 1 and GPS as stream 2 - UserID is key
    public class SimpleWindowJoin : WindowJoinGrain<MergeTuple>
    {
        public override IList<(string, MergeTuple, long)> Apply()
        {
            var s1 = dictStream1;
            var s2 = dictStream2;
            var matches = s1.Keys.Intersect(s2.Keys);

            var res = new List<(string, MergeTuple, long)>();
            foreach (var m in matches)
            {
                var tag = (TagTuple)s1[m];
                var gps = (GpsTuple)s2[m];


                res.Add(("UserId", new MergeTuple(tag, gps), startTime));
            }
            dictStream1.Clear();
            dictStream2.Clear();


            return res;
        }
        /*
        public override void Purge(long ts, ref Dictionary<string, DataTuple> streamdict)
        {
            //Currently it removes for every input ts, not intended behaviour

            if (streamdict.Count != 0)
            {

                if (streamdict.First().Value.TimeStamp < ts - windowSize)
                {
                    foreach (var sd in streamdict)
                    {

                        if (sd.Value.TimeStamp < ts - windowSize)
                        {
                            streamdict.Remove(sd.Key);
                        }
                    }
                }
            }
        }*/
        public override async Task OnNextMessage1((string, DataTuple, long) message, StreamSequenceToken sequenceToken)
        {
            var payload = message.Item2;
            var key = payload.UserId.ToString();
            var ts = message.Item3;
            dictStream1[key] = payload;
            if (startTime == 0)
            {
                startTime = ts;

            }
            else if (ts - startTime > windowSize)
            {

                await Process();
                startTime = 0;
            }

            //Purge(message.TimeStamp, ref dictStream2);

        }
        public override async Task OnNextMessage2((string, DataTuple, long) message, StreamSequenceToken sequenceToken)
        {
            
            
            var payload = message.Item2;
            var key = payload.UserId.ToString();
            var ts = message.Item3;
            dictStream2[key] = payload;
            if (startTime == 0)
            {
                startTime = ts;

            }
            else if (ts - startTime > windowSize)
            {

                await Process();
                startTime = 0;
            }

            //Purge(message.TimeStamp, ref dictStream1);
        }
    }
}