namespace Cicclo.Application.Commands;

// Contrato do caso de uso "solicitar execução de um serviço".
// RequestId é gerado pelo app e identifica a tentativa: repeti-la não cobra de novo.
// O handler (ICommandHandler<RequestServiceExecutionCommand, ServiceExecutionDto>) será implementado na próxima etapa.
public record RequestServiceExecutionCommand(Guid ServiceId, Guid RequestId);
