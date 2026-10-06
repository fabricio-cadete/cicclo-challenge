# Cicclo Challenge

Estrutura inicial do desafio técnico Full Stack Mobile da Cicclo, com Expo + React Native + TypeScript e ASP.NET Core em .NET 10.

## Estrutura

```text
cicclo-challenge/
├── mobile/
│   ├── App.tsx
│   ├── assets/
│   └── src/
│       ├── views/
│       ├── viewmodels/
│       ├── models/
│       ├── services/
│       └── components/
├── backend/
│   ├── Cicclo.sln
│   ├── src/
│   │   ├── Cicclo.Api/
│   │   ├── Cicclo.Application/
│   │   ├── Cicclo.Domain/
│   │   └── Cicclo.Infrastructure/
│   └── tests/
│       ├── Cicclo.Domain.Tests/
│       └── Cicclo.Application.Tests/
└── global.json
```

## Pré-requisitos

- .NET SDK 10.0.400 ou uma versão estável posterior da linha 10.0, conforme global.json.
- Node.js 22.13 ou superior (Node 24 LTS utilizado na validação) e npm.
- Expo Go compatível com SDK 57, ou um emulador Android, para abrir o app nativo.

As versões do app seguem o template oficial blank-typescript: Expo SDK 57, React Native 0.86 e TypeScript 6. O package-lock.json fixa a árvore de dependências.

## Backend

A partir da raiz do repositório:

```sh
dotnet restore backend/Cicclo.sln
dotnet build backend/Cicclo.sln
dotnet test backend/Cicclo.sln
dotnet run --project backend/src/Cicclo.Api --launch-profile http
```

- Saúde: http://localhost:5080/health (resposta JSON com status Healthy).
- Documento OpenAPI: http://localhost:5080/openapi/v1.json (somente em Development).
- O arquivo Cicclo.Api.http contém requisições de exemplo.

O perfil HTTP é apenas para desenvolvimento local. A API não implementa serviços de lavanderia, carteira ou persistência.

## App

Em outro terminal:

```sh
cd mobile
npm ci
npm start
```

Abra o QR code no Expo Go ou pressione `a` para usar um emulador Android disponível. Para a prévia no navegador, execute `npm run web`. O emulador iOS exige macOS; no Windows, um iPhone físico pode usar o Expo Go compatível.

Validação:

```sh
npm run typecheck
npm run lint
npx expo-doctor
npm run export:check
```

A exportação valida os bundles JavaScript de Android, iOS e web; não gera APK/IPA nem substitui um teste em dispositivo.

## Dependências entre camadas

```text
Api ------------> Application ------> Domain
 └--------------> Infrastructure ---> Application
                                └--> Domain

Domain.Tests ------> Domain
Application.Tests -> Application
```

Domain não depende de outras camadas. Application concentra os futuros casos de uso e contratos. Infrastructure implementará esses contratos. Api será o ponto de entrada HTTP e de composição das dependências.

As pastas Commands, Queries e Abstractions preparam a organização para CQRS com MediatR (sem handlers vazios). No app, as pastas de views, viewmodels, models e services reservam a separação para MVVM. A tela inicial ainda não precisa de ViewModel.

## Escopo desta etapa

Somente estrutura, configurações e inicialização. Não há regras de negócio, autenticação, banco de dados, chamadas do app à API ou testes funcionais implementados. Os dois projetos xUnit estão preparados; `dotnet test` informa que não existem testes até que os primeiros comportamentos sejam adicionados.

## Referências

- [Template oficial do Expo](https://docs.expo.dev/more/create-expo/)
- [Documentação do Expo SDK 57](https://docs.expo.dev/versions/v57.0.0/)
- [Solutions com a CLI do .NET](https://learn.microsoft.com/dotnet/core/tools/dotnet-sln)
