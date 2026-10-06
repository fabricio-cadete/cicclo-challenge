using Cicclo.Application.Abstractions;
using Cicclo.Application.Dtos;

namespace Cicclo.Application.Queries;

public record GetWalletQuery;

public class GetWalletQueryHandler(IWalletRepository wallets)
    : IQueryHandler<GetWalletQuery, WalletDto>
{
    public async Task<WalletDto> Handle(GetWalletQuery query, CancellationToken cancellationToken = default)
    {
        var wallet = await wallets.GetAsync(cancellationToken);

        return new WalletDto(wallet.Id, wallet.Balance);
    }
}
