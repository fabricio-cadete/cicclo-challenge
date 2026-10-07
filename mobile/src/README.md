# Organização do app

- app: rotas do Expo Router, apenas reexportam as views.
- views: telas e apresentação; a View só lê o estado do ViewModel e dispara ações.
- viewmodels: hooks com estado e ações das telas (MVVM).
- models: tipos e contratos de dados.
- services: comunicação HTTP e integrações.
- components: componentes visuais reutilizáveis.

Hoje existe a tela inicial (`HomeScreen`) com `useHomeViewModel`, que carrega a carteira e os serviços e executa um serviço (botão "Utilizar", confirmação em `ConfirmDialog`, chamada `POST /services/{id}/execute`, saldo atualizado com a resposta, sucesso e erro em toast). O `requestId` (UUID por tentativa) evita cobrança duplicada em novas tentativas sem resposta da API. A tela `AddBalanceScreen` (`useAddBalanceViewModel`) adiciona saldo via `POST /wallet/deposit`: o campo guarda o valor em centavos e aplica máscara de moeda enquanto o usuário digita (teclado numérico), há atalhos de R$ 20, 50 e 100 e o botão só habilita com valor maior que zero (a validação real é da API); ao concluir, volta para a Home, que recarrega o saldo. A navegação usa Expo Router, com rotas em src/app, conforme AGENTS.md.

## Estilos

Mantenha os estilos de cada tela ou componente em um arquivo ao lado dele, usando o sufixo .styles.ts. Por exemplo: views/HomeScreen.tsx importa styles de views/HomeScreen.styles.ts. O arquivo de estilos exporta o resultado de StyleSheet.create.
