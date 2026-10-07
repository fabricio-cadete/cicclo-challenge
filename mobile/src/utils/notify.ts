import Toast from 'react-native-toast-message';

const SUCCESS_DURATION_MS = 3000;
const ERROR_DURATION_MS = 5000;

export function notifySuccess(message: string) {
  Toast.show({ type: 'success', text1: message, visibilityTime: SUCCESS_DURATION_MS });
}

export function notifyError(message: string) {
  Toast.show({ type: 'error', text1: message, visibilityTime: ERROR_DURATION_MS });
}
