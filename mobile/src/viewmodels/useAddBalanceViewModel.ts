import { isAxiosError } from 'axios';
import { useCallback, useState } from 'react';

import { addBalance } from '../services/walletService';
import { formatCurrency } from '../utils/formatCurrency';
import { notifyError, notifySuccess } from '../utils/notify';

export const QUICK_AMOUNTS = [20, 50, 100];

// Limite só para o campo não estourar; a validação de verdade é da API.
const MAX_CENTS = 99_999_999;

function depositError(error: unknown): { title: string; detail: string } {
  const status = isAxiosError(error) ? error.response?.status : undefined;

  if (status === 400) {
    return { title: 'Valor inválido', detail: 'Informe um valor maior que zero.' };
  }
  return { title: 'Não foi possível adicionar saldo', detail: 'Tente novamente mais tarde.' };
}

export function useAddBalanceViewModel() {
  // O valor é guardado em centavos: o usuário digita só números e a máscara de moeda
  // (R$ 25,50) é aplicada na exibição. Evita erros de ponto flutuante e de separador.
  const [cents, setCents] = useState(0);
  const [submitting, setSubmitting] = useState(false);

  const amount = cents / 100;
  const displayValue = cents > 0 ? formatCurrency(amount) : '';
  const canSubmit = cents > 0 && !submitting;

  const changeText = useCallback((text: string) => {
    const digits = text.replace(/\D/g, '');
    setCents(Math.min(Number(digits || '0'), MAX_CENTS));
  }, []);

  const selectQuickAmount = useCallback((value: number) => setCents(value * 100), []);

  // Retorna true se o saldo foi adicionado (a View decide voltar para a tela anterior).
  const submit = useCallback(async () => {
    if (cents <= 0 || submitting) return false;

    setSubmitting(true);
    try {
      await addBalance(amount);
      notifySuccess('Saldo adicionado com sucesso!', `${formatCurrency(amount)} na sua carteira.`);
      return true;
    } catch (e) {
      const { title, detail } = depositError(e);
      notifyError(title, detail);
      return false;
    } finally {
      setSubmitting(false);
    }
  }, [cents, amount, submitting]);

  return { displayValue, changeText, selectQuickAmount, canSubmit, submitting, submit };
}
