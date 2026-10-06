using Cicclo.Domain.Enums;

namespace Cicclo.Application.Dtos;

public record ServiceExecutionDto(
    Guid Id,
    Guid ServiceId,
    decimal Price,
    ExecutionStatus Status,
    DateTime CreatedAt,
    decimal WalletBalance);
