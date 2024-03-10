using InDuckTor.Credit.Domain.Billing.Payment;

namespace InDuckTor.Credit.Domain.Billing.Period;

public class PeriodService(IPaymentService paymentService)
{
    /// <summary>
    /// <para>Создаёт Расчётный Период.</para>
    /// <para>Время закрытия передаётся для установки действительного времени закрытия Периода.
    /// Брать текущее время некорректо, т.к. действительное временя может отличаться от времени вызова этого метода.
    /// </para>
    /// </summary>
    /// <param name="loanBilling">Кредит, для которого закрывается Расчётный Период</param>
    /// <param name="closingTime">Действительное время закрытия Расчётного Периода</param>
    internal async Task<PeriodBilling> CloseBillingPeriod(LoanBilling loanBilling, DateTime closingTime)
    {
        // todo: добавить проверку времени окончания Расчётного Периода
        var periodBilling = CreatePeriodBilling(loanBilling, closingTime);
        await paymentService.DistributePaymentsForNewPeriod(loanBilling.Loan.Id, periodBilling);
        loanBilling.AddNewPeriodAndRecalculate(periodBilling);
        return periodBilling;
    }

    private static PeriodBilling CreatePeriodBilling(LoanBilling loanBilling, DateTime endDate)
    {
        ArgumentNullException.ThrowIfNull(loanBilling.PeriodAccruals);

        var billingItems = new BillingItems(
            loanBilling.PeriodAccruals.InterestAccrual,
            loanBilling.PeriodAccruals.LoanBodyPayoff,
            loanBilling.PeriodAccruals.ChargingForServices);

        var periodBilling = new PeriodBilling
        {
            PeriodStartDate = loanBilling.PeriodAccruals.PeriodStartDate,
            LoanBilling = loanBilling,
            PeriodEndDate = endDate,
            OneTimePayment = loanBilling.PeriodAccruals.OneTimePayment,
            BillingItems = billingItems,
            RemainingPayoff = billingItems.DeepCopy(),
        };

        return periodBilling;
    }
}