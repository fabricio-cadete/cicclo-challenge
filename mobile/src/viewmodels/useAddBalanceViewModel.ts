import { isAxiosError } from 'axios';
import { useCallback, useState } from 'react';

import { addBalance } from '../services/walletService';
import { formatCurrency } from '../utils/formatCurrency';
import { notifyError, notifySuccess } from '../utils/notify';
import { parseAmount } from '../utils/parseAmount';

function depositErrorMessage(error: unknown): string {
  const status = isAxiosError(error) ? error.response?.status : undefined;

  if (status === 400) return 'Informe um valor válido.';
  return 'Não foi possível adicionar saldo. Tente novamente mais tarde.';
}

export function useAddBalanceViewModel() {
  const [amountText, setAmountText] = useState('');
  const [submitting, setSubmitting] = useState(false);

  const amount = parseAmount(amountText);
  const canSubmit = amount !== null && !submitting;

  // Retorna true se o saldo foi adicionado (a View decide voltar para a tela anterior).
  const submit = useCallback(async () => {
    if (amount === null || submitting) return false;

    setSubmitting(true);
    try {
      await addBalance(amount);
      notifySuccess(`${formatCurrency(amount)} adicionados à sua carteira!`);
      return true;
    } catch (e) {
      notifyError(depositErrorMessage(e));
      return false;
    } finally {
      setSubmitting(false);
    }
  }, [amount, submitting]);

  return { amountText, setAmountText, canSubmit, submitting, submit };
}
