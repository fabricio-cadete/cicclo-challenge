import Toast from 'react-native-toast-message';

const SUCCESS_DURATION_MS = 3000;

export function notifySuccess(message: string) {
  Toast.show({ type: 'success', text1: message, visibilityTime: SUCCESS_DURATION_MS });
}
