import { create } from 'axios';
import { Platform } from 'react-native';

// O emulador Android acessa o host pelo IP especial 10.0.2.2 (localhost aponta para o próprio emulador).
// Em dispositivo físico, defina EXPO_PUBLIC_API_URL com o IP da máquina, ex.: http://192.168.0.10:5080
const defaultHost = Platform.OS === 'android' ? '10.0.2.2' : 'localhost';

export const BASE_URL = process.env.EXPO_PUBLIC_API_URL ?? `http://${defaultHost}:5080`;

export const api = create({
  baseURL: BASE_URL,
  timeout: 10000,
  headers: { 'Content-Type': 'application/json' },
});
