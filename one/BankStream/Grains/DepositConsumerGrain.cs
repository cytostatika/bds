using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Streams;

namespace Grains;

[ImplicitStreamSubscription(Constants.DepositStreamName)]
public class DepositConsumerGrain : Grain, IConsumerGrain
{
    public override async Task OnActivateAsync()
    {
        var guid = this.GetPrimaryKey();

        //Get one of the providers which we defined in config
        var streamProvider = GetStreamProvider(Constants.StreamProvider);

        //Get the reference to withdraw stream
        var stream = streamProvider.GetStream<AccountUpdate>(guid, Constants.DepositStreamName);

        await stream.SubscribeAsync(OnNextMessage);
    }

    private async Task OnNextMessage(AccountUpdate data, StreamSequenceToken token)
    {
        var depositGrain = GrainFactory.GetGrain<IAccountGrain>(data.AccountId);
        await depositGrain.Deposit(data.Amount);
    }
}