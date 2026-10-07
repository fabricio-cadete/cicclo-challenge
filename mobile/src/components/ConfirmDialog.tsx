import { Modal, Text, View } from 'react-native';

import { Button } from './Button';
import { styles } from './ConfirmDialog.styles';

type ConfirmDialogProps = {
  visible: boolean;
  title: string;
  message: string;
  confirmLabel?: string;
  cancelLabel?: string;
  loading?: boolean;
  onConfirm: () => void;
  onCancel: () => void;
};

export function ConfirmDialog({
  visible,
  title,
  message,
  confirmLabel = 'Confirmar',
  cancelLabel = 'Cancelar',
  loading = false,
  onConfirm,
  onCancel,
}: ConfirmDialogProps) {
  return (
    <Modal visible={visible} transparent animationType="fade" onRequestClose={onCancel}>
      <View style={styles.overlay}>
        <View style={styles.card}>
          <Text style={styles.title}>{title}</Text>
          <Text style={styles.message}>{message}</Text>
          <View style={styles.actions}>
            <Button title={loading ? 'Processando...' : confirmLabel} onPress={onConfirm} disabled={loading} />
            <Button title={cancelLabel} onPress={onCancel} variant="secondary" disabled={loading} />
          </View>
        </View>
      </View>
    </Modal>
  );
}
