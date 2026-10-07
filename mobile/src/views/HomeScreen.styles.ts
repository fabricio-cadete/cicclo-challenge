import { StyleSheet } from 'react-native';

import { colors } from '../theme/colors';

export const styles = StyleSheet.create({
  container: {
    flex: 1,
    alignItems: 'center',
    justifyContent: 'center',
    backgroundColor: '#F6F8FA',
    padding: 24,
  },
  title: {
    fontSize: 36,
    fontWeight: '700',
    color: colors.primary,
  },
  subtitle: {
    marginTop: 12,
    fontSize: 16,
    textAlign: 'center',
    color: '#455465',
  },
});
