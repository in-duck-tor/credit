using InDuckTor.Credit.Domain.BillingPeriod.Payment;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.BillingPeriod;

public class PeriodService(PaymentService paymentService)
{
    /// <summary>
    /// <para>Создаёт Расчётный Период.</para>
    /// <para>Время закрытия передаётся для установки действительного времени закрытия Периода.
    /// Брать текущее время некорректо, т.к. действительное временя может отличаться от времени вызова этого метода.
    /// </para>
    /// </summary>
    /// <param name="loan">Кредит, для которого закрывается Расчётный Период</param>
    /// <param name="closingTime">Действительное время закрытия Расчётного Периода</param>
    public void CloseBillingPeriod(Loan loan, DateTime closingTime)
    {
        // todo: добавить проверку времени окончания Расчётного Периода
        var periodBilling = CreatePeriodBilling(loan, closingTime);
        paymentService.DistributePaymentsForNewPeriod(loan.Id, periodBilling);
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