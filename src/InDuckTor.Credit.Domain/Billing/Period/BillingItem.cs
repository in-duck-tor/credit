namespace InDuckTor.Credit.Domain.Billing.Period;

public interface IBillingItem
{
    decimal Amount { get; }
    void ChangeAmount(decimal amount);
}

public class BillingItem(decimal amount) : IBillingItem
{
    public BillingItem() : this(0)
    {
    }

    public decimal Amount { get; private set; } = amount;

    public virtual void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        Amount += amount;
    }

    public static implicit operator BillingItem(decimal d) => new BillingItem(d);
    public static implicit operator decimal(BillingItem b) => b.Amount;
}