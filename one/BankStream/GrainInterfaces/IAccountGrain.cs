using Orleans;

namespace GrainInterfaces;

public interface IAccountGrain : IGrainWithStringKey
{
    Task Withdraw(uint amount);
    Task CommitWithdraw(uint amount);

    Task Deposit(uint amount);

    Task<uint> GetBalance();
}