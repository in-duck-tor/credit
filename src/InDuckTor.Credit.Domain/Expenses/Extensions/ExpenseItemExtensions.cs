namespace InDuckTor.Credit.Domain.Expenses.Extensions;

public static class ExpenseItemExtensions
{
    public static IPrioritizedExpenseItem ChainWith(this IPrioritizedExpenseItem prioritized, IExpenseItem item)
    {
        return new ChainedItem(prioritized, item);
    }
}