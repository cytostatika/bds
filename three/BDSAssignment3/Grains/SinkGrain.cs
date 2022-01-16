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
    public abstract class SinkGrain : Grain, ISink
    {
        public abstract Task Process(object e);

        public Task Init()
        {
            Console.WriteLine("SourceGrain of stream Sink starts.");
            Guid.NewGuid();

            return Task.CompletedTask;
        }

        protected Task OnNextMessage(object message, StreamSequenceToken sequenceToken)
        {
            Process(message);
            return Task.CompletedTask;
        }
    }

    public class ConsoleSink : SinkGrain
    {
        public override async Task OnActivateAsync()
        {
            var streamProvider = GetStreamProvider("SMSProvider");
            var stream = streamProvider.GetStream<object>(Constants.StreamGuid, Constants.SinkNameSpace);

            // To resume stream in case of stream deactivation
            var subscriptionHandles = await stream.GetAllSubscriptionHandles();
            if (subscriptionHandles.Count > 0)
                foreach (var subscriptionHandle in subscriptionHandles)
                    await subscriptionHandle.ResumeAsync(OnNextMessage);

            await stream.SubscribeAsync(OnNextMessage);
        }

        public override Task Process(object e)
        {
            if (e is IEnumerable enumerable)
                foreach (var tup in enumerable)
                    Console.WriteLine(tup);
            else
                Console.WriteLine(e);

            return Task.CompletedTask;
        }
    }

    public class FileSink : SinkGrain
    {
        private readonly string _projectPath =
            Directory.GetParent(Environment.CurrentDirectory)?.Parent?.Parent?.Parent?.FullName;

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

        public override Task Process(object e)
        {
            using var sw = File.AppendText(Path.Join(_projectPath, "Client", "Log.txt"));
            if (e is IEnumerable enumerable)
                foreach (var tup in enumerable)
                    sw.WriteLine(tup);
            else
                sw.WriteLine(e);

            return Task.CompletedTask;
        }
    }
}