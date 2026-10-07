import { LaundryService } from '../models/LaundryService';
import { api } from './api';

export async function getServices(): Promise<LaundryService[]> {
  const { data } = await api.get<LaundryService[]>('/services');
  return data;
}
