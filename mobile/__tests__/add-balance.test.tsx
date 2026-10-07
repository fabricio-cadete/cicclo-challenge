import { userEvent } from '@testing-library/react-native';
import { screen } from 'expo-router/testing-library';

import { api } from '../src/services/api';
import { formatCurrency } from '../src/utils/formatCurrency';
import { notifyError, notifySuccess } from '../src/utils/notify';
import { installFakeApi, networkError } from '../test-utils/fakeApi';
import { renderApp } from '../test-utils/renderApp';

jest.mock('../src/services/api', () => ({ api: { get: jest.fn(), post: jest.fn() } }));
jest.mock('../src/utils/notify');

const mockedApi = jest.mocked(api);

async function openAddBalance(user: ReturnType<typeof userEvent.setup>) {
  await renderApp();
  await screen.findByText(formatCurrency(50));
  await user.press(screen.getByRole('button', { name: 'Adicionar saldo' }));
  return screen.findByPlaceholderText('R$ 0,00');
}

beforeEach(() => {
  jest.clearAllMocks();
});

describe('Adicionar saldo', () => {
  it('adiciona o valor digitado, avisa o sucesso e volta para a Home com o saldo novo', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();
    const input = await openAddBalance(user);

    await user.type(input, '2550');
    expect(input).toHaveDisplayValue(formatCurrency(25.5));

    await user.press(screen.getByRole('button', { name: 'Adicionar saldo' }));

    expect(mockedApi.post).toHaveBeenCalledWith('/wallet/deposit', { amount: 25.5 });
    expect(notifySuccess).toHaveBeenCalledWith('Saldo adicionado com sucesso!');
    expect(await screen.findByText(formatCurrency(75.5))).toBeOnTheScreen();

    // voltou para a Home: a tela de adicionar saldo não está mais visível
    expect(screen.queryByPlaceholderText('R$ 0,00')).not.toBeOnTheScreen();
  });

  it('preenche o valor pelos atalhos', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();
    const input = await openAddBalance(user);

    await user.press(screen.getByRole('button', { name: 'R$ 50' }));
    expect(input).toHaveDisplayValue(formatCurrency(50));

    await user.press(screen.getByRole('button', { name: 'Adicionar saldo' }));

    expect(mockedApi.post).toHaveBeenCalledWith('/wallet/deposit', { amount: 50 });
    expect(await screen.findByText(formatCurrency(100))).toBeOnTheScreen();
  });

  it('não permite adicionar com o campo vazio ou valor zero', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();
    const input = await openAddBalance(user);

    expect(screen.getByRole('button', { name: 'Adicionar saldo' })).toBeDisabled();

    await user.type(input, '000');
    expect(input).toHaveDisplayValue('');
    expect(screen.getByRole('button', { name: 'Adicionar saldo' })).toBeDisabled();
    expect(mockedApi.post).not.toHaveBeenCalled();
  });

  it('ignora letras e símbolos digitados', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();
    const input = await openAddBalance(user);

    await user.type(input, 'abc-1,5x');

    // só os dígitos "15" valem: R$ 0,15
    expect(input).toHaveDisplayValue(formatCurrency(0.15));
  });

  it('mostra erro e permanece na tela quando a API falha', async () => {
    const fake = installFakeApi(mockedApi);
    const user = userEvent.setup();
    const input = await openAddBalance(user);

    await user.type(input, '1000');
    fake.failWith = networkError();
    await user.press(screen.getByRole('button', { name: 'Adicionar saldo' }));

    expect(await screen.findByPlaceholderText('R$ 0,00')).toBeOnTheScreen();
    expect(notifyError).toHaveBeenCalledWith('Não foi possível adicionar saldo. Tente novamente mais tarde.');
    expect(notifySuccess).not.toHaveBeenCalled();
  });
});
