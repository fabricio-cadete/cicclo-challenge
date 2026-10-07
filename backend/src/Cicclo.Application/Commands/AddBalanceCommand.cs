using Cicclo.Application.Abstractions;
using Cicclo.Application.Dtos;

namespace Cicclo.Application.Commands;

public record AddBalanceCommand(decimal Amount);

public class AddBalanceCommandHandler(IWalletRepository wallets)
    : ICommandHandler<AddBalanceCommand, WalletDto>
{
    public async Task<WalletDto> Handle(AddBalanceCommand command, CancellationToken cancellationToken = default)
    {
        var wallet = await wallets.GetAsync(cancellationToken);

        // Valida o valor (positivo, até 2 casas) e credita.
        wallet.Credit(command.Amount);

        return new WalletDto(wallet.Id, wallet.Balance);
    }
}
