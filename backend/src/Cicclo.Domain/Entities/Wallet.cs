using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Entities;

public class Wallet
{
    public const decimal InitialBalance = 50.00m;

    // A carteira é compartilhada entre requisições (débito e depósito simultâneos),
    // então cada operação sobre o saldo é feita sob lock.
    private readonly object _lock = new();
    private decimal _balance;

    public Guid Id { get; private set; }

    public decimal Balance
    {
        get { lock (_lock) return _balance; }
    }

    private Wallet() { }

    public static Wallet Create(decimal initialBalance = InitialBalance)
    {
        if (initialBalance < 0 || HasMoreThanTwoDecimals(initialBalance))
            throw new InvalidAmountException(initialBalance);

        return new Wallet { Id = Guid.NewGuid(), _balance = initialBalance };
    }

    public void Debit(decimal amount)
    {
        EnsureValidAmount(amount);

        lock (_lock)
        {
            if (amount > _balance)
                throw new InsufficientBalanceException(_balance, amount);

            _balance -= amount;
        }
    }

    public void Credit(decimal amount)
    {
        EnsureValidAmount(amount);

        lock (_lock)
        {
            _balance += amount;
        }
    }

    internal static void EnsureValidAmount(decimal amount)
    {
        if (amount <= 0 || HasMoreThanTwoDecimals(amount))
            throw new InvalidAmountException(amount);
    }

    private static bool HasMoreThanTwoDecimals(decimal value) => decimal.Round(value, 2) != value;
}
