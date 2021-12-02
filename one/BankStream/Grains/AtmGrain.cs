using System;
using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    public class AccountUpdate
    {
        private readonly uint _amount;
        private readonly IAccountGrain _accountGrain;

        public uint Amount => _amount;
        public IAccountGrain AccountGrain => _accountGrain;

        public AccountUpdate(uint amount, IAccountGrain accountGrain)
        {
            _amount = amount;
            _accountGrain = accountGrain;
        }

        public override string ToString()
        {
            return $"{_amount} to {_accountGrain}";
        }
    }
    public class AtmGrain : Grain, IAtmGrain
    {
        private Guid _guid;
        
        public async Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer)
        {
            _guid = Guid.NewGuid();
            var streamProvider = GetStreamProvider(Constants.StreamProvider);
            //var withdrawStream = streamProvider.GetStream<uint>(guid, Constants.WithdrawStreamName);
            var depositStream = streamProvider.GetStream<AccountUpdate>(_guid, Constants.DepositStreamName);


            //await withdrawStream.OnNextAsync(amountToTransfer);
            await depositStream.OnNextAsync(new AccountUpdate(amountToTransfer, toAccount));
            // await Task.WhenAll(
            //     fromAccount.Withdraw(amountToTransfer),
            //     toAccount.Deposit(amountToTransfer));
        }
    }
}