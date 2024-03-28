using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Expenses.Extensions;

public static class PeriodExpenseExtensions
{
    public static IPrioritizedExpenseItem GetLoanBodyItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PrioritizedExpenseItem(paymentPriority, periodBilling.RemainingPayoff.LoanBodyPayoff);
    }

    public static IPrioritizedExpenseItem GetInterestItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PrioritizedExpenseItem(paymentPriority, periodBilling.RemainingPayoff.Interest);
    }

    public static IPrioritizedExpenseItem GetServicesItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PrioritizedExpenseItem(paymentPriority, periodBilling.RemainingPayoff.ChargingForServices);
    }
}