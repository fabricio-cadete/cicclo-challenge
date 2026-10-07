using Cicclo.Domain.Entities;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Tests;

public class WalletTests
{
    [Fact]
    public void Create_StartsWithInitialBalance()
    {
        Assert.Equal(50.00m, Wallet.Create().Balance);
    }

    [Fact]
    public void Debit_ReducesBalance()
    {
        var wallet = Wallet.Create();

        wallet.Debit(18.90m);

        Assert.Equal(31.10m, wallet.Balance);
    }

    [Fact]
    public void Debit_ExactBalance_LeavesZero()
    {
        var wallet = Wallet.Create();

        wallet.Debit(50.00m);

        Assert.Equal(0m, wallet.Balance);
    }

    [Fact]
    public void Debit_InsufficientBalance_ThrowsAndKeepsBalance()
    {
        var wallet = Wallet.Create();

        Assert.Throws<InsufficientBalanceException>(() => wallet.Debit(50.01m));
        Assert.Equal(50.00m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.123)]
    public void Debit_InvalidAmount_Throws(decimal amount)
    {
        var wallet = Wallet.Create();

        Assert.Throws<InvalidAmountException>(() => wallet.Debit(amount));
    }

    [Fact]
    public void Credit_IncreasesBalance()
    {
        var wallet = Wallet.Create();

        wallet.Credit(10.50m);

        Assert.Equal(60.50m, wallet.Balance);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10.123)]
    public void Credit_InvalidAmount_ThrowsAndKeepsBalance(decimal amount)
    {
        var wallet = Wallet.Create();

        Assert.Throws<InvalidAmountException>(() => wallet.Credit(amount));
        Assert.Equal(50.00m, wallet.Balance);
    }
}
