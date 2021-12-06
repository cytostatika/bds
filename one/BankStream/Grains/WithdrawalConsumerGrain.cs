using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Concurrency;
using Orleans.Streams;

namespace Grains
{
    [ImplicitStreamSubscription(Constants.WithdrawStreamName)]
    [StatelessWorker]
    public class WithdrawalConsumerGrain : Grain, IConsumerGrain
    {
        public override async Task OnActivateAsync()
        {
            var guid = this.GetPrimaryKey();

            //Get one of the providers which we defined in config
            var streamProvider = GetStreamProvider(Constants.StreamProvider);

            //Get the reference to withdraw stream
            var stream = streamProvider.GetStream<AccountUpdate>(guid, Constants.WithdrawStreamName);
            //Set our OnNext method to the lambda which simply prints the data. This doesn't make new subscriptions, because we are using implicit subscriptions via [ImplicitStreamSubscription].
            await stream.SubscribeAsync(async (data, token) =>
            {
                var withdrawGrain = GrainFactory.GetGrain<IAccountGrain>(data.AccountId);
                await withdrawGrain.Withdraw(data.Amount);
            });
        }
    }
}