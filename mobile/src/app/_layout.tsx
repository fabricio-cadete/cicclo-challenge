import { Stack } from 'expo-router';
import Toast from 'react-native-toast-message';

import { colors } from '../theme/colors';

export default function RootLayout() {
  return (
    <>
      <Stack screenOptions={{ headerTintColor: colors.primary }}>
        <Stack.Screen name="index" options={{ headerShown: false }} />
        <Stack.Screen name="add-balance" options={{ title: 'Adicionar saldo' }} />
      </Stack>
      <Toast />
    </>
  );
}
