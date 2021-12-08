using System;
namespace Utilities
{
    [Serializable]
    public class TransactionContext
    {
        public int bid;
        public int tid;

        public TransactionContext(int bid, int tid)
        {
            this.bid = bid;
            this.tid = tid;
        }
    }
}