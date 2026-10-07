import { userEvent, waitFor } from '@testing-library/react-native';
import { screen } from 'expo-router/testing-library';

import { api } from '../src/services/api';
import { formatCurrency } from '../src/utils/formatCurrency';
import { notifyError, notifySuccess } from '../src/utils/notify';
import { installFakeApi, networkError, WASH } from '../test-utils/fakeApi';
import { renderApp } from '../test-utils/renderApp';

jest.mock('../src/services/api', () => ({ api: { get: jest.fn(), post: jest.fn() } }));
jest.mock('../src/utils/notify');

const mockedApi = jest.mocked(api);

beforeEach(() => {
  jest.clearAllMocks();
});

describe('Home: carregamento', () => {
  it('mostra o saldo e os serviços com seus preços', async () => {
    installFakeApi(mockedApi);

    await renderApp();

    expect(await screen.findByText(formatCurrency(50))).toBeOnTheScreen();
    expect(screen.getByText('Lavagem')).toBeOnTheScreen();
    expect(screen.getByText(formatCurrency(18.9))).toBeOnTheScreen();
    expect(screen.getByText('Secagem')).toBeOnTheScreen();
    expect(screen.getByText(formatCurrency(20.9))).toBeOnTheScreen();
  });

  it('mostra erro amigável e carrega ao tentar novamente', async () => {
    const fake = installFakeApi(mockedApi);
    fake.failWith = networkError();
    const user = userEvent.setup();

    await renderApp();

    expect(await screen.findByText(/Tente novamente mais tarde/)).toBeOnTheScreen();

    fake.failWith = null;
    await user.press(screen.getByText('Tentar novamente'));

    expect(await screen.findByText(formatCurrency(50))).toBeOnTheScreen();
  });
});

describe('Home: utilizar serviço', () => {
  it('confirma, chama a API, atualiza o saldo e avisa o sucesso', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();

    await renderApp();
    await screen.findByText(formatCurrency(50));

    await user.press(screen.getAllByRole('button', { name: 'Utilizar' })[0]);
    expect(screen.getByText('Confirmar uso do serviço')).toBeOnTheScreen();

    await user.press(screen.getByRole('button', { name: 'Confirmar' }));

    expect(await screen.findByText(formatCurrency(31.1))).toBeOnTheScreen();
    expect(mockedApi.post).toHaveBeenCalledWith(`/services/${WASH.id}/execute`, {
      requestId: expect.any(String),
    });
    expect(notifySuccess).toHaveBeenCalledWith('Serviço "Lavagem" solicitado com sucesso!');
    expect(screen.queryByText('Confirmar uso do serviço')).not.toBeOnTheScreen();
  });

  it('não chama a API se o usuário cancelar a confirmação', async () => {
    installFakeApi(mockedApi);
    const user = userEvent.setup();

    await renderApp();
    await screen.findByText(formatCurrency(50));

    await user.press(screen.getAllByRole('button', { name: 'Utilizar' })[0]);
    await user.press(screen.getByRole('button', { name: 'Cancelar' }));

    expect(mockedApi.post).not.toHaveBeenCalled();
    expect(screen.getByText(formatCurrency(50))).toBeOnTheScreen();
  });

  it('desabilita "Utilizar" quando o saldo não cobre o preço', async () => {
    // 10,00 não cobre lavagem (18,90) nem secagem (20,90)
    installFakeApi(mockedApi, { balance: 10 });

    await renderApp();
    await screen.findByText(formatCurrency(10));

    for (const button of screen.getAllByRole('button', { name: 'Utilizar' })) {
      expect(button).toBeDisabled();
    }
  });

  it('desabilita só o serviço que o saldo não cobre', async () => {
    // 20,00 cobre a lavagem (18,90), mas não a secagem (20,90)
    installFakeApi(mockedApi, { balance: 20 });

    await renderApp();
    await screen.findByText(formatCurrency(20));

    const [wash, dry] = screen.getAllByRole('button', { name: 'Utilizar' });
    expect(wash).toBeEnabled();
    expect(dry).toBeDisabled();
  });

  it('mostra erro e mantém o saldo quando a API recusa por saldo insuficiente', async () => {
    // A tela mostra 50,00, mas a API já tem menos saldo (tela desatualizada)
    const fake = installFakeApi(mockedApi);
    const user = userEvent.setup();

    await renderApp();
    await screen.findByText(formatCurrency(50));
    fake.balance = 5;

    await user.press(screen.getAllByRole('button', { name: 'Utilizar' })[0]);
    await user.press(screen.getByRole('button', { name: 'Confirmar' }));

    expect(await screen.findByText(formatCurrency(50))).toBeOnTheScreen();
    expect(notifyError).toHaveBeenCalledWith('Saldo insuficiente para utilizar este serviço.');
    expect(notifySuccess).not.toHaveBeenCalled();
  });

  it('reaproveita o requestId depois de falha sem resposta, para não cobrar duas vezes', async () => {
    const fake = installFakeApi(mockedApi);
    const user = userEvent.setup();

    await renderApp();
    await screen.findByText(formatCurrency(50));

    // 1ª tentativa: sem conexão
    fake.failWith = networkError();
    await user.press(screen.getAllByRole('button', { name: 'Utilizar' })[0]);
    await user.press(screen.getByRole('button', { name: 'Confirmar' }));
    await waitFor(() =>
      expect(notifyError).toHaveBeenCalledWith('Não foi possível utilizar o serviço. Tente novamente mais tarde.'),
    );

    // 2ª tentativa: conexão voltou
    fake.failWith = null;
    await user.press(screen.getAllByRole('button', { name: 'Utilizar' })[0]);
    await user.press(screen.getByRole('button', { name: 'Confirmar' }));
    expect(await screen.findByText(formatCurrency(31.1))).toBeOnTheScreen();

    const requestIds = mockedApi.post.mock.calls.map(([, body]) => (body as { requestId: string }).requestId);
    expect(requestIds).toHaveLength(2);
    expect(requestIds[0]).toBe(requestIds[1]);
  });
});
