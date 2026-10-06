using Cicclo.Application.Abstractions;
using Cicclo.Application.Dtos;
using Cicclo.Application.Exceptions;
using Cicclo.Domain.Entities;

namespace Cicclo.Application.Commands;

// RequestId é gerado pelo app e identifica a tentativa: repeti-la não cobra de novo.
public record ExecuteServiceCommand(Guid ServiceId, Guid RequestId);

// Singleton: o lock abaixo precisa ser compartilhado entre todas as requisições.
public class ExecuteServiceCommandHandler(
    ILaundryServiceRepository services,
    IWalletRepository wallets,
    IServiceExecutionRepository executions)
    : ICommandHandler<ExecuteServiceCommand, ServiceExecutionDto>
{
    // Serializa "checar duplicidade -> debitar -> registrar" para o débito ser atômico.
    // Com banco de dados isso viraria uma transação.
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<ServiceExecutionDto> Handle(
        ExecuteServiceCommand command, CancellationToken cancellationToken = default)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var wallet = await wallets.GetAsync(cancellationToken);

            var existing = await executions.GetByRequestIdAsync(command.RequestId, cancellationToken);
            if (existing is not null)
            {
                // Repetição da mesma solicitação: devolve o resultado original, sem nova cobrança.
                if (existing.ServiceId != command.ServiceId)
                    throw new RequestIdConflictException(command.RequestId);

                return ToDto(existing, wallet);
            }

            var service = await services.GetByIdAsync(command.ServiceId, cancellationToken)
                ?? throw new ServiceNotFoundException(command.ServiceId);

            // Valida o RequestId e debita a carteira (lança se o saldo for insuficiente).
            var execution = ServiceExecution.Request(wallet, service, command.RequestId);
            await executions.AddAsync(execution, cancellationToken);

            return ToDto(execution, wallet);
        }
        finally
        {
            _gate.Release();
        }
    }

    private static ServiceExecutionDto ToDto(ServiceExecution e, Wallet wallet) =>
        new(e.Id, e.ServiceId, e.Price, e.Status, e.CreatedAt, wallet.Balance);
}
