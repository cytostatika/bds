using System;
using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Orleans;

namespace Grains
{
    public class AccountUpdate
    {
        public AccountUpdate(uint amount, string accountId)
        {
            Amount = amount;
            AccountId = accountId;
        }

        public uint Amount { get; }

        public string AccountId { get; }
    }

    public class AtmGrain : Grain, IAtmGrain
    {
        private Guid _guid;

        public async Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer)
        {
            _guid = Guid.NewGuid();
            var streamProvider = GetStreamProvider(Constants.StreamProvider);
            var withdrawStream = streamProvider.GetStream<AccountUpdate>(_guid, Constants.WithdrawStreamName);
            var depositStream = streamProvider.GetStream<AccountUpdate>(_guid, Constants.DepositStreamName);

            Task withdrawTask =
                withdrawStream.OnNextAsync(new AccountUpdate(amountToTransfer, fromAccount.GetPrimaryKeyString()));
            Task depositTask =
                depositStream.OnNextAsync(new AccountUpdate(amountToTransfer, toAccount.GetPrimaryKeyString()));

            await Task.WhenAll(
                withdrawTask,
                depositTask);
        }
    }
}