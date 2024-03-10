using InDuckTor.Credit.Domain.BillingPeriod;
using InDuckTor.Credit.Domain.BillingPeriod.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.Payment.Models;

namespace InDuckTor.Credit.Domain.Payment;

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
    /// <param name="periodBilling">Расчёт за Период, по которому распределются Платежи</param>
    public void DistributePaymentsForNewPeriod(PeriodBilling periodBilling)
    {
        var payments = paymentRepository.GetAllNonDistributedPayments(periodBilling.Loan.Id);
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
    /// Распределяет Платежи по Расчётам за Период, а также изменяет значения по кредиту у: тела долга, задолженностей и штрафов 
    /// </summary>
    /// <param name="loan">Кредит, по которому происходит распределение</param>
    /// <param name="payments">Платежи для распределения</param>
    /// <param name="periods">Расчёты за Период, по которым происходит распределение</param>
    /// <exception cref="InvalidPaymentDistributionException">Выбрасывается, если после распределения,
    /// один из платежей не распределён полностью и период, по которому он распределялся, не оплачен</exception>
    private void DistributePayments(Loan loan, List<Payment> payments, List<PeriodBilling> periods)
    {
        if (payments.Count == 0) return;
        if (periods.Count == 0) return;

        using var paymentEnumerator = payments.GetEnumerator();
        var payment = paymentEnumerator.Current;
        using var periodEnumerator = periods.GetEnumerator();
        var period = periodEnumerator.Current;

        while (true)
        {
            payment.DistributeOn(period, loan.Penalty);

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

interface IBillingItem
{
    decimal Amount { get; }
    void ChangeAmount(decimal amount);
}

interface IPrioritizedBillingItem : IBillingItem
{
    int Priority { get; }
}

class BillingItem(decimal amount) : IBillingItem
{
    public decimal Amount { get; private set; } = amount;

    public virtual void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        Amount += amount;
    }
}

class PaymentBillingItem(int priority, BillingItem billingItem) : IPrioritizedBillingItem
{
    private BillingItem BillingItem { get; } = billingItem;

    public decimal Amount => BillingItem.Amount;
    public int Priority { get; } = priority;

    public void ChangeAmount(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(Amount + amount);
        BillingItem.ChangeAmount(amount);
    }
}

class PeriodPaymentBillingItem(PeriodBilling periodBilling, PaymentBillingItem paymentBillingItem)
    : IPrioritizedBillingItem
{
    public decimal Amount => PaymentBillingItem.Amount;
    public int Priority => PaymentBillingItem.Priority;

    private PaymentBillingItem PaymentBillingItem { get; } = paymentBillingItem;
    private PeriodBilling PeriodBilling { get; } = periodBilling;

    public void ChangeAmount(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(PeriodBilling.RemainingPayoff);
        if (PeriodBilling.RemainingPayoff.GetTotalSum() == 0) PeriodBilling.RemainingPayoff = null;
    }
}