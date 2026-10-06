using Cicclo.Domain.Entities;
using Cicclo.Domain.Enums;
using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Tests;

public class ServiceExecutionTests
{
    private static LaundryService Wash() => LaundryService.Create("Lavagem", ServiceType.Wash, 18.90m);
    private static LaundryService Dry() => LaundryService.Create("Secagem", ServiceType.Dry, 20.90m);

    [Fact]
    public void Request_DebitsWalletAndRecordsPrice()
    {
        var wallet = Wallet.Create();
        var service = Wash();

        var execution = ServiceExecution.Request(wallet, service, Guid.NewGuid());

        Assert.Equal(31.10m, wallet.Balance);
        Assert.Equal(18.90m, execution.Price);
        Assert.Equal(ExecutionStatus.Requested, execution.Status);
        Assert.Equal(wallet.Id, execution.WalletId);
        Assert.Equal(service.Id, execution.ServiceId);
    }

    [Fact]
    public void Request_InsufficientBalance_Throws()
    {
        var wallet = Wallet.Create();
        ServiceExecution.Request(wallet, Wash(), Guid.NewGuid());
        ServiceExecution.Request(wallet, Dry(), Guid.NewGuid());

        // saldo restante: 10,20
        Assert.Throws<InsufficientBalanceException>(() => ServiceExecution.Request(wallet, Wash(), Guid.NewGuid()));
        Assert.Equal(10.20m, wallet.Balance);
    }

    [Fact]
    public void Request_EmptyRequestId_ThrowsWithoutDebit()
    {
        var wallet = Wallet.Create();

        Assert.Throws<DomainException>(() => ServiceExecution.Request(wallet, Wash(), Guid.Empty));
        Assert.Equal(50.00m, wallet.Balance);
    }

    [Fact]
    public void Fail_RefundsWallet()
    {
        var wallet = Wallet.Create();
        var execution = ServiceExecution.Request(wallet, Wash(), Guid.NewGuid());

        execution.Fail(wallet);

        Assert.Equal(50.00m, wallet.Balance);
        Assert.Equal(ExecutionStatus.Failed, execution.Status);
    }

    [Fact]
    public void Fail_AfterComplete_ThrowsAndDoesNotRefund()
    {
        var wallet = Wallet.Create();
        var execution = ServiceExecution.Request(wallet, Wash(), Guid.NewGuid());
        execution.Complete();

        Assert.Throws<InvalidExecutionStateException>(() => execution.Fail(wallet));
        Assert.Equal(31.10m, wallet.Balance);
    }
}
