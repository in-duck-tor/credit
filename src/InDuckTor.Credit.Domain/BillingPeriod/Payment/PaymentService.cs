using InDuckTor.Credit.Domain.BillingPeriod.Payment.Model;

namespace InDuckTor.Credit.Domain.BillingPeriod.Payment;

public class PaymentService(IPaymentRepository paymentRepository, IPeriodBillingRepository periodBillingRepository)
{
    /// <summary>
    /// Распределяет Платёж для только что созданного Расчётного Периода. В конце операции все платежи должны распределиться.
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="periodBilling"></param>
    public void DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling)
    {
    }

    // Чтобы распределить платёж нужно:
    // 1. Получить все Расчёты за Периоды, по которым есть задолженности
    // 2. Распределить платёж
    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="payment"></param>
    public void DistributePayment(long loanId, Payment payment)
    {
        var unpaidPeriods = periodBillingRepository.GetAllUnpaidPeriodBillings(loanId);

        foreach (var period in unpaidPeriods)
        {
            if (payment.IsDistributed) return;
            payment.DistributeOn(period);

            ArgumentNullException.ThrowIfNull(period.RemainingPayoff);
            if (period.RemainingPayoff.GetTotalSum() == 0) period.RemainingPayoff = null;
        }
    }

    public Payment CreatePayment(NewPayment newPayment)
    {
        return new Payment
        {
            PaymentAmount = newPayment.PaymentAmount,
            PaymentDistribution = new PaymentDistribution()
        };
    }
}