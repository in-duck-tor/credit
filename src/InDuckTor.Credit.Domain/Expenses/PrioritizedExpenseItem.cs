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
        ExpenseItem.ChangeAmount(amount);
    }
}

public class ChainedItem : IPrioritizedExpenseItem
{
    public ChainedItem(IPrioritizedExpenseItem prioritizedExpenseItem, IExpenseItem chained)
    {
        PrioritizedExpenseItem = prioritizedExpenseItem;
        Chained = chained;
    }

    private IPrioritizedExpenseItem PrioritizedExpenseItem { get; }
    private IExpenseItem Chained { get; }

    public decimal Amount => PrioritizedExpenseItem.Amount;
    public PaymentPriority Priority => PrioritizedExpenseItem.Priority;


    public void ChangeAmount(decimal amount)
    {
        PrioritizedExpenseItem.ChangeAmount(amount);
        Chained.ChangeAmount(amount);
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
    public static IPrioritizedExpenseItem ChainWith(this IPrioritizedExpenseItem prioritized, IExpenseItem item)
    {
        return new ChainedItem(prioritized, item);
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