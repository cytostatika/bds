using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace GrainStreamProcessing.GrainImpl
{
    public class SinkGrain : Grain, ISink
    {
        readonly string _projectPath = Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;

        public Task Process(object e)
        {
            
            using StreamWriter sw = File.AppendText(Path.Join(_projectPath,"Client","Log.txt"));
            if (e is IEnumerable enumerable)
                foreach (var tup in enumerable)
                    sw.WriteLine($"Processed in Sink: {tup}");
            else
                sw.WriteLine($"Processed in Sink: {e}");

            return Task.CompletedTask;
        }

        public Task Init()
        {
            Console.WriteLine("SourceGrain of stream Filter starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }

        public override async Task OnActivateAsync()
        {
             File.Create(Path.Join(_projectPath, "Client", "Log.txt")).Close();
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, Constants.SinkNameSpace);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        private Task OnNextMessage(object message, StreamSequenceToken sequenceToken)
        {
            Process(message);
            return Task.CompletedTask;
        }
    }
}