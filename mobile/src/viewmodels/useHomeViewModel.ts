import { useCallback, useEffect, useState } from 'react';

import { LaundryService } from '../models/LaundryService';
import { Wallet } from '../models/Wallet';
import { getServices } from '../services/laundryService';
import { getWallet } from '../services/walletService';

export function useHomeViewModel() {
  const [wallet, setWallet] = useState<Wallet | null>(null);
  const [services, setServices] = useState<LaundryService[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [reloadCount, setReloadCount] = useState(0);

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

  return { wallet, services, loading, error, reload };
}
