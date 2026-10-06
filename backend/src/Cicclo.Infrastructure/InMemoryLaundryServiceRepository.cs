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

    public IReadOnlyList<LaundryService> GetAll() => _services;
}
