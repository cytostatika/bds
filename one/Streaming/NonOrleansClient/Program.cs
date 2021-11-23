using System;
using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

using var client = new ClientBuilder()
    .UseLocalhostClustering()
    .ConfigureLogging(logging => logging.AddConsole())
    .AddSimpleMessageStreamProvider("SMSProvider")
    .Build();

await client.Connect();

var streamProvider = client.GetStreamProvider("SMSProvider");

var guid = Guid.NewGuid();
var stream = streamProvider.GetStream<int>(guid, Constants.StreamNamespace);

Console.WriteLine($"Sending event to StreamId: [{guid}, {Constants.StreamNamespace}]");

for (int i = 0; i < 30; i++)
{
    Console.WriteLine($"Sending '{i+1}'");
    await stream.OnNextAsync(i + 1);
    await Task.Delay(TimeSpan.FromSeconds(1));
}

Console.WriteLine("Done!");