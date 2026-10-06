namespace Cicclo.Application.Exceptions;

public class ServiceNotFoundException(Guid serviceId)
    : Exception($"Serviço {serviceId} não encontrado.");
