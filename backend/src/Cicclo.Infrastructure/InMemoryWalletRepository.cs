using Cicclo.Application.Abstractions;
using Cicclo.Domain.Entities;

namespace Cicclo.Infrastructure;

// Carteira única em memória, criada com o saldo inicial (R$ 50,00).
public class InMemoryWalletRepository : IWalletRepository
{
    private readonly Wallet _wallet = Wallet.Create();

    public Task<Wallet> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(_wallet);
}
