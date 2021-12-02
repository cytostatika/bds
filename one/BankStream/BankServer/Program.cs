using Common;
using Grains;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Microsoft.Extensions.Hosting;
using Grains;

await Host.CreateDefaultBuilder()
    .UseOrleans(siloBuilder =>
    {
        siloBuilder
            .UseLocalhostClustering()
            .AddMemoryGrainStorage("PubSubStore")
            .AddSimpleMessageStreamProvider(Constants.StreamProvider);
    })
    .RunConsoleAsync();