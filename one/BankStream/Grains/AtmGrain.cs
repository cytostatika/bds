using System;
using System.Threading.Tasks;
using Common;
using GrainInterfaces;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    public class AtmGrain : Grain, IAtmGrain
    {
        private Guid _guid;
        
        public async Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer)
        {
            _guid = Guid.NewGuid();
            var streamProvider = GetStreamProvider(Constants.StreamProvider);
            //var withdrawStream = streamProvider.GetStream<uint>(guid, Constants.WithdrawStreamName);
            var depositStream = streamProvider.GetStream<uint>(_guid, Constants.DepositStreamName);


            //await withdrawStream.OnNextAsync(amountToTransfer);
            await depositStream.OnNextAsync(amountToTransfer);
            // await Task.WhenAll(
            //     fromAccount.Withdraw(amountToTransfer),
            //     toAccount.Deposit(amountToTransfer));
        }
    }
}