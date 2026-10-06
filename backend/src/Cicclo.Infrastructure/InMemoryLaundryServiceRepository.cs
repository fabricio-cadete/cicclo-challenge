using Cicclo.Application.Abstractions;
using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;

namespace Cicclo.Infrastructure;

// Dados em memória (sem banco): reiniciar a API recria os serviços.
public class InMemoryLaundryServiceRepository : ILaundryServiceRepository
{
    private readonly IReadOnlyList<LaundryService> _services =
    [
        LaundryService.Create("Lavagem", ServiceType.Wash, 18.90m),
        LaundryService.Create("Secagem", ServiceType.Dry, 20.90m)
    ];

    public Task<IReadOnlyList<LaundryService>> GetAllAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(_services);

    public Task<LaundryService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        Task.FromResult(_services.FirstOrDefault(s => s.Id == id));
}
