using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Concurrency.Interface;
using Grain.Interface;
using Orleans;
using Utilities;
using MathNet.Numerics.Statistics;
using MathNet.Numerics.Distributions;

namespace Client
{
    class Program
    {
        static int numAccountActor = 4;
        static int numClient = 8;
        static IClusterClient[] clients;
        static Thread[] threads;
        static CountdownEvent threadACKs;
        static int elapsedTimeInMilliSec = 10000;   // run clients for 10s

        static int Main()
        {
            return RunMainAsync().Result;
        }

        private static async Task<int> RunMainAsync()
        {
            // STEP 1: initialize clients
            clients = new IClusterClient[numClient];
            for (int i = 0; i < numClient; i++)
            {
                var config = new ClientConfiguration();
                clients[i] = await config.StartClientWithRetries(i);
            }
             
            // STEP 2: initialize the coordinator
            var coordinator = clients[0].GetGrain<ICoordinator>(0);
            await coordinator.Init();

            // STEP 3: initialize all AccountActors
            var tasks = new List<Task>();
            for (int i = 0; i < numAccountActor; i++)
            {
                var actor = clients[0].GetGrain<IAccountActor>(i);
                var actorAccessInfo = new List<int>();
                actorAccessInfo.Add(i);
                tasks.Add(actor.SubmitTransaction("Init", i, actorAccessInfo));
            }
            await Task.WhenAll(tasks);
            
            // STEP 4: spawn threads to generate transaction requests
            threads = new Thread[numClient];
            threadACKs = new CountdownEvent(numClient);
            for (int i = 0; i < numClient; i++)
            {
                threads[i] = new Thread(ThreadWorkAsync);
                threads[i].Start(i);
            }
            
            // STEP 5: wait for all threads to finish
            threadACKs.Wait();

            // STEP 6: check garbage collection
            Console.WriteLine($"Check GC...");
            tasks = new List<Task>();
            tasks.Add(coordinator.CheckGarbageCollection());
            for (int i = 0; i < numAccountActor; i++)
            {
                var actor = clients[0].GetGrain<IAccountActor>(i);
                tasks.Add(actor.CheckGarbageCollection());
            }
            await Task.WhenAll(tasks);
            Console.WriteLine("Finish checking GC");

            // STEP 7: check all accounts' balance
            var checkBalance = new List<Task<object>>();
            for (int i = 0; i < numAccountActor; i++)
            {
                var actor = clients[0].GetGrain<IAccountActor>(i);
                var actorAccessInfo = new List<int>();
                actorAccessInfo.Add(i);
                checkBalance.Add(actor.SubmitTransaction("GetBalance", null, actorAccessInfo));
            }
            await Task.WhenAll(checkBalance);

            var totalBalance = 0;
            foreach (var t in checkBalance)
            {
                var balance = (int)t.Result;
                // each account's balance should be non-negative
                Debug.Assert(balance >= 0);
                totalBalance += balance;
            }
            // the total amount of money should remain the same
            Debug.Assert(totalBalance == Constants.InitialBalance * numAccountActor);
            
            Console.WriteLine("Finish....");
            return 0;
        }

        private static async void ThreadWorkAsync(object obj)
        {
            // each thread uses a different connection to silo
            var threadID = (int)obj;
            var client = clients[threadID];
            var numTransaction = 0;
            var accountDistribution = new DiscreteUniform(0, numAccountActor - 1, new Random());
            var moneyDistribution = new DiscreteUniform(1, 10, new Random());
            Console.WriteLine($"Thread {threadID} starts...");
            var globalWatch = new Stopwatch();
            globalWatch.Restart();
            var startTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            while (globalWatch.ElapsedMilliseconds < elapsedTimeInMilliSec)
            {
                // generate a transaction
                var fromAccount = accountDistribution.Sample();
                var toAccount = accountDistribution.Sample();
                while(toAccount == fromAccount)
                    toAccount = accountDistribution.Sample();

                var money = moneyDistribution.Sample();
                var funcInput = new Tuple<int, int>(money, toAccount);

                var actorAccessInfo = new List<int>();
                actorAccessInfo.Add(fromAccount);
                actorAccessInfo.Add(toAccount);

                // send a new request when previous one has done
                var fromAccountActor = client.GetGrain<IAccountActor>(fromAccount);
                await fromAccountActor.SubmitTransaction("Transfer", funcInput, actorAccessInfo);
                numTransaction++;
            }
            var endTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            globalWatch.Stop();
            var time = endTime - startTime;   // in milliseconds
            Console.WriteLine($"Thread {threadID} elapsed {time}ms");
            var throughput = numTransaction * 1000.0 / time;
            Console.WriteLine($"Thread {threadID} tp = {throughput}");
            threadACKs.Signal();
        }
    }
}