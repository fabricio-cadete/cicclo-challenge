namespace Cicclo.Domain.Exceptions;

public class InsufficientBalanceException(decimal balance, decimal required)
    : DomainException($"Saldo insuficiente: disponível {balance:F2}, necessário {required:F2}.");
