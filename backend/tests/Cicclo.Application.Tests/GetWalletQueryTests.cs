using Cicclo.Application.Abstractions;
using Cicclo.Application.Queries;
using Cicclo.Domain.Entities;

namespace Cicclo.Application.Tests;

public class GetWalletQueryTests
{
    private class FakeWalletRepository(Wallet wallet) : IWalletRepository
    {
        public Task<Wallet> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(wallet);
    }

    [Fact]
    public async Task Handle_ReturnsWalletIdAndBalance()
    {
        var wallet = Wallet.Create();
        var handler = new GetWalletQueryHandler(new FakeWalletRepository(wallet));

        var result = await handler.Handle(new GetWalletQuery());

        Assert.Equal(wallet.Id, result.Id);
        Assert.Equal(50.00m, result.Balance);
    }

    [Fact]
    public async Task Handle_ReflectsCurrentBalanceAfterDebit()
    {
        var wallet = Wallet.Create();
        wallet.Debit(18.90m);
        var handler = new GetWalletQueryHandler(new FakeWalletRepository(wallet));

        var result = await handler.Handle(new GetWalletQuery());

        Assert.Equal(31.10m, result.Balance);
    }
}
