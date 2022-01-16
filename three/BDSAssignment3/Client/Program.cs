using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client;
using GrainStreamProcessing.Functions;
using GrainStreamProcessing.GrainInterfaces;
using GrainStreamProcessing.Model;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;

namespace GrainStreamProcessing
{
    public class Program
    {
        private static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using var client = await ConnectClient();
                //await FilterClient(client);
                //await FlatMapClient(client);
                //await AggregateClient(client);
                //await JoinClient(client);
                await StreamClient(client);
                Console.ReadKey();

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine($"\nException while trying to run client: {e.Message}");
                Console.WriteLine("Make sure the silo the client is trying to connect to is running.");
                Console.WriteLine("\nPress any key to exit.");
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> ConnectClient()
        {
            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "cluster";
                    options.ServiceId = "GrainStreamProcessing";
                })
                .ConfigureApplicationParts(parts => parts
                    .AddApplicationPart(typeof(ISource).Assembly).WithReferences()
                )
                .ConfigureLogging(logging => logging.AddConsole())
                .AddSimpleMessageStreamProvider("SMSProvider")
                .Build();

            await client.Connect();
            Console.WriteLine("Client successfully connected to silo host \n");
            return client;
        }


        private static async Task StreamClient(IClusterClient client)
        {
            // Get photo, tag and gps streams
            var streamProvider = client.GetStreamProvider("SMSProvider");
            var guid = new Guid();
            var photoStream = streamProvider.GetStream<string>(guid, "Photo");
            var tagStream = streamProvider.GetStream<string>(guid, "Tag");
            var gpsStream = streamProvider.GetStream<string>(guid, "GPS");

            var photoSource = client.GetGrain<ISource>(guid, "Photo");
            var tagSource = client.GetGrain<ISource>(guid, "Tag");
            var gpsSource = client.GetGrain<ISource>(guid, "GPS");

            var filterGrain = client.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.OddNumberFilter");
            var flatMapGrain = client.GetGrain<IFlatMap>(0, "GrainStreamProcessing.GrainImpl.AddMap");
            var aggregateGrain = client.GetGrain<IAggregate>(0, "GrainStreamProcessing.GrainImpl.AverageLongitudeAggregate");
            var joinGrain = client.GetGrain<IWindowJoin>(0, "GrainStreamProcessing.GrainImpl.SimpleWindowJoin");
            var sink = client.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.FileSink");

            // Activate source grains for sink, photo, tag and gps streams by calling Init method, in order to subscribe these streams.
            await photoSource.Init(Constants.FlatMapNameSpace);
            await tagSource.Init(Constants.FlatMapNameSpace);
            await gpsSource.Init(Constants.WindowJoinTwoNameSpace);
            await aggregateGrain.Init(Constants.FilterNameSpace);


            await filterGrain.Init(Constants.WindowJoinOneNameSpace);
            await joinGrain.Init(Constants.WindowJoinOneNameSpace, Constants.WindowJoinTwoNameSpace,Constants.AggregateNameSpace, 2000);
            await flatMapGrain.Init(Constants.SinkNameSpace);

            await sink.Init();
            // Feeding data to streams
            await DataDriver.Run(photoStream, tagStream, gpsStream, 1600, 0);
        }

        private static async Task FilterClient(IClusterClient client)
        {
            // The code below shows how to specify an exact grain class which implements the IFilter interface

            var random = new Random();
            //var filterGrain = client.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.LargerThanTenFilter");
            var filterGrain = client.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.OddNumberFilter");
            var sinkGrain = client.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.ConsoleSink");

            await filterGrain.Init(Constants.SinkNameSpace);
            await sinkGrain.Init();
            for (var i = 0; i < 20; ++i)
            {
                var res = CreateTestTuple(random, 0);
                Console.WriteLine(res); // Output these numbers to Client console.
                await filterGrain
                    .Process(res); // Send these numbers to the filter operator, and numbers that pass this filter will be outputted onto Silo console.
            }
        }

        private static async Task FlatMapClient(IClusterClient client)
        {
            // The code below shows how to specify an exact grain class which implements the IFilter interface

            var random = new Random();
            var flatMapGrain = client.GetGrain<IFlatMap>(0, "GrainStreamProcessing.GrainImpl.AddMap");
            var sinkGrain = client.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.ConsoleSink");
            await flatMapGrain.Init(Constants.SinkNameSpace);
            await sinkGrain.Init();

            for (var i = 0; i < 20; ++i)
            {
                var res = CreateTestTuple(random, 0);
                Console.WriteLine(res); // Output these numbers to Client console.
                await flatMapGrain
                    .Process(res); // Send these numbers to the filter operator, and numbers that pass this filter will be outputted onto Silo console.
            }
        }

        private static async Task AggregateClient(IClusterClient client)
        {
            // The code below shows how to specify an exact grain class which implements the IFilter interface
            var random = new Random();
            var aggregateGrain =
                client.GetGrain<IAggregate>(0, "GrainStreamProcessing.GrainImpl.AverageLongitudeAggregate");
            var sinkGrain = client.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.ConsoleSink");
            await aggregateGrain.Init(Constants.SinkNameSpace);
            await sinkGrain.Init();
            for (var i = 0; i < 20; ++i)
            {
                var res = CreateTestTuple(random, 0);
                res.Item2.UserId[0] = 10;
                Console.WriteLine(res); // Output these numbers to Client console.
                await aggregateGrain
                    .Process(res); // Send these numbers to the aggregate operator
            }
        }

        private static async Task JoinClient(IClusterClient client)
        {
            var random = new Random();
            var joinGrain = client.GetGrain<IWindowJoin>(0, "GrainStreamProcessing.GrainImpl.SimpleWindowJoin");
            var sinkGrain = client.GetGrain<ISink>(0, "GrainStreamProcessing.GrainImpl.ConsoleSink");

            await joinGrain.Init(Constants.WindowJoinOneNameSpace, Constants.WindowJoinTwoNameSpace,
                Constants.SinkNameSpace, 2000);
            await sinkGrain.Init();
            for (var i = 0; i < 20; ++i)
            {
                var res = CreateTestTuple(random, 0);

                Console.WriteLine(res); // Output these numbers to Client console.

                await joinGrain.OnNextMessage1(res, null);
                await joinGrain.OnNextMessage2(res, null);

                await Task.Delay(1000);
            }
        }

        private static (string, DataTuple, long) CreateTestTuple(Random rand, int randSpan)
        {
            var r = rand.Next(20).ToString(); // Randomly generate twenty numbers between 0 and 19.
            var ts = DataDriver.getCurrentTimestamp() + rand.Next(2 * randSpan + 1) - randSpan;
            return ("UserId", new PhotoTuple(new List<string> {r, r, r, r}), ts);
        }
    }
}