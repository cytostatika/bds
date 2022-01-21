using GrainInterfaces;
using Orleans;

namespace Grains;

[Serializable]
public class Balance
{
    public uint Value { get; set; } = 1000;
}

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
            throw new InvalidOperationException(
                $"Withdrawing {amount} credits from account \"{this.GetPrimaryKeyString()}\" would overdraw it."
                + $" This account has {_balance.Value} credits.");

        //_balance.Value -= amount;
        return Task.CompletedTask;
    }

    public Task CommitWithdraw(uint amount)
    {
        _balance.Value -= amount;
        return Task.CompletedTask;
    }

    public Task<uint> GetBalance()
    {
        return Task.FromResult(_balance.Value);
    }
}