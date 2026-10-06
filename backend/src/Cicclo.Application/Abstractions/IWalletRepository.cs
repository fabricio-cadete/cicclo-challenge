using Cicclo.Domain.Entities;

namespace Cicclo.Application.Abstractions;

public interface IWalletRepository
{
    Task<Wallet> GetAsync(CancellationToken cancellationToken = default);
}
