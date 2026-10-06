# Application

Casos de uso e contratos da aplicação. Depende somente de Domain.

As pastas Commands e Queries reservam espaço para CQRS: comandos alteram estado, consultas leem dados. Estrutura CQRS sem mediador:

- Abstractions: `ICommandHandler`, `IQueryHandler` (contratos dos handlers) e os repositórios `ILaundryServiceRepository`, `IWalletRepository` e `IServiceExecutionRepository`.
- Dtos: `ServiceDto`, `WalletDto`, `ServiceExecutionDto`.
- Queries: `ListServicesQuery` e seu handler.
- Commands: `RequestServiceExecutionCommand` (apenas o contrato; o handler vem na próxima etapa).
