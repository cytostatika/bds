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
        public async Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer)
        {
            var streamProvider = GetStreamProvider(Constants.StreamProvider);

            var withdrawStream =
                streamProvider.GetStream<AccountUpdate>(Constants.WithdrawId, Constants.WithdrawStreamName);
            var depositStream =
                streamProvider.GetStream<AccountUpdate>(Constants.DepositId, Constants.DepositStreamName);

            await withdrawStream.OnNextAsync(new AccountUpdate(amountToTransfer, fromAccount.GetPrimaryKeyString()));
            await depositStream.OnNextAsync(new AccountUpdate(amountToTransfer, toAccount.GetPrimaryKeyString()));
        }
    }
}