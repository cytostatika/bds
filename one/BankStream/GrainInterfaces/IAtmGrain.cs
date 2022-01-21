using Orleans;

namespace GrainInterfaces;

public interface IAtmGrain : IGrainWithIntegerKey
{
    Task Transfer(IAccountGrain fromAccount, IAccountGrain fromSecondAccount, IAccountGrain toAccount,
        uint amountToTransfer);
}