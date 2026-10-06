using Cicclo.Domain.Enums;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Entities;

public class LaundryService
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public ServiceType Type { get; private set; }
    public decimal Price { get; private set; }

    private LaundryService() { }

    public static LaundryService Create(string name, ServiceType type, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("O nome do serviço é obrigatório.");

        Wallet.EnsureValidAmount(price);

        return new LaundryService { Id = Guid.NewGuid(), Name = name.Trim(), Type = type, Price = price };
    }
}
