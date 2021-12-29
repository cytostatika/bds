using GrainStreamProcessing.GrainInterfaces;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using System;
using System.Threading.Tasks;
using Client;

namespace GrainStreamProcessing
{
    public class Program
    {
        static int Main(string[] args)
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                using (var client = await ConnectClient())
                {
                    //await SampleClient(client);
                    await StreamClient(client);
                    Console.ReadKey();
                }

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

            // Activate source grains for photo, tag and gps streams by calling Init method, in order to subscribe these streams.
            await photoSource.Init();
            await tagSource.Init();
            await gpsSource.Init();

            // Feeding data to streams
            await DataDriver.Run(photoStream, tagStream, gpsStream, 1600, 0);
        }

        private static async Task SampleClient(IClusterClient client)
        {
            // The code below shows how to specify an exact grain class which implements the IFilter interface

            Random random = new Random();
            //var filterGrain = client.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.LargerThanTenFilter");
            var filterGrain = client.GetGrain<IFilter>(0, "GrainStreamProcessing.GrainImpl.OddNumberFilter");
            for (int i = 0; i < 20; ++i)
            {
               long r = random.Next(20); // Randomly generate twenty numbers between 0 and 19.
                Console.WriteLine(r); // Output these numbers to Client console.
               await filterGrain.Process(r); // Send these numbers to the filter operator, and numbers that pass this filter will be outputted onto Silo console.
            }
        }
    }


}