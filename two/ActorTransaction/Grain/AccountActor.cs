using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Concurrency;
using Grain.Interface;
using Utilities;

namespace Grain
{
    public class Account
    {
        public int accountID;
        public int balance;
    }

    public class AccountActor : TransactionalActor<Account>, IAccountActor
    {
        public AccountActor() : base()
        {
        }

        public async Task<object> Init(TransactionContext context, object funcInput)
        {
            await Task.CompletedTask;
            var myState = (Account)this.actorState;
            myState.accountID = (int)funcInput;
            myState.balance = Constants.InitialBalance;
            return null;
        }

        public async Task<object> GetBalance(TransactionContext context, object funcInput)
        {
            await Task.CompletedTask;
            var myState = (Account)this.actorState;
            return myState.balance;
        }

        public async Task<object> Transfer(TransactionContext context, object funcInput)
        {
            var myState = (Account)this.actorState;
            var input = (Tuple<int, int>)funcInput;
            var money = input.Item1;
            var toAccount = input.Item2;
            Debug.Assert(myState.accountID != toAccount);

            if (myState.balance < money) money = 0;

            var toAccountActor = GrainFactory.GetGrain<IAccountActor>(toAccount);
            var call = new FunctionCall("Deposit", money, typeof(AccountActor));
            await toAccountActor.Execute(call, context);

            myState.balance -= money;
            return null;
        }

        public async Task<object> Deposit(TransactionContext context, object funcInput)
        {
            await Task.CompletedTask;
            var myState = (Account)this.actorState;
            var money = (int)funcInput;
            myState.balance += money;
            return null;
        }
    }
}