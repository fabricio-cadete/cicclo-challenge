# Domain

Entidades, enums, exceções e regras de negócio. Não referencia outros projetos, ASP.NET Core ou bibliotecas de persistência.

- Entities: `Wallet` (saldo, débito e crédito, thread-safe via lock), `LaundryService` (serviço e preço) e `ServiceExecution` (solicitação de execução, com débito na criação e estorno em caso de falha).
- Enums: `ServiceType`, `ExecutionStatus`.
- Exceptions: `DomainException` e suas derivadas (saldo insuficiente, valor inválido, transição de estado inválida).

Valores monetários usam `decimal`, positivos e com no máximo 2 casas decimais.
