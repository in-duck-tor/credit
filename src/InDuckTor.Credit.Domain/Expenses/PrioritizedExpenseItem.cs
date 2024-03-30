namespace InDuckTor.Credit.Domain.Expenses;

public enum PaymentPriority
{
    DebtInterest = 10,
    DebtBody = 20,
    Penalty = 30,
    RegularInterest = 40,
    RegularBody = 50,
    ChargingForServices = 60
}

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