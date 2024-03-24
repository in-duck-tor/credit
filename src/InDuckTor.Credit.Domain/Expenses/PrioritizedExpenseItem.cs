using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Expenses;

public interface IPrioritizedExpenseItem : IExpenseItem
{
    PaymentPriority Priority { get; }
}

public class PrioritizedExpenseItem : IPrioritizedExpenseItem
{
    public PrioritizedExpenseItem(PaymentPriority priority, ExpenseItem expenseItem)
    {
        ExpenseItem = expenseItem;
        Priority = priority;
    }

    private ExpenseItem ExpenseItem { get; }

    public decimal Amount => ExpenseItem.Amount;
    public PaymentPriority Priority { get; }

    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        ExpenseItem.ChangeAmount(amount);
    }
}

public class PeriodPaymentExpenseItem : IPrioritizedExpenseItem
{
    public PeriodPaymentExpenseItem(PeriodBilling periodBilling, PaymentPriority priority, ExpenseItem expenseItem) :
        this(
            periodBilling, new PrioritizedExpenseItem(priority, expenseItem))
    {
    }

    public PeriodPaymentExpenseItem(PeriodBilling periodBilling, PrioritizedExpenseItem prioritizedExpenseItem)
    {
        PrioritizedExpenseItem = prioritizedExpenseItem;
        PeriodBilling = periodBilling;
    }

    private PrioritizedExpenseItem PrioritizedExpenseItem { get; }
    private PeriodBilling PeriodBilling { get; }

    public decimal Amount => PrioritizedExpenseItem.Amount;
    public PaymentPriority Priority => PrioritizedExpenseItem.Priority;

    public void ChangeAmount(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(PeriodBilling.RemainingPayoff);
        PrioritizedExpenseItem.ChangeAmount(amount);
        if (PeriodBilling.RemainingPayoff.GetTotalSum() == 0) PeriodBilling.RemainingPayoff = null;
    }
}

public class ChainedPrioritizedItem : IPrioritizedExpenseItem
{
    public ChainedPrioritizedItem(IPrioritizedExpenseItem prioritizedExpenseItem, IExpenseItem chainedItem)
    {
        PrioritizedExpenseItem = prioritizedExpenseItem;
        ChainedItem = chainedItem;
    }

    private IPrioritizedExpenseItem PrioritizedExpenseItem { get; }
    private IExpenseItem ChainedItem { get; }

    public decimal Amount => PrioritizedExpenseItem.Amount;
    public PaymentPriority Priority => PrioritizedExpenseItem.Priority;


    public void ChangeAmount(decimal amount)
    {
        PrioritizedExpenseItem.ChangeAmount(amount);
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
    public static ChainedPrioritizedItem ChainWith(this IPrioritizedExpenseItem prioritized, IExpenseItem item)
    {
        return new ChainedPrioritizedItem(prioritized, item);
    }
}

public static class BillingItemsExtensions
{
    public static void ChangeBasedOnPriority(this ExpenseItems expenseItems,
        PaymentPriority paymentPriority,
        decimal amount)
    {
        switch (paymentPriority)
        {
            case PaymentPriority.DebtInterest:
            case PaymentPriority.RegularInterest:
                expenseItems.ChangeInterest(amount);
                break;
            case PaymentPriority.DebtBody:
            case PaymentPriority.RegularBody:
                expenseItems.ChangeLoanBodyPayoff(amount);
                break;
            case PaymentPriority.ChargingForServices:
                expenseItems.ChangeChargingForServices(amount);
                break;
            case PaymentPriority.Penalty:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(paymentPriority), paymentPriority, null);
        }
    }
}