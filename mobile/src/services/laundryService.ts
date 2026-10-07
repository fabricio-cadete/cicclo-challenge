import { LaundryService } from '../models/LaundryService';
import { ServiceExecution } from '../models/ServiceExecution';
import { api } from './api';

export async function getServices(): Promise<LaundryService[]> {
  const { data } = await api.get<LaundryService[]>('/services');
  return data;
}

// requestId identifica a tentativa: repetir o mesmo valor não cobra de novo.
export async function executeService(serviceId: string, requestId: string): Promise<ServiceExecution> {
  const { data } = await api.post<ServiceExecution>(`/services/${serviceId}/execute`, { requestId });
  return data;
}
