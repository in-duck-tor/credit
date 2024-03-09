using InDuckTor.Credit.Domain.BillingPeriod.Exceptions;
using InDuckTor.Credit.Domain.BillingPeriod.Payment.Models;

namespace InDuckTor.Credit.Domain.BillingPeriod.Payment;

public class PaymentService(IPaymentRepository paymentRepository, IPeriodBillingRepository periodBillingRepository)
{
    // В идеале, т.к. в реальном банке расчётные периоды закрываются каждый день, при закрытии нового периода создавать
    // таску, которая позже будет обработана. При этом, чтобы не начислить лишние проценты и не приписать долг в период,
    // нужно проверить, есть ли у пользователя нераспределённые платежи, а также создать механизм перерасчёта процентов,
    // чтобы, если проценты без учёта нового тела успели начислиться, можно было их пересчитать.
    // Но, т.к. это сложно, мы будем производить распределение платежей для созданного периода сразу при его создании.
    /// <summary>
    /// Распределяет Платёж для только что созданного Расчётного Периода. В конце операции все платежи должны распределиться.
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="periodBilling"></param>
    public void DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling)
    {
        var payments = paymentRepository.GetAllNonDistributedPayments(loanId);
        DistributePayments(payments, [periodBilling]);
    }

    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="payment"></param>
    public void DistributePayment(long loanId, Payment payment)
    {
        var unpaidPeriods = periodBillingRepository.GetAllUnpaidPeriodBillings(loanId);
        DistributePayments([payment], unpaidPeriods);
    }

    public Payment CreatePayment(NewPayment newPayment)
    {
        return new Payment
        {
            PaymentAmount = newPayment.PaymentAmount,
            PaymentDistribution = new PaymentDistribution()
        };
    }

    /// <summary>
    /// Распределяет Платежи по Расчётам за Период
    /// </summary>
    /// <param name="payments"></param>
    /// <param name="periods"></param>
    /// <exception cref="InvalidPaymentDistributionException"></exception>
    private void DistributePayments(List<Payment> payments, List<PeriodBilling> periods)
    {
        if (payments.Count == 0) return;
        if (periods.Count == 0) return;

        using var paymentEnumerator = payments.GetEnumerator();
        var payment = paymentEnumerator.Current;
        using var periodEnumerator = periods.GetEnumerator();
        var period = periodEnumerator.Current;

        while (true)
        {
            payment.DistributeOn(period);

            if (!payment.IsDistributed && !period.IsPaid)
                throw new InvalidPaymentDistributionException(
                    "Платёж не может быть не распределён при неоплаченном периоде");

            if (payment.IsDistributed)
            {
                if (!paymentEnumerator.MoveNext()) return;
                payment = paymentEnumerator.Current;
            }

            if (!period.IsPaid) continue;

            if (!periodEnumerator.MoveNext()) return;
            period = periodEnumerator.Current;
        }
    }
}