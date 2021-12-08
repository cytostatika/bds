using System;
using Orleans;
using Orleans.Runtime;
using Orleans.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Orleans.Configuration;
using Utilities;

namespace Client
{
    public class ClientConfiguration
    {
        private IClusterClient client;
        private static readonly int maxAttempts = 10;

        public async Task<IClusterClient> StartClientWithRetries(int clientID)
        {
            if (client == null)
            {
                int attempt = 0;
                while (true)
                {
                    try
                    {
                        client = new ClientBuilder()
                                    .UseLocalhostClustering()
                                    .Configure<ClusterOptions>(options =>
                                    {
                                        options.ClusterId = Constants.ClusterId;
                                        options.ServiceId = Constants.ServiceId;
                                    })
                                    .Build();

                        await client.Connect();
                        Console.WriteLine($"Client {clientID} successfully connect to silo host");
                        break;
                    }
                    catch (SiloUnavailableException)
                    {
                        attempt++;
                        Console.WriteLine($"Attempt {attempt} of {maxAttempts} failed to initialize the Orleans client.");
                        if (attempt > maxAttempts)
                        {
                            throw;
                        }
                        await Task.Delay(TimeSpan.FromSeconds(4));
                    }
                }
            }
            return client;
        }
    }
}