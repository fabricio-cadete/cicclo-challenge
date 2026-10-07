import { Text, View } from 'react-native';

import { styles } from './ErrorBanner.styles';

type ErrorBannerProps = {
  message: string;
};

export function ErrorBanner({ message }: ErrorBannerProps) {
  return (
    <View accessibilityRole="alert" style={styles.banner}>
      <Text style={styles.text}>{message}</Text>
    </View>
  );
}
