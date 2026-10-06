using Cicclo.Domain.Entities;

namespace Cicclo.Application.Abstractions;

public interface ILaundryServiceRepository
{
    Task<IReadOnlyList<LaundryService>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<LaundryService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
