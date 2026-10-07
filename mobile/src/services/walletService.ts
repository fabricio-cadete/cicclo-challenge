import { Wallet } from '../models/Wallet';
import { api } from './api';

export async function getWallet(): Promise<Wallet> {
  const { data } = await api.get<Wallet>('/wallet');
  return data;
}

export async function addBalance(amount: number): Promise<Wallet> {
  const { data } = await api.post<Wallet>('/wallet/deposit', { amount });
  return data;
}
