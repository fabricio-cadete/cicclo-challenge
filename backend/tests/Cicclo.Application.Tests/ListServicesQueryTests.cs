using Cicclo.Application.Abstractions;
using Cicclo.Application.Queries;
using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;

namespace Cicclo.Application.Tests;

public class ListServicesQueryTests
{
    private class FakeRepository(params LaundryService[] services) : ILaundryServiceRepository
    {
        public Task<IReadOnlyList<LaundryService>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LaundryService>>(services);

        public Task<LaundryService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(services.FirstOrDefault(s => s.Id == id));
    }

    [Fact]
    public async Task Handle_ReturnsAllServicesWithPrices()
    {
        var wash = LaundryService.Create("Lavagem", ServiceType.Wash, 18.90m);
        var dry = LaundryService.Create("Secagem", ServiceType.Dry, 20.90m);
        var handler = new ListServicesQueryHandler(new FakeRepository(wash, dry));

        var result = await handler.Handle(new ListServicesQuery());

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Id == wash.Id && s.Name == "Lavagem" && s.Price == 18.90m);
        Assert.Contains(result, s => s.Id == dry.Id && s.Name == "Secagem" && s.Price == 20.90m);
    }

    [Fact]
    public async Task Handle_NoServices_ReturnsEmptyList()
    {
        var handler = new ListServicesQueryHandler(new FakeRepository());

        Assert.Empty(await handler.Handle(new ListServicesQuery()));
    }
}
