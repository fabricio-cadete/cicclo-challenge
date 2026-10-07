import { renderRouter } from 'expo-router/testing-library';

import AddBalanceScreen from '../src/views/AddBalanceScreen';
import HomeScreen from '../src/views/HomeScreen';

// Renderiza as duas telas com a navegação do Expo Router (em memória), como no app.
export function renderApp() {
  return renderRouter({
    index: HomeScreen,
    'add-balance': AddBalanceScreen,
  });
}
