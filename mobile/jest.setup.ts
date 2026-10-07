// randomUUID é nativo; nos testes usamos um contador previsível.
// (O prefixo "mock" é exigido pelo Jest para variáveis usadas dentro de jest.mock.)
let mockUuidCount = 0;

jest.mock('expo-crypto', () => ({
  randomUUID: () => `request-${++mockUuidCount}`,
}));
