# Cicclo Challenge

Aplicação desenvolvida para o desafio técnico Full Stack Mobile da **Cicclo**. Permite consultar uma carteira digital, visualizar os serviços de lavanderia e solicitar sua execução utilizando o saldo disponível.

## Funcionalidades

- Carteira com saldo inicial de **R$ 50,00**.
- **Lavagem por R$ 18,90** e **Secagem por R$ 20,90**.
- Confirmação do serviço, débito e atualização do saldo.
- Validação de saldo insuficiente e mensagens de sucesso ou erro.
- Proteção contra cobranças duplicadas por `requestId`.
- **Adicionar saldo**, funcionalidade extra para facilitar a demonstração após consumir o saldo inicial. É um crédito simulado, sem integração com pagamentos.

## Tecnologias

- **Backend:** C#, .NET 10, ASP.NET Core, xUnit e armazenamento em memória.
- **Mobile:** React Native 0.86, Expo SDK 57, TypeScript 6, Expo Router e Axios.
- **Testes do mobile:** Jest e React Native Testing Library.

## Arquitetura

O backend utiliza **Clean Architecture + CQRS**, dividido em quatro projetos:

| Camada | Responsabilidade |
| --- | --- |
| `Domain` | Entidades e regras de carteira, serviços e execuções |
| `Application` | Consultas, comandos, handlers e contratos de repositório |
| `Infrastructure` | Implementação dos repositórios em memória |
| `Api` | Controllers, tratamento de erros e injeção de dependência |

No mobile, a organização segue **MVVM**: Views apresentam a interface; ViewModels em hooks concentram estado e ações; services realizam as chamadas HTTP; models definem os contratos. As rotas ficam em `mobile/src/app/`.

## Regras e consistência

- O preço cobrado é consultado pelo backend; o cliente envia apenas o serviço e o `requestId`.
- Valores monetários usam `decimal` no backend. Movimentações devem ser positivas e ter no máximo duas casas decimais; a carteira não permite saldo negativo.
- Repetir um `requestId` já registrado para o mesmo serviço retorna a execução existente sem novo débito. Usá-lo para outro serviço gera conflito. O saldo retornado é o atual da carteira.
- Após falha sem resposta HTTP, o mobile preserva a chave para uma nova tentativa manual. A chave é descartada após sucesso ou erro com resposta HTTP, inclusive `5xx`.

## API

| Método | Endpoint | Descrição |
| --- | --- | --- |
| `GET` | `/health` | Saúde da aplicação |
| `GET` | `/services` | Serviços disponíveis |
| `GET` | `/wallet` | Carteira e saldo |
| `POST` | `/services/{serviceId}/execute` | Solicitar serviço |
| `POST` | `/wallet/deposit` | Adicionar saldo |

Para executar um serviço, utilize o `serviceId` obtido em `/services` e envie:

```json
{ "requestId": "8bfcc530-5621-46bb-8b66-9bdf806e0244" }
```

Use um UUID novo para cada solicitação e preserve-o ao repetir a mesma tentativa. Para adicionar saldo:

```json
{ "amount": 25.50 }
```

Operações válidas retornam `200`. Os principais erros são `400` para dados inválidos, `404` para serviço inexistente, `409` para conflito de `requestId` e `422` para saldo insuficiente. Erros de negócio utilizam `ProblemDetails`.

Exemplos de requisições estão em [Cicclo.Api.http](backend/src/Cicclo.Api/Cicclo.Api.http). O documento OpenAPI fica em `/openapi/v1.json`, somente em `Development`.

## Execução local

**Pré-requisitos:** .NET SDK 10.0.400 ou versão compatível da linha 10.0 conforme `global.json`, Node.js 22.13+ e npm, além de Expo Go compatível com SDK 57 ou emulador Android.

### Backend

Na raiz do repositório:

```sh
dotnet restore backend/Cicclo.sln
dotnet build backend/Cicclo.sln
dotnet run --project backend/src/Cicclo.Api --launch-profile http
```

A API inicia em `http://localhost:5080`. Verifique [o health check](http://localhost:5080/health). Não é necessário configurar banco de dados.

### Mobile

Crie `mobile/.env` com a URL acessível pelo dispositivo:

```dotenv
EXPO_PUBLIC_API_URL=http://10.0.2.2:5080
```

- **Emulador Android:** `http://10.0.2.2:5080`.
- **Celular físico:** `http://IP_LOCAL_DO_COMPUTADOR:5080`, substituindo pelo IP real.
- **API hospedada:** `https://cicclo-challenge.onrender.com`.

Para acessar pelo celular físico, mantenha os dispositivos na mesma rede, permita a porta no firewall e inicie a API com:

```sh
dotnet run --project backend/src/Cicclo.Api --launch-profile http --urls "http://0.0.0.0:5080"
```

Em outro terminal, a partir da raiz:

```sh
cd mobile
npm ci
npm start
```

Abra pelo Expo Go ou pressione `a` para usar o emulador Android. Após alterar o `.env`, reinicie o Expo e recarregue o app. Sem a variável, o aplicativo usa `10.0.2.2:5080` no Android e `localhost:5080` nas demais plataformas, via HTTP.

### Docker (opcional)

Para executar a API em contêiner, na raiz:

```sh
docker build -t cicclo-api ./backend
docker run --rm -p 8080:8080 cicclo-api
```

Nesse caso, use a porta **8080** na URL configurada no mobile.

## Testes

Os testes priorizam consistência do saldo, repetição de solicitações e os principais fluxos do usuário.

| Camada | Cenários cobertos |
| --- | --- |
| Domain | Saldo inicial, débito/crédito, valores inválidos, saldo insuficiente, solicitação e estorno no domínio |
| Application | Consultas, execução, idempotência, conflito de chave e concorrência de execuções e depósitos |
| API | Integração com `WebApplicationFactory`, contratos HTTP, erros, saldo e solicitações concorrentes |
| Mobile | Carregamento, confirmação/cancelamento, atualização de saldo, erros, reutilização de `requestId` e recarga |

Os testes de API usam instâncias isoladas. Os testes do mobile exercitam telas, ViewModels e services com uma API falsa; não são testes ponta a ponta contra o backend.

Na raiz, execute os testes do backend:

```sh
dotnet test backend/Cicclo.sln
```

Para o mobile, a partir da raiz:

```sh
cd mobile
npm test -- --runInBand
npm run typecheck
npm run lint
```

## APK e API hospedada

**APK Android no OneDrive:** `https://1drv.ms/u/c/2f0b48503cc8d66a/IQBN_IJCAF0xSof8o8MhChaRATjQTtHFSSchxiUfTKhTM4M?e=ORLKgc`

**API no Render:** [health check](https://cicclo-challenge.onrender.com/health).

A configuração do APK de avaliação aponta para `https://cicclo-challenge.onrender.com`. Caso a API esteja retomando após inatividade, aguarde e tente novamente.

## Escopo, limitações e melhorias

A prioridade foi entregar o fluxo solicitado com regras consistentes e testes, dentro do prazo do desafio. O armazenamento em memória simplifica a execução, e a recarga permite continuar a demonstração sem reiniciar a API.

- **Persistência:** reiniciar a API apaga saldo e execuções e recria os identificadores dos serviços. As chaves pendentes do mobile também não sobrevivem à perda do ViewModel.
- **Escopo:** há uma carteira, sem autenticação, múltiplos usuários, histórico visível ou integração com máquinas e pagamentos.
- **Execução:** a API registra o status `Requested`. Conclusão e estorno existem no domínio, mas não são expostos no aplicativo.
- **Interface:** o saldo pode ficar desatualizado após ações de outros clientes ou erros, até uma nova consulta. A prévia web exige configuração de CORS na API.

Com mais tempo, os próximos passos seriam persistência com transações, integração com banco de dados real, autenticação por usuário, reconciliação de tentativas pendentes, histórico de execuções e automação de build/testes.

## Uso de IA

Utilizei **Claude Code para agilizar a implementação da aplicação** e **GPT para auxiliar na documentação**. Os arquivos `CLAUDE.md` registram contexto e orientações para os agentes. A responsabilidade pelo código entregue, pelas decisões técnicas e pela compreensão da solução permanece comigo.
