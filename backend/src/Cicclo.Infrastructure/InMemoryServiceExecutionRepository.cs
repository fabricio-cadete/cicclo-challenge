using System.Collections.Concurrent;
using Cicclo.Application.Abstractions;
using Cicclo.Domain.Entities;

namespace Cicclo.Infrastructure;

public class InMemoryServiceExecutionRepository : IServiceExecutionRepository
{
    private readonly ConcurrentDictionary<Guid, ServiceExecution> _byRequestId = new();

    public Task<ServiceExecution?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default) =>
        Task.FromResult(_byRequestId.GetValueOrDefault(requestId));

    public Task AddAsync(ServiceExecution execution, CancellationToken cancellationToken = default)
    {
        _byRequestId[execution.RequestId] = execution;
        return Task.CompletedTask;
    }
}
