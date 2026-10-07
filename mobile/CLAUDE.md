# CLAUDE.md — Mobile

Contexto específico do app Expo. O contexto global do projeto está em `../CLAUDE.md`. Regras gerais de Expo/Router/EAS estão em `AGENTS.md`.

## Stack

Expo SDK 57, React Native 0.86, TypeScript 6. Arquitetura MVVM.

```text
src/
├── views/       # telas e apresentação
├── viewmodels/  # hooks com estado e ações das telas
├── models/      # tipos e contratos de dados
├── services/    # comunicação HTTP e integrações
├── components/  # componentes visuais reutilizáveis
└── theme/       # cores e tokens visuais (colors.ts)
```

## Comandos

Em `mobile/`:

```sh
npm ci
npm start            # Expo Go ou emulador Android (tecla a)
npm run web          # prévia no navegador
npm run typecheck
npm run lint
npx expo-doctor
npm run export:check # valida bundles JS; não gera APK/IPA
```

Pré-requisitos: Node 22.13+ (24 LTS usado), Expo Go compatível com SDK 57 ou emulador Android. No Windows não há emulador iOS.
Instalar dependências com `npx expo install <pacote>` (resolve versões compatíveis com o SDK). Rodar `typecheck` e `lint` antes de concluir uma tarefa.

## Requisições HTTP

- **Sempre** usar a instância `api` exportada por `src/services/api.ts` (axios com `baseURL` configurada). Não usar `fetch` nem criar outra instância do axios.
- A URL base vem de `EXPO_PUBLIC_API_URL`; sem ela, usa `10.0.2.2:5080` no Android e `localhost:5080` nos demais. Em dispositivo físico, definir a variável com o IP da máquina.
- Funções de chamada à API ficam em `src/services/` (um arquivo por recurso, ex.: `walletService.ts`), consumindo `api`. Views não chamam o axios diretamente; passam por viewmodels.
- Tipos de request/response ficam em `src/models/`.

## Estilos

- Cada tela ou componente tem os estilos em **arquivo próprio** ao lado dele, com sufixo `.styles.ts` (ex.: `views/HomeScreen.tsx` importa de `views/HomeScreen.styles.ts`).
- O arquivo de estilos exporta o resultado de `StyleSheet.create`.
- Não declarar `StyleSheet.create` dentro do componente/tela.

## Tema

- A cor primária do app é `#634A72`, definida em `src/theme/colors.ts` (`colors.primary`).
- Estilos importam as cores de `theme/colors.ts`; não repetir hexadecimais nos arquivos `.styles.ts`.

## Navegação

Quando houver navegação, usar Expo Router com rotas em `src/app/` (ver `AGENTS.md`). Ainda não há navegação nesta etapa.

## Convenções

- Estado e ações das telas ficam em viewmodels (hooks); views só apresentam.
- Interface simples; não é critério de avaliação.
