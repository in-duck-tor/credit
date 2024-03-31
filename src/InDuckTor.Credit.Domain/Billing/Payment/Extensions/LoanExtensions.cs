using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.Expenses.Extensions;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Payment.Extensions;

public static class LoanExtensions
{
    // Вместо того, чтобы возвращать изменяемые статьи, можно возвращать только информацию о сумме, которую нужно оплатить
    // и приоритете статьи. Далее, при распределении платежа, будет составляться "Распределение Платежа по Периоду", которое потом
    // применится к периоду, а несколько таких распределений применятся к кредиту.
    // Либо можно продолжить создавать изменяемые статьи, но не включать в них категории кредита, и изменять по схеме выше.
    // Нужно это для того, чтобы централизовать изменения полей сущностей, упростив дебаг.
    public static List<IPrioritizedExpenseItem> GetExpenseItemsForPeriod(this Loan loan, PeriodBilling periodBilling)
    {
        if (periodBilling.Loan.Id != loan.Id)
            throw Errors.EntitiesIsNotRelatedException.WithNames(nameof(Loan), nameof(PeriodBilling));

        List<IPrioritizedExpenseItem> items = [];

        if (periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.DebtInterest)
                .ChainWith(loan.Debt));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.DebtBody)
                .ChainWith(loan.CurrentBody)
                .ChainWith(loan.Debt));
        }

        items.Add(new PrioritizedExpenseItem(PaymentPriority.Penalty, loan.Penalty));

        if (!periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.RegularInterest));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.RegularBody)
                .ChainWith(loan.CurrentBody));
        }

        items.Add(periodBilling.GetServicesItem(PaymentPriority.ChargingForServices));

        return items;
    }

    public static void Main()
    {
        var d = 15000.28617216117216117216117;
        Console.WriteLine(Math.Round(d, 2, MidpointRounding.ToPositiveInfinity));
    }
}