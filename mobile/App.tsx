import Toast from 'react-native-toast-message';

import HomeScreen from './src/views/HomeScreen';

export default function App() {
  return (
    <>
      <HomeScreen />
      {/* Deve ser o último elemento para ficar por cima das telas. */}
      <Toast />
    </>
  );
}
