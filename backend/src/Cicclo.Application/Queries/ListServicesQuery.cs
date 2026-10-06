using Cicclo.Application.Abstractions;
using Cicclo.Application.Dtos;

namespace Cicclo.Application.Queries;

public record ListServicesQuery;

public class ListServicesQueryHandler(ILaundryServiceRepository services)
    : IQueryHandler<ListServicesQuery, IReadOnlyList<ServiceDto>>
{
    public async Task<IReadOnlyList<ServiceDto>> Handle(
        ListServicesQuery query, CancellationToken cancellationToken = default)
    {
        var all = await services.GetAllAsync(cancellationToken);

        return all.Select(s => new ServiceDto(s.Id, s.Name, s.Type, s.Price)).ToList();
    }
}
