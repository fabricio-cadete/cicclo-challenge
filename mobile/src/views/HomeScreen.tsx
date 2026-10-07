import { StatusBar } from 'expo-status-bar';
import { FlatList, Text, View } from 'react-native';

import { Button } from '../components/Button';
import { ConfirmDialog } from '../components/ConfirmDialog';
import { Loading } from '../components/Loading';
import { useHomeViewModel } from '../viewmodels/useHomeViewModel';
import { formatCurrency } from '../utils/formatCurrency';
import { styles } from './HomeScreen.styles';

export default function HomeScreen() {
  const {
    wallet,
    services,
    loading,
    error,
    reload,
    selectedService,
    executing,
    canExecute,
    selectService,
    cancelSelection,
    confirmExecution,
  } = useHomeViewModel();

  if (loading && !wallet) {
    return (
      <View style={styles.centered}>
        <Loading />
        <StatusBar style="dark" />
      </View>
    );
  }

  if (error && !wallet) {
    return (
      <View style={styles.centered}>
        <Text style={styles.errorText}>{error}</Text>
        <Button title="Tentar novamente" onPress={reload} />
        <StatusBar style="dark" />
      </View>
    );
  }

  return (
    <View style={styles.container}>
      <View style={styles.walletCard}>
        <Text style={styles.walletLabel}>Saldo da carteira</Text>
        <Text style={styles.walletBalance}>{formatCurrency(wallet?.balance ?? 0)}</Text>
      </View>

      <Text style={styles.sectionTitle}>Serviços disponíveis</Text>

      <FlatList
        data={services}
        keyExtractor={(service) => service.id}
        refreshing={loading}
        onRefresh={reload}
        ListEmptyComponent={<Text style={styles.emptyText}>Nenhum serviço disponível.</Text>}
        renderItem={({ item }) => (
          <View style={styles.serviceItem}>
            <View>
              <Text style={styles.serviceName}>{item.name}</Text>
              <Text style={styles.servicePrice}>{formatCurrency(item.price)}</Text>
            </View>
            <Button title="Utilizar" onPress={() => selectService(item)} disabled={!canExecute(item)} />
          </View>
        )}
      />

      <ConfirmDialog
        visible={selectedService !== null}
        title="Confirmar uso do serviço"
        message={
          selectedService
            ? `Utilizar "${selectedService.name}" por ${formatCurrency(selectedService.price)}? O valor será debitado da sua carteira.`
            : ''
        }
        loading={executing}
        onConfirm={confirmExecution}
        onCancel={cancelSelection}
      />

      <StatusBar style="dark" />
    </View>
  );
}
