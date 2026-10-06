# Organização do app

- views: telas e apresentação; HomeScreen contém somente a tela inicial.
- viewmodels: espaço reservado para hooks com estado e ações das telas (MVVM).
- models: tipos e contratos de dados.
- services: comunicação HTTP e integrações.
- components: componentes visuais reutilizáveis.

Ainda não há ViewModels, chamadas HTTP ou regras de negócio. Não há navegação entre telas nesta etapa. Ao adicionar navegação, usar Expo Router com rotas em src/app, conforme AGENTS.md.

## Estilos

Mantenha os estilos de cada tela ou componente em um arquivo ao lado dele, usando o sufixo .styles.ts. Por exemplo: views/HomeScreen.tsx importa styles de views/HomeScreen.styles.ts. O arquivo de estilos exporta o resultado de StyleSheet.create.
