namespace Cicclo.Domain.Exceptions;

public class InvalidAmountException(decimal amount)
    : DomainException($"Valor inválido: {amount}. Informe um valor positivo com até 2 casas decimais.");
