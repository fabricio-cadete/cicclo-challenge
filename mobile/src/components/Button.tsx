import { Pressable, Text } from 'react-native';

import { styles } from './Button.styles';

type ButtonProps = {
  title: string;
  onPress: () => void;
  variant?: 'primary' | 'secondary';
  disabled?: boolean;
};

export function Button({ title, onPress, variant = 'primary', disabled = false }: ButtonProps) {
  return (
    <Pressable
      accessibilityRole="button"
      disabled={disabled}
      onPress={onPress}
      style={({ pressed }) => [
        styles.button,
        styles[variant],
        pressed && styles.pressed,
        disabled && styles.disabled,
      ]}
    >
      <Text
        numberOfLines={1}
        adjustsFontSizeToFit
        style={[styles.text, variant === 'primary' ? styles.primaryText : styles.secondaryText]}
      >
        {title}
      </Text>
    </Pressable>
  );
}
