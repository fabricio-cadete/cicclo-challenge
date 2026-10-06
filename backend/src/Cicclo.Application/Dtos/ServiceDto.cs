using Cicclo.Domain.Enums;

namespace Cicclo.Application.Dtos;

public record ServiceDto(Guid Id, string Name, ServiceType Type, decimal Price);
