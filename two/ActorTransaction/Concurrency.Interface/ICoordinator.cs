using System;
using Orleans;
using Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Concurrency.Interface
{
    public interface ICoordinator : IGrainWithIntegerKey
    {
        /*
         * This initerface is called to activate the coordinator
         */
        Task Init();

        /*
         * This interface is interacted by transactional actors to create a new transaction
         */
        Task<TransactionContext> NewTransaction(List<int> actorAccessInfo);

        /*
         * This interface is interacted by transactional actors to acknowledge the completion of a batch
         */
        Task BatchComplete(int bid);

        /*
         * This interface is used to check if all in-memory metadata have been properly garbage collected
         */
        Task CheckGarbageCollection();
    }
}