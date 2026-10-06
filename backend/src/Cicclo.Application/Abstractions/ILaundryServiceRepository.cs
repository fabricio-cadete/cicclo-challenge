using Cicclo.Domain.Entities;

namespace Cicclo.Application.Abstractions;

public interface ILaundryServiceRepository
{
    IReadOnlyList<LaundryService> GetAll();
}
