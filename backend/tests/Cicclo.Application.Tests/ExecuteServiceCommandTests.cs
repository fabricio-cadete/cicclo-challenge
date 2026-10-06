using Cicclo.Application.Abstractions;
using Cicclo.Application.Commands;
using Cicclo.Application.Exceptions;
using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Application.Tests;

public class ExecuteServiceCommandTests
{
    private class FakeServices(params LaundryService[] services) : ILaundryServiceRepository
    {
        public Task<IReadOnlyList<LaundryService>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<LaundryService>>(services);

        public Task<LaundryService?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(services.FirstOrDefault(s => s.Id == id));
    }

    private class FakeWallets(Wallet wallet) : IWalletRepository
    {
        public Task<Wallet> GetAsync(CancellationToken cancellationToken = default) => Task.FromResult(wallet);
    }

    private class FakeExecutions : IServiceExecutionRepository
    {
        public List<ServiceExecution> Items { get; } = [];

        public Task<ServiceExecution?> GetByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default) =>
            Task.FromResult(Items.FirstOrDefault(e => e.RequestId == requestId));

        public Task AddAsync(ServiceExecution execution, CancellationToken cancellationToken = default)
        {
            Items.Add(execution);
            return Task.CompletedTask;
        }
    }

    private readonly LaundryService _wash = LaundryService.Create("Lavagem", ServiceType.Wash, 18.90m);
    private readonly LaundryService _dry = LaundryService.Create("Secagem", ServiceType.Dry, 20.90m);
    private readonly Wallet _wallet = Wallet.Create();
    private readonly FakeExecutions _executions = new();

    private ExecuteServiceCommandHandler CreateHandler() =>
        new(new FakeServices(_wash, _dry), new FakeWallets(_wallet), _executions);

    [Fact]
    public async Task Handle_DebitsWalletAndRecordsExecution()
    {
        var result = await CreateHandler().Handle(new ExecuteServiceCommand(_wash.Id, Guid.NewGuid()));

        Assert.Equal(31.10m, _wallet.Balance);
        Assert.Equal(31.10m, result.WalletBalance);
        Assert.Equal(18.90m, result.Price);
        Assert.Equal(ExecutionStatus.Requested, result.Status);
        Assert.Single(_executions.Items);
    }

    [Fact]
    public async Task Handle_InsufficientBalance_ThrowsWithoutDebitOrRecord()
    {
        var handler = CreateHandler();
        await handler.Handle(new ExecuteServiceCommand(_wash.Id, Guid.NewGuid()));
        await handler.Handle(new ExecuteServiceCommand(_dry.Id, Guid.NewGuid()));

        // saldo restante: 10,20
        await Assert.ThrowsAsync<InsufficientBalanceException>(
            () => handler.Handle(new ExecuteServiceCommand(_wash.Id, Guid.NewGuid())));

        Assert.Equal(10.20m, _wallet.Balance);
        Assert.Equal(2, _executions.Items.Count);
    }

    [Fact]
    public async Task Handle_SameRequestIdTwice_ChargesOnlyOnce()
    {
        var handler = CreateHandler();
        var command = new ExecuteServiceCommand(_wash.Id, Guid.NewGuid());

        var first = await handler.Handle(command);
        var second = await handler.Handle(command);

        Assert.Equal(first.Id, second.Id);
        Assert.Equal(31.10m, _wallet.Balance);
        Assert.Single(_executions.Items);
    }

    [Fact]
    public async Task Handle_SameRequestIdForAnotherService_ThrowsConflict()
    {
        var handler = CreateHandler();
        var requestId = Guid.NewGuid();
        await handler.Handle(new ExecuteServiceCommand(_wash.Id, requestId));

        await Assert.ThrowsAsync<RequestIdConflictException>(
            () => handler.Handle(new ExecuteServiceCommand(_dry.Id, requestId)));

        Assert.Equal(31.10m, _wallet.Balance);
    }

    [Fact]
    public async Task Handle_UnknownService_ThrowsNotFound()
    {
        await Assert.ThrowsAsync<ServiceNotFoundException>(
            () => CreateHandler().Handle(new ExecuteServiceCommand(Guid.NewGuid(), Guid.NewGuid())));

        Assert.Equal(50.00m, _wallet.Balance);
    }

    [Fact]
    public async Task Handle_EmptyRequestId_ThrowsWithoutDebit()
    {
        await Assert.ThrowsAsync<DomainException>(
            () => CreateHandler().Handle(new ExecuteServiceCommand(_wash.Id, Guid.Empty)));

        Assert.Equal(50.00m, _wallet.Balance);
        Assert.Empty(_executions.Items);
    }

    [Fact]
    public async Task Handle_ConcurrentRequests_NeverOverdrawsWallet()
    {
        var handler = CreateHandler();

        // 10 solicitações simultâneas de R$ 18,90 com saldo de R$ 50,00: só 2 cabem.
        var tasks = Enumerable.Range(0, 10)
            .Select(_ => Task.Run(() => handler.Handle(new ExecuteServiceCommand(_wash.Id, Guid.NewGuid()))))
            .ToList();

        var results = await Task.WhenAll(tasks.Select(async t =>
        {
            try { await t; return true; }
            catch (InsufficientBalanceException) { return false; }
        }));

        Assert.Equal(2, results.Count(ok => ok));
        Assert.Equal(2, _executions.Items.Count);
        Assert.Equal(12.20m, _wallet.Balance);
    }
}
