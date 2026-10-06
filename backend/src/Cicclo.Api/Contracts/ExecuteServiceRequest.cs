namespace Cicclo.Api.Contracts;

// RequestId é gerado pelo app e identifica a tentativa: repeti-lo não cobra de novo.
public record ExecuteServiceRequest(Guid RequestId);
