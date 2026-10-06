using Cicclo.Domain.Entities;

namespace Cicclo.Application.Abstractions;

public interface IServiceExecutionRepository
{
    // Permite detectar solicitação repetida (mesmo RequestId) e evitar cobrança duplicada.
    Task<ServiceExecution?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);

    Task AddAsync(ServiceExecution execution, CancellationToken cancellationToken = default);
}
