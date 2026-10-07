import { Wallet } from '../models/Wallet';
import { api } from './api';

export async function getWallet(): Promise<Wallet> {
  const { data } = await api.get<Wallet>('/wallet');
  return data;
}
