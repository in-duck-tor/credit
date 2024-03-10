namespace InDuckTor.Credit.Domain.Billing.Period;

public interface IBillingItem
{
    decimal Amount { get; }
    void ChangeAmount(decimal amount);
}

public struct BillingItem(decimal amount) : IBillingItem
{
    public BillingItem() : this(0)
    {
    }

    public decimal Amount { get; private set; } = amount;

    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        Amount += amount;
    }

    public static implicit operator BillingItem(decimal d) => new(d);
    public static implicit operator decimal(BillingItem b) => b.Amount;
}