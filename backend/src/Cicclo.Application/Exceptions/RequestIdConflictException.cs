namespace Cicclo.Application.Exceptions;

public class RequestIdConflictException(Guid requestId)
    : Exception($"A solicitação {requestId} já foi usada para outro serviço.");
