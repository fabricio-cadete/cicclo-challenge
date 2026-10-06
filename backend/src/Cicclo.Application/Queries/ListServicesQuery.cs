using Cicclo.Application.Abstractions;
using Cicclo.Domain.Enums;

namespace Cicclo.Application.Queries;

public record ServiceDto(Guid Id, string Name, ServiceType Type, decimal Price);

public class ListServicesQuery(ILaundryServiceRepository services)
{
    public IReadOnlyList<ServiceDto> Handle() =>
        services.GetAll()
            .Select(s => new ServiceDto(s.Id, s.Name, s.Type, s.Price))
            .ToList();
}
