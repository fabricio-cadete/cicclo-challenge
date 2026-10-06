# CLAUDE.md — Backend

Contexto específico da API .NET. O contexto global do projeto está em `../CLAUDE.md`.

## Estrutura

```text
backend/
├── Cicclo.sln
├── src/
│   ├── Cicclo.Api/            # entrada HTTP e composição de dependências
│   ├── Cicclo.Application/    # casos de uso e contratos (Commands, Queries, Abstractions)
│   ├── Cicclo.Domain/         # regras de negócio, sem dependências
│   └── Cicclo.Infrastructure/ # implementa contratos da Application
└── tests/{Cicclo.Domain.Tests,Cicclo.Application.Tests}/  # xUnit
```

Dependências entre camadas:

```text
Api ------------> Application ------> Domain
 └--------------> Infrastructure ---> Application
                                └--> Domain

Domain.Tests ------> Domain
Application.Tests -> Application
```

- Domain não depende de nenhuma outra camada.
- Organizado para CQRS **sem mediador** (sem MediatR por questão de licença) e sem handlers vazios: casos de uso são classes simples na Application, injetadas pela API.

## Comandos

A partir da raiz do repositório:

```sh
dotnet restore backend/Cicclo.sln
dotnet build backend/Cicclo.sln
dotnet test backend/Cicclo.sln
dotnet run --project backend/src/Cicclo.Api --launch-profile http
```

- Health: http://localhost:5080/health
- Serviços: http://localhost:5080/services
- Carteira: http://localhost:5080/wallet
- Executar serviço: `POST /services/{serviceId}/executions` com `{ "requestId": "<guid>" }` (200 ok, 404 serviço inexistente, 409 requestId usado em outro serviço, 422 saldo insuficiente, 400 dados inválidos)
- OpenAPI: http://localhost:5080/openapi/v1.json (apenas Development)
- Exemplos de requisição em `src/Cicclo.Api/Cicclo.Api.http`

Pré-requisito: .NET SDK 10.0.400+ (ver `global.json` na raiz).

## Convenções

- Valores monetários: usar `decimal` (nunca `float`/`double`); considerar tratar em centavos ou garantir precisão ao serializar.
- Testes xUnit nas regras relevantes (Domain e Application); registrar no README o que foi testado e por quê.
