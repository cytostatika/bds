using System;
using System.Threading.Tasks;
using Orleans;
using Orleans.Hosting;
using Orleans.Configuration;
using Microsoft.Extensions.Logging;
using Utilities;

namespace Silo
{
    class Program
    {
        static int Main()
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            try
            {
                var host = await StartSilo();
                Console.WriteLine("Press Enter to terminate...");
                Console.ReadLine();

                await host.StopAsync();

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
        }

        private static async Task<ISiloHost> StartSilo()
        {
            var builder = new SiloHostBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = Constants.ClusterId;
                    options.ServiceId = Constants.ServiceId;
                })
                .ConfigureLogging(logging => logging.AddConsole().AddFilter("Orleans", LogLevel.Information));

            var host = builder.Build();
            await host.StartAsync();
            return host;
        }
    }
}