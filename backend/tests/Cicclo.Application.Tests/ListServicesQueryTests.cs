using Cicclo.Application.Abstractions;
using Cicclo.Application.Queries;
using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;

namespace Cicclo.Application.Tests;

public class ListServicesQueryTests
{
    private class FakeRepository(params LaundryService[] services) : ILaundryServiceRepository
    {
        public IReadOnlyList<LaundryService> GetAll() => services;
    }

    [Fact]
    public void Handle_ReturnsAllServicesWithPrices()
    {
        var wash = LaundryService.Create("Lavagem", ServiceType.Wash, 18.90m);
        var dry = LaundryService.Create("Secagem", ServiceType.Dry, 20.90m);
        var query = new ListServicesQuery(new FakeRepository(wash, dry));

        var result = query.Handle();

        Assert.Equal(2, result.Count);
        Assert.Contains(result, s => s.Id == wash.Id && s.Name == "Lavagem" && s.Price == 18.90m);
        Assert.Contains(result, s => s.Id == dry.Id && s.Name == "Secagem" && s.Price == 20.90m);
    }

    [Fact]
    public void Handle_NoServices_ReturnsEmptyList()
    {
        var query = new ListServicesQuery(new FakeRepository());

        Assert.Empty(query.Handle());
    }
}
