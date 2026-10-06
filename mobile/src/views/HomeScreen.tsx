import { StatusBar } from 'expo-status-bar';
import { Text, View } from 'react-native';

import { styles } from './HomeScreen.styles';

export default function HomeScreen() {
  return (
    <View style={styles.container}>
      <Text style={styles.title}>Hello World</Text>
      <StatusBar style="dark" />
    </View>
  );
}
