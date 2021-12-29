using System;
using Orleans;
using Utilities;
using System.Diagnostics;
using Orleans.Concurrency;
using Concurrency.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace Concurrency
{
    [Reentrant]
    public abstract class TransactionalActor<TState> : Grain, ITransactionalActor where TState: new()
    {
        public int myActorID;
        protected TState actorState;
        private ICoordinator coordinator;
        private int lastCommittedBid;
        private int lastExecutedTid;
        private Dictionary<int, TaskCompletionSource<bool>> waitBatchCommit;
        private Dictionary<int, TaskCompletionSource<bool>> waitBatchMsg;
        private Dictionary<int, Batch> batches;
        private Dictionary<int, (Dictionary<int, (TaskCompletionSource<bool>, int)>, int)> localSchedule; // <bid, (Dict(tid->(task, lasttid)), lastbid)>
        private Dictionary<int, TaskCompletionSource<bool>> lastBidWaitMsg;
        // General idea is to have (id, previd) pairs, whenever previd batch finishes we remove it to hav e(id, noid) pair in stead. Meaning that this actor can run it.


        public TransactionalActor()
        {
        }

        /*
         * This method is automatically called by Orleans runtime when this actor is accessed for the first time.
         * This method is used to initialize the actor
         */
        public override Task OnActivateAsync()
        {
            lastCommittedBid = -1;
            lastExecutedTid = -1;
            myActorID = (int)this.GetPrimaryKeyLong();
            actorState = new TState();

            // Assume that coordinator's actor ID is 0 and there is only one coordinator in the whole system
            coordinator = GrainFactory.GetGrain<ICoordinator>(0);
            waitBatchCommit = new Dictionary<int, TaskCompletionSource<bool>>();
            batches = new Dictionary<int, Batch>();
            waitBatchMsg = new Dictionary<int, TaskCompletionSource<bool>>(); 
            localSchedule = new Dictionary<int, (Dictionary<int, (TaskCompletionSource<bool>, int)>,int)>();
            lastBidWaitMsg = new Dictionary<int, TaskCompletionSource<bool>>();

            return base.OnActivateAsync();
        }

        public Task CheckGarbageCollection()
        {
            if (waitBatchCommit.Count > 0)
                Console.WriteLine($"Actor {myActorID}: waitBatchCommit has {waitBatchCommit.Count} entries");
            if (waitBatchMsg.Count > 0)
                Console.WriteLine($"Actor {myActorID}: waitBatchMsg has {waitBatchMsg.Count} entries");
            if (batches.Count > 0)
                Console.WriteLine($"Actor {myActorID}: batches has {batches.Count} entries");
            if (localSchedule.Count > 0)
                Console.WriteLine($"Actor {myActorID}: local schedule has {localSchedule.Count} entries");
            if (lastBidWaitMsg.Count > 0)
                Console.WriteLine($"Actor {myActorID}: last bid queue has {lastBidWaitMsg.Count} entries");
            /*
             * ATTENTION!! Add metadata you used here to check if it's properly garbage collected
             */

            return Task.CompletedTask;
        }

        public async Task<object> SubmitTransaction(string funcName, object funcInput, List<int> actorAccessInfo)
        {
            // STEP 1: get the transaction context from the coordinator
            var context = await coordinator.NewTransaction(actorAccessInfo);
            
            // STEP 2: invoke the first function call on this actor
            var call = new FunctionCall(funcName, funcInput, GetType());
            var transactionResult = await Execute(call, context);
            Console.WriteLine($"Actor {myActorID}: has returned result of execution");

            // STEP 3: add an entry to waitBatchCommit if it hasn't been added
            if (waitBatchCommit.ContainsKey(context.bid) == false)
                waitBatchCommit.Add(context.bid, new TaskCompletionSource<bool>());


            // STEP 4: wait for the transaction to be committed
            await waitBatchCommit[context.bid].Task;

            Console.WriteLine($"Actor {myActorID}: has finished committing");


            // STEP 5: return the result to the client after the transaction is committed
            return transactionResult;
        }

        public async Task ReceiveBatch(Batch batch)
        {
            Console.WriteLine($"Actor {myActorID}: received batch with id {batch.bid} and last bid {batch.lastBid}");
            // STEP 1: store the received batch
            batches.Add(batch.bid, batch);
            if (waitBatchMsg.ContainsKey(batch.bid))
                waitBatchMsg[batch.bid].SetResult(true);

            // STEP 2: add an entry to waitBatchCommit if it hasn't been added
            if (waitBatchCommit.ContainsKey(batch.bid) == false)
                waitBatchCommit.Add(batch.bid, new TaskCompletionSource<bool>());

            // Step 3: assuming only one ReceiveBatch is called
            var transactions =  new Dictionary<int, (TaskCompletionSource<bool>, int)>();
            var lasttid = -1;
            for (int i = 0; i<batch.transactionList.Count; i++) {
                if (i>0) 
                    lasttid = batch.transactionList[i-1];
                Console.WriteLine($"Actor {myActorID}: registering task {batch.transactionList[i]}");
                transactions.Add(batch.transactionList[i], (new TaskCompletionSource<bool>(), lasttid));
            }
            localSchedule.Add(batch.bid, (transactions, batch.lastBid));
            
            lastBidWaitMsg.Add(batch.bid, new TaskCompletionSource<bool>());
            //Check if this batches prevID is in local schedule. If it is then add its (id, previd) pair to the schedule. If not add (id, noid) to schedule

            /*
             * ATTENTION!! Add code here to maintain your local schedule
             */
            await Task.CompletedTask;
        }

        public async Task<object> Execute(FunctionCall call, TransactionContext context)
        {
            //Debug.Assert(localSchedule.ContainsKey(context.bid));
            Console.WriteLine($"Actor {myActorID}: starting execution of batch {context.bid} transaction {context.tid}");

            // STEP 0: wait until the sub-batch has arrived
            if (batches.ContainsKey(context.bid) == false)
            {
                if (waitBatchMsg.ContainsKey(context.bid) == false) 
                    waitBatchMsg.Add(context.bid, new TaskCompletionSource<bool>()); 
                await waitBatchMsg[context.bid].Task;
            }   
            Debug.Assert(batches.ContainsKey(context.bid));
     
           

            //Console.WriteLine($"Actor { myActorID}: continuing execution of batch {context.bid}");
            // STEP 1: block the call until it is its turn to execute
            /*
             * ATTENTION!! It is your task to enforce the actor to execute this call at a proper time
             * 
             * Hint: all transactional calls must be executed in the order determined in the local schedule
             * Hint: use TaskCompletiionSource to asynchronously wait for a task that will be fulfilled by the completion of another transactional call
             */
            
            //if (context.bid > 0){
            if (localSchedule.ContainsKey(context.bid))
                {
                var lastbid = localSchedule[context.bid].Item2;
                Console.WriteLine("Waiting");
                await lastBidWaitMsg[lastbid].Task;
                Console.WriteLine("Finished waiting");
                //lastBidWaitMsg.Remove(lastbid);
            }

            // Checking if it exists to allow for "init" 
            if(localSchedule.ContainsKey(context.bid)) {
                // If not the first transaction, wait for previous to complete
                if (localSchedule[context.bid].Item1.ContainsKey(context.tid))
                {
                    var lasttid = localSchedule[context.bid].Item1[context.tid].Item2;
                    if(lasttid != -1)
                        await localSchedule[context.bid].Item1[lasttid].Item1.Task;
                }
                else
                {
                    throw new Exception("Transactions not registered");
                }
            }

            // STEP 2: execute the call
            var method = call.actorType.GetMethod(call.funcName);
            var result = await (Task<object>)method.Invoke(this, new object[] { context, call.funcInput });
            Console.WriteLine($"Actor { myActorID}: finished execution of batch {context.bid} transaction {context.tid}");

            // Checking if it exists to allow for "init" 
            if (localSchedule.ContainsKey(context.bid))
            {
                localSchedule[context.bid].Item1[context.tid].Item1.SetResult(true);
            }
            // Set current transaction to finished


            // STEP 3: check if the whole batch has been completed on this actor
            /*
             * ATTENTION!! you should implement this part
             * 
             * 
             * if (...)
             * {
             *     // tell the coordinator that this actor has completed the batch
             *     _ = coordinator.BatchComplete(context.bid);
             *  +
             */
            //Console.WriteLine($"Actor { myActorID}: transaction {context.tid} max is {localSchedule[context.bid].Item1.Keys.Max()}");
            // Checking if it exists to allow for "init" 
            if (localSchedule.ContainsKey(context.bid))
            {
                Console.WriteLine($"Actor { myActorID}: transaction {context.tid} max is {localSchedule[context.bid].Item1.Keys.Max()}");
                // If last transaction in batch
                if (context.tid == localSchedule[context.bid].Item1.Keys.Max())
                {
                    Console.WriteLine($"Actor { myActorID}: transaction {context.tid} max is {localSchedule[context.bid].Item1.Keys.Max()}");
                    _ = coordinator.BatchComplete(context.bid);
                }
            }
            else
            {
                _ = coordinator.BatchComplete(context.bid);
            }
            // STEP 4: return the result
            return result;
        }

        public async Task BatchCommit(int bid)
        {

            Console.WriteLine($"Actor { myActorID}: Starting completions process");
            // STEP 0: update lastCommittedBid
            lastCommittedBid = Math.Max(lastCommittedBid, bid);
            
            // STEP 1: set the corresponding waitBatchCommit task as fulfilled
            Debug.Assert(waitBatchCommit.ContainsKey(bid) == true);
            waitBatchCommit[bid].SetResult(true);

            // STEP 2: garbage collection
            /*
             * ATTENTION!! It is your task to garbage collect all metadata that you use to maintain local schedule
             */

            if (lastBidWaitMsg.ContainsKey(bid)) {
                lastBidWaitMsg[bid].SetResult(true);
            } 
            else
            {
                lastBidWaitMsg.Add(bid, new TaskCompletionSource<bool>());

                lastBidWaitMsg[bid].SetResult(true);
            }
            //lastBidWaitMsg[bid].SetResult(true);
            //localSchedule.Remove(bid);


            waitBatchCommit.Remove(bid);
            if (waitBatchMsg.ContainsKey(bid)) waitBatchMsg.Remove(bid);
            batches.Remove(bid);
        }
    }
}