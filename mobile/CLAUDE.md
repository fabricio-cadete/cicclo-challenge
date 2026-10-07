# CLAUDE.md — Mobile

Contexto específico do app Expo. O contexto global do projeto está em `../CLAUDE.md`. Regras gerais de Expo/Router/EAS estão em `AGENTS.md`.

## Stack

Expo SDK 57, React Native 0.86, TypeScript 6. Arquitetura MVVM.

```text
src/
├── app/         # rotas do Expo Router (só reexportam as views)
├── views/       # telas e apresentação
├── viewmodels/  # hooks com estado e ações das telas
├── models/      # tipos e contratos de dados
├── services/    # comunicação HTTP e integrações
├── components/  # componentes reutilizáveis (Button, Loading, ConfirmDialog)
├── utils/       # funções auxiliares (ex.: formatCurrency)
└── theme/       # cores e tokens visuais (colors.ts)
```

## Comandos

Em `mobile/`:

```sh
npm ci
npm start            # Expo Go ou emulador Android (tecla a)
npm run web          # prévia no navegador
npm run typecheck
npm test             # testes (Jest + React Native Testing Library)
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

## Testes

- Jest (`jest-expo`) + React Native Testing Library, em `mobile/__tests__/` (nunca dentro de `src/app/`). `npm test` roda tudo.
- As telas são renderizadas com `renderRouter` (Expo Router) e a rede é substituída por uma API falsa com estado (`test-utils/fakeApi.ts`), então os testes passam por View, ViewModel e services de verdade. O `utils/notify` é mockado para checar os avisos.
- Poucos testes, nos fluxos principais: carregar a Home, utilizar serviço (com saldo, cancelar, API recusa, nova tentativa com o mesmo `requestId`), botão desabilitado sem saldo e adicionar saldo (valor digitado, atalhos, valor inválido, falha da API).
- Helpers de teste ficam em `test-utils/`, fora de `__tests__/` (senão o Jest os trata como testes).

## Feedback ao usuário

- Sucesso e erro de operações: toast (`react-native-toast-message`) via `utils/notify.ts` (`notifySuccess(título, detalhe?)` e `notifyError(título, detalhe?)`), sempre em duas linhas: título curto (ex.: "Secagem solicitada com sucesso!") e detalhe. O `<AppToast />` (`components/AppToast.tsx`) fica em `src/app/_layout.tsx`, como último elemento, e afasta o toast do topo usando a safe area. Erros ficam mais tempo na tela (5 s) que sucessos (3 s).
- Mensagens amigáveis, sem termos técnicos. Falha ao carregar a tela inicial mostra o texto de erro com o botão "Tentar novamente".
- Carregamento: spinner (`Loading`) ao carregar a tela; durante uma operação, o botão fica desabilitado e mostra "Processando...".

## Navegação

Expo Router com rotas em `src/app/` (`_layout.tsx` define a Stack e o `<Toast />`; `index.tsx` é a Home e `add-balance.tsx` a tela de adicionar saldo). Os arquivos de rota só reexportam a tela de `views/`; não colocar componentes ou lógica em `src/app/`. Nomes de arquivo em kebab-case. A Home recarrega carteira e serviços sempre que volta ao foco (`useFocusEffect`).

## Convenções

- Estado e ações das telas ficam em viewmodels (hooks); views só apresentam.
- Interface simples; não é critério de avaliação.
