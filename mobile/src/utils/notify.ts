import Toast from 'react-native-toast-message';

const SUCCESS_DURATION_MS = 3000;
const ERROR_DURATION_MS = 5000;

// Toast em duas linhas: título curto e, opcionalmente, um detalhe.
export function notifySuccess(title: string, detail?: string) {
  Toast.show({ type: 'success', text1: title, text2: detail, visibilityTime: SUCCESS_DURATION_MS });
}

export function notifyError(title: string, detail?: string) {
  Toast.show({ type: 'error', text1: title, text2: detail, visibilityTime: ERROR_DURATION_MS });
}
