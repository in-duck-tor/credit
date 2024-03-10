using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Billing.Payment;

public static class PeriodBillingExtensions
{
    public static List<IPrioritizedBillingItem> GetPayoffBillingItemsSortedByPriority(this PeriodBilling periodBilling)
    {
        var payoff = periodBilling.RemainingPayoff;
        ArgumentNullException.ThrowIfNull(payoff);

        List<IPrioritizedBillingItem> items = [];

        if (periodBilling.IsDebt)
        {
            items.Add(new PeriodPaymentBillingItem(periodBilling, PaymentPriority.DebtInterest, payoff.Interest));
            items.Add(new PeriodPaymentBillingItem(periodBilling, PaymentPriority.DebtBody, payoff.LoanBodyPayoff));
        }
        else
        {
            items.Add(new PeriodPaymentBillingItem(periodBilling, PaymentPriority.RegularInterest, payoff.Interest));
            items.Add(new PeriodPaymentBillingItem(periodBilling, PaymentPriority.RegularBody, payoff.LoanBodyPayoff));
        }

        items.Add(new PeriodPaymentBillingItem(periodBilling, PaymentPriority.ChargingForServices,
            payoff.ChargingForServices));

        return items;
    }

    public static IPrioritizedBillingItem GetLoanBodyItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PeriodPaymentBillingItem(periodBilling, paymentPriority,
            periodBilling.RemainingPayoff.LoanBodyPayoff);
    }

    public static IPrioritizedBillingItem GetInterestItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PeriodPaymentBillingItem(periodBilling, paymentPriority, periodBilling.RemainingPayoff.Interest);
    }

    public static IPrioritizedBillingItem GetServicesItem(this PeriodBilling periodBilling,
        PaymentPriority paymentPriority)
    {
        ArgumentNullException.ThrowIfNull(periodBilling.RemainingPayoff);
        return new PeriodPaymentBillingItem(periodBilling, paymentPriority,
            periodBilling.RemainingPayoff.ChargingForServices);
    }
}