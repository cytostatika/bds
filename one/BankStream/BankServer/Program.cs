using Common;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;

await Host.CreateDefaultBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("PubSubStore")
            .AddSimpleMessageStreamProvider(Constants.StreamProvider);
    })
    .RunConsoleAsync();