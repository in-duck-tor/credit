using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Period;

public class PeriodService(PaymentService paymentService)
{
    /// <summary>
    /// <para>Создаёт Расчётный Период.</para>
    /// <para>Время закрытия передаётся для установки действительного времени закрытия Периода.
    /// Брать текущее время некорректо, т.к. действительное временя может отличаться от времени вызова этого метода.
    /// </para>
    /// </summary>
    /// <param name="loanBilling">Кредит, для которого закрывается Расчётный Период</param>
    /// <param name="closingTime">Действительное время закрытия Расчётного Периода</param>
    public PeriodBilling CloseBillingPeriod(LoanBilling loanBilling, DateTime closingTime)
    {
        // todo: добавить проверку времени окончания Расчётного Периода
        var periodBilling = CreatePeriodBilling(loanBilling, closingTime);
        paymentService.DistributePaymentsForNewPeriod(periodBilling);
        loanBilling.AddAndRecalculateForNewPeriod(periodBilling);
        return periodBilling;
    }

    private PeriodBilling CreatePeriodBilling(LoanBilling loanBilling, DateTime endDate)
    {
        ArgumentNullException.ThrowIfNull(loanBilling.PeriodAccruals);

        var billingItems = new BillingItems(
            loanBilling.PeriodAccruals.InterestAccrual,
            loanBilling.PeriodAccruals.LoanBodyPayoff,
            loanBilling.PeriodAccruals.ChargingForServices);

        var periodBilling = new PeriodBilling
        {
            PeriodStartDate = loanBilling.PeriodAccruals.PeriodStartDate,
            Loan = loanBilling.Loan,
            LoanBilling = loanBilling,
            PeriodEndDate = endDate,
            OneTimePayment = loanBilling.PeriodAccruals.OneTimePayment,
            BillingItems = billingItems,
            RemainingPayoff = billingItems.DeepCopy(),
        };

        return periodBilling;
    }
}