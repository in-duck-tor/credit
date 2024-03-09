using InDuckTor.Credit.Domain.BillingPeriod.Payment;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.BillingPeriod;

public class PeriodService(PaymentService paymentService)
{
    /// <summary>
    /// Создаёт Расчётный Период
    /// </summary>
    /// <param name="loan"></param>
    /// <param name="closingTime"></param>
    public void CloseBillingPeriod(Loan loan, DateTime closingTime)
    {
        // Сюда нужно добавить проверку времени окончания Расчётного Периода
        var periodBilling = CreatePeriodBilling(loan, closingTime);
    }

    private PeriodBilling CreatePeriodBilling(Loan loan, DateTime endDate)
    {
        ArgumentNullException.ThrowIfNull(loan.PeriodAccruals);

        var billingItems = new BillingItems(
            loan.PeriodAccruals.InterestAccrual,
            loan.PeriodAccruals.LoanBodyPayoff,
            loan.PeriodAccruals.ChargingForServices);
        var periodBilling = new PeriodBilling
        {
            PeriodStartDate = loan.PeriodAccruals.PeriodStartDate,
            Loan = loan,
            PeriodEndDate = endDate,
            OneTimePayment = loan.PeriodAccruals.OneTimePayment,
            BillingItems = billingItems,
            RemainingPayoff = billingItems.DeepCopy()
        };

        return periodBilling;
    }
}