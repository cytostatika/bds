using Common;
using GrainInterfaces;
using Orleans;

namespace Grains;

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
    public async Task Transfer(IAccountGrain fromAccount, IAccountGrain fromSecondAccount, IAccountGrain toAccount,
        uint amountToTransfer)
    {
        var streamProvider = GetStreamProvider(Constants.StreamProvider);

        var withdrawStream =
            streamProvider.GetStream<List<AccountUpdate>>(Constants.WithdrawId, Constants.WithdrawStreamName);
        var depositStream =
            streamProvider.GetStream<AccountUpdate>(Constants.DepositId, Constants.DepositStreamName);


        var test = new List<AccountUpdate>
        {
            new(amountToTransfer, fromAccount.GetPrimaryKeyString()),
            new(amountToTransfer, fromSecondAccount.GetPrimaryKeyString())
        };


        await withdrawStream.OnNextAsync(test);
        await depositStream.OnNextAsync(new AccountUpdate(amountToTransfer * 2, toAccount.GetPrimaryKeyString()));
    }
}