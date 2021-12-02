using System;
using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Streams;


namespace Grains
{
    [Serializable]
    public class Balance
    {
        public uint Value { get; set; } = 1000;
    }
    
    [ImplicitStreamSubscription(Constants.DepositStreamName)]
    public class AccountGrain : Grain, IAccountGrain
    {
        private readonly Balance _balance;
        
        public AccountGrain()
        {
            _balance = new Balance();
        }

        public Task Deposit(uint amount)
        {
            _balance.Value += amount;
            return Task.CompletedTask;
        } 

        public Task Withdraw(uint amount)
        {
            if (_balance.Value < amount)
            {
                throw new InvalidOperationException(
                    $"Withdrawing {amount} credits from account \"{this.GetPrimaryKeyString()}\" would overdraw it."
                    + $" This account has {_balance.Value} credits.");
            }
            _balance.Value -= amount;
            return Task.CompletedTask;
        }

        public Task<uint> GetBalance() => Task.FromResult(_balance.Value);
        
        
        public override async Task OnActivateAsync()
        {
            //Create a GUID based on our GUID as a grain
            var identityString = this.IdentityString;
            var guid = this.GetPrimaryKey(out identityString);
            //Get one of the providers which we defined in config
            var streamProvider = GetStreamProvider(Constants.StreamProvider);
            //Get the reference to a stream
            var stream = streamProvider.GetStream<AccountUpdate>(guid, Constants.DepositStreamName);
            //Set our OnNext method to the lambda which simply prints the data. This doesn't make new subscriptions, because we are using implicit subscriptions via [ImplicitStreamSubscription].
            await stream.SubscribeAsync(async (data, token) =>
            {
                Console.WriteLine(data);
                await data.AccountGrain.Deposit(data.Amount);
            });
        }
        
        
    }
}