using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Period;

public class PeriodService(IPaymentService paymentService)
{
    /// <summary>
    /// <para>Создаёт Расчётный Период.</para>
    /// <para>Время закрытия передаётся для установки действительного времени закрытия Периода.
    /// Брать текущее время некорректо, т.к. действительное временя может отличаться от времени вызова этого метода.
    /// </para>
    /// </summary>
    /// <param name="loan">Кредит, для которого закрывается Расчётный Период</param>
    internal async Task<PeriodBilling> CloseBillingPeriod(Loan loan)
    {
        var periodBilling = loan.ClosePeriod();
        await paymentService.DistributePaymentsForNewPeriod(loan.Id, periodBilling);
        return periodBilling;
    }
}