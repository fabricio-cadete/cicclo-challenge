# Application

Casos de uso e contratos da aplicação. Depende somente de Domain.

As pastas Commands e Queries reservam espaço para CQRS: comandos alteram estado, consultas leem dados. Abstractions contém os contratos (hoje `ILaundryServiceRepository`). Consultas e comandos são classes simples; hoje existe `ListServicesQuery`. Nenhum handler, mediador ou regra foi implementado nesta etapa.
