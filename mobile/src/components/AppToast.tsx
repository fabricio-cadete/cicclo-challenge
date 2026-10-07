import { useSafeAreaInsets } from 'react-native-safe-area-context';
import Toast, { BaseToast, ErrorToast, ToastConfig } from 'react-native-toast-message';

import { colors } from '../theme/colors';

// Distância extra entre o toast e a borda da área segura (notch/barra de status).
const TOP_MARGIN = 16;

const toastConfig: ToastConfig = {
  success: (props) => (
    <BaseToast {...props} style={{ borderLeftColor: colors.primary }} text2NumberOfLines={2} />
  ),
  error: (props) => <ErrorToast {...props} text2NumberOfLines={2} />,
};

export function AppToast() {
  const insets = useSafeAreaInsets();

  return <Toast config={toastConfig} topOffset={insets.top + TOP_MARGIN} />;
}
