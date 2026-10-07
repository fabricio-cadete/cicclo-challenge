# CLAUDE.md

Contexto global do projeto para o Claude Code. Leia antes de qualquer tarefa.
Detalhes específicos de cada parte ficam em `mobile/CLAUDE.md` e `backend/CLAUDE.md`.

## Sobre o projeto

Desafio técnico Full Stack Mobile da **Cicclo** (etapa técnica de processo seletivo). Aplicação mobile simples que representa parte do fluxo de uma lavanderia automatizada.

- App: React Native / Expo (TypeScript) — ver `mobile/CLAUDE.md`
- Backend: C# / .NET (ASP.NET Core, .NET 10) — ver `backend/CLAUDE.md`
- Comunicação: API HTTP entre app e backend
- Prazo: 2 dias a partir do envio (não é necessário usar tudo)

O candidato é responsável pelo código entregue e precisa **entender e saber explicar** cada decisão, mesmo com uso de IA (que é esperado e incentivado). Simplicidade compreendida vale mais que sofisticação.

## Cenário

- Usuário possui carteira digital com **saldo inicial de R$ 50,00**.
- Serviços iniciais da lavanderia:
  - **Lavagem**: R$ 18,90
  - **Secagem**: R$ 20,90
- Pelo app, o usuário visualiza os serviços disponíveis e solicita a execução de um deles usando o saldo da carteira.
- Cabe a nós definir fluxo, API, dados e regras para a operação ser consistente (ex.: saldo insuficiente, débito atômico, evitar cobrança duplicada, tratamento de erros).

## O que é avaliado

Interpretação do problema, estrutura da solução, regras identificadas, decisões tomadas, integração app/backend, tratamento de situações inesperadas, o que foi testado e por quê, uso de IA, priorização de tempo e compreensão da solução.

Não buscamos complexidade desnecessária nem interface elaborada.

## Requisitos de entrega

- Repositório Git com app React Native, API .NET e testes relevantes.
- **Histórico de commits faz parte da avaliação**: commits pequenos e naturais, refletindo a evolução. Nunca entregar tudo em um único commit.
- **Testes automatizados** nos pontos mais relevantes (sem cobertura total). Registrar o que foi testado e por quê.
- **README** com instruções para executar, decisões importantes, limitações conhecidas, pendências e o que melhoraria com mais tempo. Se algo ficou de fora, explicar o que seria feito e por que foi priorizado outro ponto.
- APK Android: opcional (diferencial, ausência não prejudica).

## Estrutura do repositório

```text
cicclo-challenge/
├── mobile/     # App Expo (ver mobile/CLAUDE.md)
├── backend/    # API .NET (ver backend/CLAUDE.md)
└── global.json # .NET SDK 10.0.400+
```

## Estado atual

Estrutura, configurações e inicialização. O app tem o axios em `mobile/src/services/api.ts` e a tela inicial (MVVM) que lista carteira e serviços e solicita a execução de um serviço, e uma tela para adicionar saldo (Expo Router). O Domain já tem Wallet, LaundryService e ServiceExecution com testes xUnit. A API expõe `GET /services`, `GET /wallet`, `POST /services/{id}/execute` e `POST /wallet/deposit` (dados em memória, sem banco de dados). Ainda **não há**: autenticação e testes do app.

## Convenções de trabalho

- Responder e documentar em português (pt-BR); identificadores de código em inglês.
- Priorizar escopo mínimo coerente; evitar complexidade desnecessária e abstrações especulativas.
- Fazer commits pequenos e frequentes, com mensagens que descrevam a evolução natural.
- Registrar no README, ao longo do desenvolvimento, decisões, limitações e melhorias futuras.
- Manter o README como fonte de verdade para execução do projeto; atualizar os CLAUDE.md se a estrutura ou os comandos mudarem.
