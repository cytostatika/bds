using System;
using Orleans;
using Utilities;
using Orleans.Concurrency;
using Concurrency.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Concurrency
{
    [Reentrant]
    public class Coordinator : Grain, ICoordinator
    {
        private int lastBid;   // last emitted bid
        private int nextTid;
        private int nextBid;
        private int lastCommittedBid;
        private Dictionary<int, List<Tuple<int, List<int>>>> batchedTransactions;    // <bid, List<<tid, actorAccessInfo>>>
        private Dictionary<int, TaskCompletionSource<bool>> waitBatchCommit;   // <bid, batch commit task>
        private Dictionary<int, int> expectedAcksPerBatch;
        private Dictionary<int, int> mapBidToLastBid;  // <bid, lastBid>
        private Dictionary<int, List<int>> actorsPerBatch;
        private Dictionary<int, int> lastBidPerActor;


        /*
         * use this timer to generate epoch-based batches
         * reference: https://dotnet.github.io/orleans/docs/grains/timers_and_reminders.html
         * reference: https://docs.microsoft.com/en-us/dotnet/api/microsoft.servicefabric.actors.runtime.actorbase.registertimer?view=azure-dotnet
        */
        private IDisposable timer;
        private readonly TimeSpan dueTime = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan period = TimeSpan.FromMilliseconds(100);

        public Coordinator()
        {
        }

        public Task CheckGarbageCollection()
        {
            if (batchedTransactions.Count > 0)
                Console.WriteLine($"Coordinator: batchedTransactions has {batchedTransactions.Count} entries");
            if (waitBatchCommit.Count > 0)
                Console.WriteLine($"Coordinator: waitBatchCommit has {waitBatchCommit.Count} entries");
            if (expectedAcksPerBatch.Count > 0)
                Console.WriteLine($"Coordinator: expectedAcksPerBatch has {expectedAcksPerBatch.Count} entries");
            if (mapBidToLastBid.Count > 0)
                Console.WriteLine($"Coordinator: mapBidToLastBid has {mapBidToLastBid.Count} entries");
            if (actorsPerBatch.Count > 0)
                Console.WriteLine($"Coordinator: actorsPerBatch has {actorsPerBatch.Count} entries");

            return Task.CompletedTask;
        }

        public Task Init()
        {
            lastBid = -1;
            nextTid = 0;
            nextBid = 0;
            lastCommittedBid = -1;
            batchedTransactions = new Dictionary<int, List<Tuple<int, List<int>>>>();
            expectedAcksPerBatch = new Dictionary<int, int>();
            waitBatchCommit = new Dictionary<int, TaskCompletionSource<bool>>();
            actorsPerBatch = new Dictionary<int, List<int>>();
            mapBidToLastBid = new Dictionary<int, int>();
            timer = RegisterTimer(GenerateBatch, null, dueTime, period);     // the method "GenerateBatch" will be called periodically
            lastBidPerActor = new Dictionary<int, int>();
            return Task.CompletedTask;
        }

        public async Task<TransactionContext> NewTransaction(List<int> actorAccessInfo)
        {
            Console.WriteLine("Coord: Starting new transaction process");
            // STEP 1: check if we should start a new batch / if the old batch has been emitted
            if (batchedTransactions.ContainsKey(nextBid) == false)
                batchedTransactions.Add(nextBid, new List<Tuple<int, List<int>>>());

            // STEP 2: generate the TransactionContext
            var context = new TransactionContext(nextBid, nextTid);
            nextTid++;

            // STEP 3: put this transaction in the batch
            batchedTransactions[nextBid].Add(new Tuple<int, List<int>>(context.tid, actorAccessInfo));

            return context;
        }

        private Task GenerateBatch(object _)
        {
            // STEP 1: check if there are any transactions to emit
            if (batchedTransactions.ContainsKey(nextBid) == false) return Task.CompletedTask;

            // STEP 2: increment nextBid so transactions come later will be put into the new batch
            // Assume we use the tid of the first transaction in the batch as bid
            var bid = nextBid;
            var myLastBid = lastBid;
            mapBidToLastBid.Add(bid, myLastBid);
            var transactions = batchedTransactions[bid];
            lastBid = nextBid;
            nextBid += transactions.Count;

            // STEP 3: generate sub-batches for each accessed actor
            var actors = new Dictionary<int, Batch>();   // <actorID, sub-batch>
            foreach (var transaction in transactions)
            {
                var tid = transaction.Item1;
                var actorAccessInfo = transaction.Item2;
                foreach (var actor in actorAccessInfo)
                {
                    if (actors.ContainsKey(actor) == false)
                    {
                        if (lastBidPerActor.ContainsKey(actor) == false)
                            actors.Add(actor, new Batch(bid, -1));
                        else
                            actors.Add(actor, new Batch(bid, lastBidPerActor[actor]));
                        lastBidPerActor[actor] = bid;
                    }
                    actors[actor].AddTransaction(tid);
                }
            }

            // STEP 4: add some entries for this batch
            expectedAcksPerBatch.Add(bid, actors.Count);
            actorsPerBatch.Add(bid, new List<int>(actors.Keys));
            Console.WriteLine($"Coord: Expected actors for {bid} is {expectedAcksPerBatch[bid]}");

            // STEP 5: emit sub-batches to related actors
            foreach (var item in actors)
            {
                var actorID = item.Key;
                var subBatch = item.Value;

                var actor = GrainFactory.GetGrain<ITransactionalActor>(actorID, "Grain.AccountActor");
                _ = actor.ReceiveBatch(subBatch);
            }

            // STEP 6: garbage collection
            batchedTransactions.Remove(bid);
            return Task.CompletedTask;
        }

        public async Task BatchComplete(int bid)
        {
            Console.WriteLine($"Coord: Starting completions process for {bid}");
            // STEP 1: check if all accessed actors have completed the batch
            Debug.Assert(expectedAcksPerBatch.ContainsKey(bid));
            expectedAcksPerBatch[bid]--;
            if (expectedAcksPerBatch[bid] != 0) return;

            // STEP 2: check if its previous batch has committed
            Debug.Assert(mapBidToLastBid.ContainsKey(bid));
            var myLastBid = mapBidToLastBid[bid];
            if (myLastBid > lastCommittedBid)
            {
                // need to wait for previous batch to commit
                if (waitBatchCommit.ContainsKey(myLastBid) == false)
                    waitBatchCommit.Add(myLastBid, new TaskCompletionSource<bool>());
                await waitBatchCommit[myLastBid].Task;
            }
            Console.WriteLine("Coord: Finished waiting for last batch commit");

            // STEP 3: commit the batch
            lastCommittedBid = bid;
            if (waitBatchCommit.ContainsKey(bid))
                waitBatchCommit[bid].SetResult(true);

            // STEP 4: inform all related actors
            Debug.Assert(actorsPerBatch.ContainsKey(bid));
            foreach (var actorID in actorsPerBatch[bid])
            {
                var actor = GrainFactory.GetGrain<ITransactionalActor>(actorID, "Grain.AccountActor");
                _ = actor.BatchCommit(bid);
            }

            Console.WriteLine("Coord: Finished informing all actors of finished commit");

            // STEP 5: garbage collection
            if (waitBatchCommit.ContainsKey(bid))
                waitBatchCommit.Remove(bid);
            expectedAcksPerBatch.Remove(bid);
            mapBidToLastBid.Remove(bid);
            actorsPerBatch.Remove(bid);
        }
    }
}