import { isAxiosError } from 'axios';
import { randomUUID } from 'expo-crypto';
import { useCallback, useEffect, useRef, useState } from 'react';

import { LaundryService } from '../models/LaundryService';
import { Wallet } from '../models/Wallet';
import { executeService, getServices } from '../services/laundryService';
import { getWallet } from '../services/walletService';
import { notifySuccess } from '../utils/notify';

function executionErrorMessage(error: unknown): string {
  const status = isAxiosError(error) ? error.response?.status : undefined;

  if (status === 422) return 'Saldo insuficiente para utilizar este serviço.';
  if (status === 404) return 'Este serviço não está mais disponível.';
  return 'Não foi possível utilizar o serviço. Tente novamente mais tarde.';
}

export function useHomeViewModel() {
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [services, setServices] = useState<LaundryService[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadCount, setReloadCount] = useState(0);

  // Serviço aguardando confirmação do usuário.
  const [selectedService, setSelectedService] = useState<LaundryService | null>(null);
  const [executing, setExecuting] = useState(false);
  // Erro da última execução; fica visível até a próxima ação (ErrorBanner). Sucesso vira toast.
  const [executionError, setExecutionError] = useState<string | null>(null);

  // requestId por serviço. Só é descartado quando a API responde (sucesso ou erro de negócio);
  // se faltar resposta (rede/timeout), a nova tentativa reaproveita o id e a API não cobra duas vezes.
  const requestIds = useRef(new Map<string, string>());

  useEffect(() => {
    // Evita atualizar o estado se a tela for desmontada ou um novo carregamento começar.
    let active = true;

    Promise.all([getWallet(), getServices()])
      .then(([walletData, servicesData]) => {
        if (!active) return;
        setWallet(walletData);
        setServices(servicesData);
        setError(null);
      })
      .catch(() => {
        if (!active) return;
        setError('Não foi possível carregar as informações. Tente novamente mais tarde.');
      })
      .finally(() => {
        if (active) setLoading(false);
      });

    return () => {
      active = false;
    };
  }, [reloadCount]);

  const reload = useCallback(() => {
    setLoading(true);
    setReloadCount((count) => count + 1);
  }, []);

  const selectService = useCallback((service: LaundryService) => {
    setExecutionError(null);
    setSelectedService(service);
  }, []);

  // Só desabilita o botão; quem garante a regra é a API (422 se o saldo for insuficiente).
  const canExecute = useCallback(
    (service: LaundryService) => !executing && (wallet?.balance ?? 0) >= service.price,
    [wallet, executing],
  );

  const cancelSelection = useCallback(() => setSelectedService(null), []);

  const confirmExecution = useCallback(async () => {
    if (!selectedService || executing) return;

    const service = selectedService;
    const requestId = requestIds.current.get(service.id) ?? randomUUID();
    requestIds.current.set(service.id, requestId);

    setExecuting(true);
    try {
      const execution = await executeService(service.id, requestId);
      requestIds.current.delete(service.id);
      setWallet((current) => (current ? { ...current, balance: execution.walletBalance } : current));
      notifySuccess(`Serviço "${service.name}" solicitado com sucesso!`);
    } catch (e) {
      if (isAxiosError(e) && e.response) requestIds.current.delete(service.id);
      setExecutionError(executionErrorMessage(e));
    } finally {
      setExecuting(false);
      setSelectedService(null);
    }
  }, [selectedService, executing]);

  return {
    wallet,
    services,
    loading,
    error,
    reload,
    selectedService,
    executing,
    executionError,
    canExecute,
    selectService,
    cancelSelection,
    confirmExecution,
  };
}
