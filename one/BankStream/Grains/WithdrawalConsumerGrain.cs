using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace Grains;

[ImplicitStreamSubscription(Constants.WithdrawStreamName)]
public class WithdrawalConsumerGrain : Grain, IConsumerGrain
{
    public override async Task OnActivateAsync()
    {
        var guid = this.GetPrimaryKey();

        //Get one of the providers which we defined in config
        var streamProvider = GetStreamProvider(Constants.StreamProvider);

        //Get the reference to withdraw stream
        var stream = streamProvider.GetStream<List<AccountUpdate>>(guid, Constants.WithdrawStreamName);

        await stream.SubscribeAsync(OnNextMessage);
    }

    private async Task OnNextMessage(List<AccountUpdate> data, StreamSequenceToken token)
    {
        var updates = new Dictionary<string, uint>();
        foreach (var dat in data)
        {
            var withdrawGrain = GrainFactory.GetGrain<IAccountGrain>(dat.AccountId);
            if (!updates.TryAdd(dat.AccountId, dat.Amount)) updates[withdrawGrain.GetPrimaryKeyString()] += dat.Amount;
        }

        foreach (var key in updates.Keys)
        {
            var withdrawGrain = GrainFactory.GetGrain<IAccountGrain>(key);
            await withdrawGrain.Withdraw(updates[key]);
        }

        foreach (var dat in data)
        {
            var withdrawGrain = GrainFactory.GetGrain<IAccountGrain>(dat.AccountId);
            await withdrawGrain.CommitWithdraw(dat.Amount);
        }
    }
}