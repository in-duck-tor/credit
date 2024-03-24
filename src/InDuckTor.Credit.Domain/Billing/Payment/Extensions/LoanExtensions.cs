using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.Expenses.Extensions;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Payment.Extensions;

public static class LoanExtensions
{
    public static List<IPrioritizedExpenseItem> GetBillingItemsForPeriod(this Loan loan, PeriodBilling periodBilling)
    {
        if (periodBilling.Loan.Id != loan.Id)
            throw Errors.EntitiesIsNotRelatedException.WithNames(nameof(Loan), nameof(PeriodBilling));

        List<IPrioritizedExpenseItem> items = [];

        if (periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.DebtInterest)
                .ChainWith(loan.Debt));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.DebtBody)
                .ChainWith(loan.Body)
                .ChainWith(loan.Debt));
        }

        items.Add(new PrioritizedExpenseItem(PaymentPriority.Penalty, loan.Penalty));

        if (!periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.RegularInterest));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.RegularBody)
                .ChainWith(loan.Body));
        }

        items.Add(periodBilling.GetServicesItem(PaymentPriority.ChargingForServices));

        return items;
    }
}