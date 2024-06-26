namespace InDuckTor.Credit.Domain.Expenses;

public interface IExpenseItem
{
    decimal Amount { get; }
    void ChangeAmount(decimal amount);
}

public class ExpenseItem : IExpenseItem
{
    private ExpenseItem() : this(0)
    {
    }

    public ExpenseItem(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(amount);
        Amount = amount;
    }

    private const decimal RoundingValue = 0.0000001m;

    public decimal Amount { get; private set; }

    public static ExpenseItem Zero => new();

    public void ChangeAmount(decimal amount)
    {
        var newAmount = Amount + amount;
        if (Math.Abs(newAmount) <= RoundingValue) newAmount = 0;
        ArgumentOutOfRangeException.ThrowIfNegative(newAmount);
        Amount = newAmount;
    }

    public static implicit operator ExpenseItem(decimal d) => new(d);
    public static implicit operator decimal(ExpenseItem ei) => ei.Amount;
}