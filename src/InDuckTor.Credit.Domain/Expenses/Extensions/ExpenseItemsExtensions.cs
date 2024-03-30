namespace InDuckTor.Credit.Domain.Expenses.Extensions;

public static class ExpenseItemsExtensions
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