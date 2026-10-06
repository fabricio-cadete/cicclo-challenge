using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Tests;

public class LaundryServiceTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_EmptyName_Throws(string name)
    {
        Assert.Throws<DomainException>(() => LaundryService.Create(name, ServiceType.Wash, 18.90m));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Create_InvalidPrice_Throws(decimal price)
    {
        Assert.Throws<InvalidAmountException>(() => LaundryService.Create("Lavagem", ServiceType.Wash, price));
    }
}
