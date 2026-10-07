import { useRouter } from 'expo-router';
import { Text, TextInput, View } from 'react-native';

import { Button } from '../components/Button';
import { useAddBalanceViewModel } from '../viewmodels/useAddBalanceViewModel';
import { styles } from './AddBalanceScreen.styles';

export default function AddBalanceScreen() {
  const router = useRouter();
  const { amountText, setAmountText, canSubmit, submitting, submit } = useAddBalanceViewModel();

  async function handleSubmit() {
    if (await submit()) router.back();
  }

  return (
    <View style={styles.container}>
      <Text style={styles.label}>Valor (R$)</Text>
      <TextInput
        style={styles.input}
        value={amountText}
        onChangeText={setAmountText}
        placeholder="0,00"
        keyboardType="decimal-pad"
        editable={!submitting}
        onSubmitEditing={handleSubmit}
        autoFocus
      />
      <Button
        title={submitting ? 'Processando...' : 'Adicionar saldo'}
        onPress={handleSubmit}
        disabled={!canSubmit}
      />
    </View>
  );
}
