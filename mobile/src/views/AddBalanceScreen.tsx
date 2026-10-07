import { useRouter } from 'expo-router';
import { Text, TextInput, View } from 'react-native';

import { Button } from '../components/Button';
import { QUICK_AMOUNTS, useAddBalanceViewModel } from '../viewmodels/useAddBalanceViewModel';
import { styles } from './AddBalanceScreen.styles';

export default function AddBalanceScreen() {
  const router = useRouter();
  const { displayValue, changeText, selectQuickAmount, canSubmit, submitting, submit } =
    useAddBalanceViewModel();

  async function handleSubmit() {
    if (await submit()) router.back();
  }

  return (
    <View style={styles.container}>
      <Text style={styles.label}>Valor</Text>
      <TextInput
        style={styles.input}
        value={displayValue}
        onChangeText={changeText}
        placeholder="R$ 0,00"
        keyboardType="number-pad"
        editable={!submitting}
        onSubmitEditing={handleSubmit}
        autoFocus
      />

      <View style={styles.quickAmounts}>
        {QUICK_AMOUNTS.map((value) => (
          <View key={value} style={styles.quickAmount}>
            <Button
              title={`R$ ${value}`}
              variant="secondary"
              onPress={() => selectQuickAmount(value)}
              disabled={submitting}
            />
          </View>
        ))}
      </View>

      <Button
        title={submitting ? 'Processando...' : 'Adicionar saldo'}
        onPress={handleSubmit}
        disabled={!canSubmit}
      />
    </View>
  );
}
