using Cicclo.Domain.Enums;

namespace Cicclo.Domain.Exceptions;

public class InvalidExecutionStateException(ExecutionStatus current, ExecutionStatus target)
    : DomainException($"Transição inválida de {current} para {target}.");
