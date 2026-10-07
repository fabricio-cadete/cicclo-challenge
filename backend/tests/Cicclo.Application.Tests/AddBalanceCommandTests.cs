using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Domain.Entities;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Application.Tests;

public class AddBalanceCommandTests
{
    private class FakeWallets(Wallet wallet) : IWalletRepository
    {
        public Task<Wallet> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(wallet);
    }

    private readonly Wallet _wallet = Wallet.Create();

    private AddBalanceCommandHandler CreateHandler() => new(new FakeWallets(_wallet));

    [Fact]
    public async Task Handle_CreditsWalletAndReturnsNewBalance()
    {
        var result = await CreateHandler().Handle(new AddBalanceCommand(25.50m));

        Assert.Equal(75.50m, result.Balance);
        Assert.Equal(75.50m, _wallet.Balance);
        Assert.Equal(_wallet.Id, result.Id);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(10.555)]
    public async Task Handle_InvalidAmount_ThrowsAndKeepsBalance(decimal amount)
    {
        await Assert.ThrowsAsync<InvalidAmountException>(
            () => CreateHandler().Handle(new AddBalanceCommand(amount)));

        Assert.Equal(50.00m, _wallet.Balance);
    }

    [Fact]
    public async Task Handle_ConcurrentDeposits_NeverLosesAnyAmount()
    {
        var handler = CreateHandler();

        await Task.WhenAll(Enumerable.Range(0, 100)
            .Select(_ => Task.Run(() => handler.Handle(new AddBalanceCommand(1.00m)))));

        Assert.Equal(150.00m, _wallet.Balance);
    }
}
