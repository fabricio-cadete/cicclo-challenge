# Application

Casos de uso e contratos da aplicação. Depende somente de Domain.

As pastas Commands e Queries reservam espaço para CQRS: comandos alteram estado, consultas leem dados. Estrutura CQRS sem mediador:

- Abstractions: `ICommandHandler`, `IQueryHandler` (contratos dos handlers) e os repositórios `ILaundryServiceRepository`, `IWalletRepository` e `IServiceExecutionRepository`.
- Dtos: `ServiceDto`, `WalletDto`, `ServiceExecutionDto`.
- Queries: `ListServicesQuery` e `GetWalletQuery`, cada uma com seu handler.
- Commands: `AddBalanceCommand` (credita a carteira e devolve o novo saldo) e `ExecuteServiceCommand` e seu handler. Valida o serviço, impede cobrança duplicada pelo `RequestId` (repetir devolve o resultado original; usar o mesmo id para outro serviço gera conflito), debita a carteira e registra a execução. Um `SemaphoreSlim` serializa o fluxo para o débito ser atômico (com banco de dados seria uma transação), por isso o handler é singleton.
- Exceptions: `ServiceNotFoundException` e `RequestIdConflictException`.
