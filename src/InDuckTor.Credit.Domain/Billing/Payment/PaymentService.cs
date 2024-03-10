using InDuckTor.Credit.Domain.Billing.Exceptions;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Billing.Payment;

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
    /// <param name="loanId">Id кредита, для которого происходит распределение</param>
    /// <param name="periodBilling">Расчёт за Период, по которому распределются Платежи</param>
    public void DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling)
    {
        var payments = paymentRepository.GetAllNonDistributedPayments(loanId);
        DistributePayments(periodBilling.LoanBilling, payments, [periodBilling]);
        if (!periodBilling.IsPaid) periodBilling.IsDebt = true;
    }

    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="payment"></param>
    public void DistributePayment(long loanId, Payment payment)
    {
        var unpaidPeriods = periodBillingRepository.GetAllUnpaidPeriodBillings(loanId);

        if (unpaidPeriods.Count == 0) return;
        
        DistributePayments(unpaidPeriods[0].LoanBilling, [payment], unpaidPeriods);
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
    /// Распределяет Платежи по Расчётам за Период, а также изменяет значения по кредиту у: тела долга, задолженностей и штрафов 
    /// </summary>
    /// <param name="loan">Кредит, по которому происходит распределение</param>
    /// <param name="loanBilling">Расчёт по Кредиту</param>
    /// <param name="payments">Платежи для распределения</param>
    /// <param name="unpaidPeriods">Расчёты за Период, по которым происходит распределение</param>
    /// <exception cref="InvalidPaymentDistributionException">Выбрасывается, если после распределения,
    /// один из платежей не распределён полностью и период, по которому он распределялся, не оплачен</exception>
    private void DistributePayments(LoanBilling loanBilling, List<Payment> payments, List<PeriodBilling> unpaidPeriods)
    {
        if (payments.Count == 0) return;
        if (unpaidPeriods.Count == 0) return;

        using var paymentEnumerator = payments.GetEnumerator();
        var payment = paymentEnumerator.Current;
        using var periodEnumerator = unpaidPeriods.GetEnumerator();
        var period = periodEnumerator.Current;

        while (true)
        {
            var paymentItems = loanBilling.GetBillingItemsForPeriod(period); 
            payment.DistributeOn(period, paymentItems);

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