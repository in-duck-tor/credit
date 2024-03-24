namespace InDuckTor.Credit.Domain.Expenses;

public interface IExpenseItem
{
    decimal Amount { get; }
    void ChangeAmount(decimal amount);
}

public struct ExpenseItem(decimal amount) : IExpenseItem
{
    public ExpenseItem() : this(0)
    {
    }

    public decimal Amount { get; private set; } = amount;

    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        Amount += amount;
    }

    public static implicit operator ExpenseItem(decimal d) => new(d);
    public static implicit operator decimal(ExpenseItem b) => b.Amount;
}