# Testes

- Cicclo.Domain.Tests: invariantes e regras de negócio (carteira, serviço e execução).
- Cicclo.Application.Tests: casos de uso com repositórios falsos (consultas e execução de serviço, incluindo duplicidade de `RequestId` e concorrência).
- Cicclo.Api.Tests: integração HTTP com `WebApplicationFactory`, usando a API real com dados em memória. Cada teste sobe sua própria instância, então o estado não vaza entre testes. Cobre status HTTP, execução de serviço, saldo da carteira depois da operação, serviço inexistente (404), saldo insuficiente (422), `requestId` repetido/conflitante/vazio, depósito de saldo (válido e inválido) e requisições simultâneas.

Executar tudo: `dotnet test backend/Cicclo.sln`.

Se uma instância da API estiver rodando (por exemplo, em debug), ela trava a DLL em `bin` e o build da solução falha; pare a API ou use `dotnet test <projeto> --artifacts-path <pasta>`.
