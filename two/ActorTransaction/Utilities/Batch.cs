using System;
using System.Collections.Generic;

namespace Utilities
{
    [Serializable]
    public class Batch
    {
        public int bid;
        public int lastBid;

        // it is the list of transactions that will access this actor
        // transactions in the list should be executed in the order of tids
        public List<int> transactionList;

        public Batch(int bid, int lastBid)
        {
            this.bid = bid;
            this.lastBid = lastBid;
            transactionList = new List<int>();
        }

        public void AddTransaction(int tid)
        {
            transactionList.Add(tid);
        }
    }
}