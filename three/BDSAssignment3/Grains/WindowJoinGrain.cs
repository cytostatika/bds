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

        public abstract IList<string> Apply();

        // TODO: change these to getters/setter or whwatever and change them according to the input in Init.
        //       Also create the entire topology either through chaining of init functions or in source grain by calling Inits with correct input.

        private string inStream1;
        private string inStream2;
        private string outStream;

        public long windowSize;
        public long startTime;
        public Dictionary<string, T> dictStream1;
        public Dictionary<string, T> dictStream2;

        // TODO: remove parameters, its already in state lol
        public async Task Process()
        {
            Console.WriteLine($"Process entered for {startTime}");
            var streamProvider = GetStreamProvider("SMSProvider");
            var window = Apply();
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, outStream);

            await stream.OnNextAsync(window);

        }
        public Task Init(string in1, string in2, string out1, long wdSize)
        {
            Console.WriteLine($"WindowGrain of streams {in1}, {in2}, and output to {out1} starts.");
            inStream1 = in1;
            inStream2 = in2;
            outStream = out1;
            windowSize = wdSize;
            startTime = 0;
            dictStream1 = new Dictionary<string, T>();
            dictStream2 = new Dictionary<string, T>();
            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
            // Hardcoded atm for streamlining the development of the join algorithm
            // Potential fix is to subscribe in another function (like Init perhaps)
            inStream1 = Constants.WindowJoinOneNameSpace;
            inStream2 = Constants.WindowJoinTwoNameSpace;

            var streamProvider = GetStreamProvider("SMSProvider");
            var stream1 = streamProvider.GetStream<T>(Constants.StreamGuid, inStream1);
            await stream1.SubscribeAsync(OnNextMessage1);
            var stream2 = streamProvider.GetStream<T>(Constants.StreamGuid, inStream2);
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

        //TODO: make these abstract and move down to client-side
        public abstract Task OnNextMessage1(T message, StreamSequenceToken sequenceToken);

        public abstract Task OnNextMessage2(T message, StreamSequenceToken sequenceToken);


        public abstract void Purge(long ts, ref Dictionary<string, T> streamdict);



    }



    // Tag and GPS
    public class SimpleWindowJoin : WindowJoinGrain<DataTuple>
    {
        public override IList<string> Apply() 
        {

            Console.WriteLine($"Apply entered for {startTime}");
            var s1 = dictStream1;
            var s2 = dictStream2;
            var matches = s1.Keys.Intersect(s2.Keys);

            var res = new List<string>();
            foreach (var m in matches){
                var photo_id = s1[m].PhotoId.ToString();
                var latitude = s2[m].Lat.ToString();
                var longitude = s2[m].Long.ToString();
                //var tmp = s1[m].ToString() + s2[m].ToString();
                var tmp = $"MergeTuple: { photo_id }, { latitude}, { longitude}";
                //Console.WriteLine($"Apply in Window: {tmp}");
                res.Add(tmp);
            }
            dictStream1.Clear();
            dictStream2.Clear();


            return res;
        }
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
                        }//Maybe break in the else??
                    }
                }
            }
        }
        public override async Task OnNextMessage1(DataTuple message, StreamSequenceToken sequenceToken)
        {
            
            //Console.WriteLine($"OnNextMessage1 in Window: {message}, {message.TimeStamp - startTime}, {startTime}");
            dictStream1[message.UserId.ToString()] = message;
            if (startTime == 0) { 
                startTime = message.TimeStamp;
               
            }
            else if(message.TimeStamp - startTime > windowSize)
            {

                await Process();
                startTime = 0;
            }
            
            //Purge(message.TimeStamp, ref dictStream2);
            
        }
        public override async Task OnNextMessage2(DataTuple message, StreamSequenceToken sequenceToken)
        {
            //Console.WriteLine($"OnNextMessage2 in Window: {message}");

            //Console.WriteLine($"OnNextMessage2 in Window: {message}, {message.TimeStamp - startTime}, {startTime}");
            dictStream2[message.UserId.ToString()] = message;
            //Purge(message.TimeStamp, ref dictStream1);
            if (startTime == 0)
            {
                startTime = message.TimeStamp;

            }
            else if (message.TimeStamp - startTime > windowSize)
            {
                await Process();

                startTime = 0;
            }
        }
    }
}