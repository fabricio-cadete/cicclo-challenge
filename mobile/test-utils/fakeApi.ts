import type { api } from '../src/services/api';

type Api = typeof api;

// API falsa com estado em memória, no mesmo contrato do backend:
// GET /wallet, GET /services, POST /services/{id}/execute e POST /wallet/deposit.
// Usada no lugar do axios para os testes passarem por ViewModel e services de verdade.

export const WASH = { id: 'wash-id', name: 'Lavagem', type: 'Wash', price: 18.9 };
export const DRY = { id: 'dry-id', name: 'Secagem', type: 'Dry', price: 20.9 };

function httpError(status: number) {
  // Mesmo formato que isAxiosError() reconhece.
  return { isAxiosError: true, response: { status } };
}

export function installFakeApi(mockedApi: jest.Mocked<Api>, options: { balance?: number } = {}) {
  const state = {
    balance: options.balance ?? 50,
    // Quando definido, todas as chamadas falham com esse erro (ex.: API fora do ar).
    failWith: null as unknown,
  };
  const services = [WASH, DRY];

  mockedApi.get.mockImplementation(async (url: string) => {
    if (state.failWith) throw state.failWith;
    if (url === '/wallet') return { data: { id: 'wallet-id', balance: state.balance } };
    if (url === '/services') return { data: services };
    throw httpError(404);
  });

  mockedApi.post.mockImplementation(async (url: string, body?: unknown) => {
    if (state.failWith) throw state.failWith;

    const execute = url.match(/^\/services\/(.+)\/execute$/);
    if (execute) {
      const service = services.find((s) => s.id === execute[1]);
      if (!service) throw httpError(404);
      if (state.balance < service.price) throw httpError(422);

      state.balance = Math.round((state.balance - service.price) * 100) / 100;
      return {
        data: {
          id: 'execution-id',
          serviceId: service.id,
          price: service.price,
          status: 'Requested',
          createdAt: new Date().toISOString(),
          walletBalance: state.balance,
        },
      };
    }

    if (url === '/wallet/deposit') {
      const { amount } = body as { amount: number };
      if (amount <= 0) throw httpError(400);

      state.balance = Math.round((state.balance + amount) * 100) / 100;
      return { data: { id: 'wallet-id', balance: state.balance } };
    }

    throw httpError(404);
  });

  return state;
}

export function networkError() {
  // Sem `response`: simula falta de conexão/timeout.
  return { isAxiosError: true, message: 'Network Error' };
}
