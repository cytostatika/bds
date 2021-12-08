using System;
using Orleans;
using Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Concurrency.Interface
{
    public interface ITransactionalActor : IGrainWithIntegerKey
    {
        /* 
         * This interface is interacted by clients to submit transactions to the system
         * 
         * startFunc -- the first function called by the transaction
         * funcInput -- input data passed to the called function
         * actorAccessInfo -- the list of actors that will be accessed by the transaction
         * 
         * returned object: data that the client tends to read
         */
        Task<object> SubmitTransaction(string funcName, object funcInput, List<int> actorAccessInfo);

        /*
         * This interface is interacted by the coordinator to send a batch to an actor
         */
        Task ReceiveBatch(Batch batch);

        /*
         * This interface is interacted by other transactional actors to invoke a transactional function call
         * 
         * returned object: data read from the transactional actor
         */
        Task<object> Execute(FunctionCall call, TransactionContext context);

        /* 
         * This interface is interacted by the coordinator to commit a batch
         */
        Task BatchCommit(int bid);

        /*
         * This interface is used to check if all in-memory metadata have been properly garbage collected
         */
        Task CheckGarbageCollection();
    }
}