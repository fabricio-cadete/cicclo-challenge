# Organização do app

- views: telas e apresentação; a View só lê o estado do ViewModel e dispara ações.
- viewmodels: hooks com estado e ações das telas (MVVM).
- models: tipos e contratos de dados.
- services: comunicação HTTP e integrações.
- components: componentes visuais reutilizáveis.

Hoje existe a tela inicial (`HomeScreen`) com `useHomeViewModel`, que carrega a carteira e os serviços pelos services `walletService` e `laundryService`. Não há navegação entre telas nesta etapa. Ao adicionar navegação, usar Expo Router com rotas em src/app, conforme AGENTS.md.

## Estilos

Mantenha os estilos de cada tela ou componente em um arquivo ao lado dele, usando o sufixo .styles.ts. Por exemplo: views/HomeScreen.tsx importa styles de views/HomeScreen.styles.ts. O arquivo de estilos exporta o resultado de StyleSheet.create.
