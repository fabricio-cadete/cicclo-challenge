import { Stack } from 'expo-router';

import { AppToast } from '../components/AppToast';
import { colors } from '../theme/colors';

export default function RootLayout() {
  return (
    <>
      <Stack screenOptions={{ headerTintColor: colors.primary }}>
        <Stack.Screen name="index" options={{ headerShown: false }} />
        <Stack.Screen name="add-balance" options={{ title: 'Adicionar saldo' }} />
      </Stack>
      {/* Deve ser o último elemento para ficar por cima das telas. */}
      <AppToast />
    </>
  );
}
