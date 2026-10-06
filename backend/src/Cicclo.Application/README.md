# Application

Casos de uso e contratos da aplicação. Depende somente de Domain.

As pastas Commands e Queries reservam espaço para CQRS: comandos alteram estado, consultas leem dados. Abstractions receberá os contratos necessários aos casos de uso. O MediatR está configurado (`AddApplication()`), mas nenhum handler ou regra foi implementado ainda.
