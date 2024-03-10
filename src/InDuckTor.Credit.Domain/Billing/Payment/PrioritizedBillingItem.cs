using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Billing.Payment;

public interface IPrioritizedBillingItem : IBillingItem
{
    PaymentPriority Priority { get; }
}

public class PrioritizedBillingItem(PaymentPriority priority, BillingItem billingItem) : IPrioritizedBillingItem
{
    private BillingItem BillingItem { get; } = billingItem;

    public decimal Amount => BillingItem.Amount;
    public PaymentPriority Priority { get; } = priority;

    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        BillingItem.ChangeAmount(amount);
    }
}

public class PeriodPaymentBillingItem(PeriodBilling periodBilling, PrioritizedBillingItem prioritizedBillingItem)
    : IPrioritizedBillingItem
{
    public PeriodPaymentBillingItem(PeriodBilling periodBilling, PaymentPriority priority, BillingItem billingItem) :
        this(
            periodBilling, new PrioritizedBillingItem(priority, billingItem))
    {
    }

    private PrioritizedBillingItem PrioritizedBillingItem { get; } = prioritizedBillingItem;
    private PeriodBilling PeriodBilling { get; } = periodBilling;

    public decimal Amount => PrioritizedBillingItem.Amount;
    public PaymentPriority Priority => PrioritizedBillingItem.Priority;

    public void ChangeAmount(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(PeriodBilling.RemainingPayoff);
        PrioritizedBillingItem.ChangeAmount(amount);
        if (PeriodBilling.RemainingPayoff.GetTotalSum() == 0) PeriodBilling.RemainingPayoff = null;
    }
}

public class ChainedPrioritizedItem(IPrioritizedBillingItem prioritizedBillingItem, IBillingItem chainedItem)
    : IPrioritizedBillingItem
{
    private IPrioritizedBillingItem PrioritizedBillingItem { get; } = prioritizedBillingItem;
    private IBillingItem ChainedItem { get; } = chainedItem;

    public decimal Amount => PrioritizedBillingItem.Amount;
    public PaymentPriority Priority => PrioritizedBillingItem.Priority;


    public void ChangeAmount(decimal amount)
    {
        PrioritizedBillingItem.ChangeAmount(amount);
        ChainedItem.ChangeAmount(amount);
    }
}

public enum PaymentPriority
{
    DebtInterest = 10,
    DebtBody = 20,
    Penalty = 30,
    RegularInterest = 40,
    RegularBody = 50,
    ChargingForServices = 60
}

public static class BillingItemExtensions
{
    public static ChainedPrioritizedItem ChainWith(this IPrioritizedBillingItem prioritized, IBillingItem item)
    {
        return new ChainedPrioritizedItem(prioritized, item);
    }
}

public static class BillingItemsExtensions
{
    public static void ChangeBasedOnPriority(this BillingItems billingItems,
        PaymentPriority paymentPriority,
        decimal amount)
    {
        switch (paymentPriority)
        {
            case PaymentPriority.DebtInterest:
            case PaymentPriority.RegularInterest:
                billingItems.ChangeInterest(amount);
                break;
            case PaymentPriority.DebtBody:
            case PaymentPriority.RegularBody:
                billingItems.ChangeLoanBodyPayoff(amount);
                break;
            case PaymentPriority.ChargingForServices:
                billingItems.ChangeChargingForServices(amount);
                break;
        }
    }
}