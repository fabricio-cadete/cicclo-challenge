using Cicclo.Domain.Entities;

namespace Cicclo.Application.Abstractions;

public interface IWalletRepository
{
    // Não há autenticação: existe uma única carteira do usuário.
    Task<Wallet> GetAsync(CancellationToken cancellationToken = default);
}
