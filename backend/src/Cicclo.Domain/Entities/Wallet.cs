using Cicclo.Domain.Exceptions;

namespace Cicclo.Domain.Entities;

public class Wallet
{
    public const decimal InitialBalance = 50.00m;

    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }

    private Wallet() { }

    public static Wallet Create(decimal initialBalance = InitialBalance)
    {
        if (initialBalance < 0 || HasMoreThanTwoDecimals(initialBalance))
            throw new InvalidAmountException(initialBalance);

        return new Wallet { Id = Guid.NewGuid(), Balance = initialBalance };
    }

    public void Debit(decimal amount)
    {
        EnsureValidAmount(amount);

        if (amount > Balance)
            throw new InsufficientBalanceException(Balance, amount);

        Balance -= amount;
    }

    public void Credit(decimal amount)
    {
        EnsureValidAmount(amount);
        Balance += amount;
    }

    internal static void EnsureValidAmount(decimal amount)
    {
        if (amount <= 0 || HasMoreThanTwoDecimals(amount))
            throw new InvalidAmountException(amount);
    }

    private static bool HasMoreThanTwoDecimals(decimal value) => decimal.Round(value, 2) != value;
}
