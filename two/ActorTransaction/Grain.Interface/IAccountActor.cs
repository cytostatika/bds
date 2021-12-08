using System;
using System.Threading.Tasks;
using Concurrency.Interface;
using Utilities;

namespace Grain.Interface
{
    public interface IAccountActor : ITransactionalActor
    {
        Task<object> Init(TransactionContext context, object funcInput);
        Task<object> GetBalance(TransactionContext context, object funcInput);
        Task<object> Transfer(TransactionContext context, object funcInput);
        Task<object> Deposit(TransactionContext context, object funcInput);
    }
}