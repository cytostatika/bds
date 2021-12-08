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
    public abstract class TransactionalActor<TState> : Grain, ITransactionalActor
    {
        private int myActorID;
        protected TState actorState;
        private ICoordinator coordinator;
        private int lastCommittedBid;
        private Dictionary<int, TaskCompletionSource<bool>> waitBatchCommit;
        private Dictionary<int, TaskCompletionSource<bool>> waitBatchMsg;
        private Dictionary<int, Batch> batches;

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
            myActorID = (int)this.GetPrimaryKeyLong();

            // Assume that coordinator's actor ID is 0 and there is only one coordinator in the whole system
            coordinator = GrainFactory.GetGrain<ICoordinator>(0);
            waitBatchCommit = new Dictionary<int, System.Threading.Tasks.TaskCompletionSource<bool>>();
            batches = new Dictionary<int, Batch>();
            waitBatchMsg = new Dictionary<int, TaskCompletionSource<bool>>();
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

            // STEP 3: add an entry to waitBatchCommit if it hasn't been added
            if (waitBatchCommit.ContainsKey(context.bid) == false)
                waitBatchCommit.Add(context.bid, new TaskCompletionSource<bool>());

            // STEP 4: wait for the transaction to be committed
            await waitBatchCommit[context.bid].Task;

            // STEP 5: return the result to the client after the transaction is committed
            return transactionResult;
        }

        public async Task ReceiveBatch(Batch batch)
        {
            // STEP 1: store the received batch
            batches.Add(batch.bid, batch);
            if (waitBatchMsg.ContainsKey(batch.bid))
                waitBatchMsg[batch.bid].SetResult(true);

            // STEP 2: add an entry to waitBatchCommit if it hasn't been added
            if (waitBatchCommit.ContainsKey(batch.bid) == false)
                waitBatchCommit.Add(batch.bid, new TaskCompletionSource<bool>());

            /*
             * ATTENTION!! Add code here to maintain your local schedule
             */
            await Task.CompletedTask;
        }

        public async Task<object> Execute(FunctionCall call, TransactionContext context)
        {
            // STEP 0: wait until the sub-batch has arrived
            if (batches.ContainsKey(context.bid) == false)
            {
                waitBatchMsg.Add(context.bid, new TaskCompletionSource<bool>());
                await waitBatchMsg[context.bid].Task;
            }   
            Debug.Assert(batches.ContainsKey(context.bid));

            // STEP 1: block the call until it is its turn to execute
            /*
             * ATTENTION!! It is your task to enforce the actor to execute this call at a proper time
             * 
             * Hint: all transactional calls must be executed in the order determined in the local schedule
             * Hint: use TaskCompletiionSource to asynchronously wait for a task that will be fulfilled by the completion of another transactional call
             */

            // STEP 2: execute the call
            var method = call.actorType.GetMethod(call.funcName);
            var result = await (Task<object>)method.Invoke(this, new object[] { context, call.funcInput });

            // STEP 3: check if the whole batch has been completed on this actor
            /*
             * ATTENTION!! you should implement this part
             * 
             * 
             * if (...)
             * {
             *     // tell the coordinator that this actor has completed the batch
             *     _ = coordinator.BatchComplete(context.bid);
             * }
             */

            // STEP 4: return the result
            return result;
        }

        public Task BatchCommit(int bid)
        {
            // STEP 0: update lastCommittedBid
            lastCommittedBid = Math.Max(lastCommittedBid, bid);

            // STEP 1: set the corresponding waitBatchCommit task as fulfilled
            Debug.Assert(waitBatchCommit.ContainsKey(bid) == true);
            waitBatchCommit[bid].SetResult(true);

            // STEP 2: garbage collection
            /*
             * ATTENTION!! It is your task to garbage collect all metadata that you use to maintain local schedule
             */
            waitBatchCommit.Remove(bid);
            if (waitBatchMsg.ContainsKey(bid)) waitBatchMsg.Remove(bid);
            batches.Remove(bid);
            return Task.CompletedTask;
        }
    }
}