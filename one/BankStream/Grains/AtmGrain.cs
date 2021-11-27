using System;
using System.Threading.Tasks;
using GrainInterfaces;
using Orleans;
using Orleans.Concurrency;

namespace Grains
{
    [StatelessWorker]
    public class AtmGrain : Grain, IAtmGrain
    {
        public async Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer)
        {
            await Task.WhenAll(
                fromAccount.Withdraw(amountToTransfer),
                toAccount.Deposit(amountToTransfer));
        }
    }
}