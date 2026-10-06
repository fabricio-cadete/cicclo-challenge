using Cicclo.Domain.Enums;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Entities;

public class ServiceExecution
{
    public Guid Id { get; private set; }

    // Chave enviada pelo cliente: a mesma solicitação repetida não gera nova cobrança.
    public Guid RequestId { get; private set; }
    public Guid WalletId { get; private set; }
    public Guid ServiceId { get; private set; }

    // Preço cobrado no momento da solicitação (o preço do serviço pode mudar depois).
    public decimal Price { get; private set; }
    public ExecutionStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private ServiceExecution() { }

    // Debita a carteira e registra a execução. Se o saldo for insuficiente, lança antes de criar a execução.
    public static ServiceExecution Request(Wallet wallet, LaundryService service, Guid requestId)
    {
        if (requestId == Guid.Empty)
            throw new DomainException("O identificador da solicitação é obrigatório.");

        wallet.Debit(service.Price);

        return new ServiceExecution
        {
            Id = Guid.NewGuid(),
            RequestId = requestId,
            WalletId = wallet.Id,
            ServiceId = service.Id,
            Price = service.Price,
            Status = ExecutionStatus.Requested,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Complete() => TransitionTo(ExecutionStatus.Completed);

    // Falha na execução: estorna o valor cobrado.
    public void Fail(Wallet wallet)
    {
        TransitionTo(ExecutionStatus.Failed);
        wallet.Credit(Price);
    }

    private void TransitionTo(ExecutionStatus target)
    {
        if (Status != ExecutionStatus.Requested)
            throw new InvalidExecutionStateException(Status, target);

        Status = target;
    }
}
