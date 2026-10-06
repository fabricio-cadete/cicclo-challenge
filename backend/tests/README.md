# Testes

- Cicclo.Domain.Tests: preparado com xUnit e referência a Domain, para testar invariantes e regras de negócio.
- Cicclo.Application.Tests: preparado com xUnit e referência a Application, para testar casos de uso e seus contratos.

Os testes vazios gerados pelo template foram removidos. Ainda não existem casos de teste: eles serão adicionados junto com os comportamentos. O comando `dotnet test backend/Cicclo.sln` valida a infraestrutura de execução, mas nesta etapa informa que não encontrou testes. Isso não representa cobertura de regras de negócio.
