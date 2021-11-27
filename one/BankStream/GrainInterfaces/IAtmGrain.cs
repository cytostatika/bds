using System;
using System.Threading.Tasks;
using Orleans;

namespace GrainInterfaces
{
    public interface IAtmGrain : IGrainWithIntegerKey
    {
        Task Transfer(IAccountGrain fromAccount, IAccountGrain toAccount, uint amountToTransfer);
    }
}