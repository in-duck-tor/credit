using InDuckTor.Credit.Domain.Billing.Payment.Extensions;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Payment;

public interface IPaymentService
{
    Task<Payment> CreatePayment(Loan loan, NewPayment newPayment, CancellationToken cancellationToken);

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
    Task DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling);

    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="loan">Кредит, для которого распределяется Платёж</param>
    /// <param name="payment">Платёж, который нужно распределить</param>
    Task DistributePayment(Loan loan, Payment payment);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPeriodBillingRepository _periodBillingRepository;

    public PaymentService(IPaymentRepository paymentRepository,
        IPeriodBillingRepository periodBillingRepository)
    {
        _paymentRepository = paymentRepository;
        _periodBillingRepository = periodBillingRepository;
    }

    public async Task<Payment> CreatePayment(Loan loan, NewPayment newPayment, CancellationToken cancellationToken)
    {
        Errors.Payment.InvalidRegularPaymentAmount.ThrowIfNotPositive(newPayment.PaymentAmount);

        if (loan.Id != newPayment.LoanId || loan.ClientId != newPayment.ClientId)
            throw new Errors.Loan.NotFound("Loan with specified client id and loan id is not found");

        var undistributedPayments = await _paymentRepository.GetAllNonDistributedPayments(loan.Id);

        var totalRemainingPayment = loan.GetCurrentTotalPayment();
        var totalPaymentToDistributeSum = undistributedPayments.Select(p => p.PaymentToDistribute).Sum();
        var currentMaxPayment = totalRemainingPayment - totalPaymentToDistributeSum;

        if (currentMaxPayment == 0) Errors.Payment.InvalidRegularPaymentAmount.TooMuch();

        var realPaymentAmount = newPayment.PaymentAmount > currentMaxPayment
            ? currentMaxPayment
            : newPayment.PaymentAmount;

        return new Payment(newPayment.LoanId, newPayment.ClientId, realPaymentAmount);
    }

    public async Task DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling)
    {
        var payments = await _paymentRepository.GetAllNonDistributedPayments(loanId);
        DistributePayments(periodBilling.Loan, payments, [periodBilling]);
        if (!periodBilling.IsPaid) periodBilling.IsDebt = true;
    }

    public async Task DistributePayment(Loan loan, Payment payment)
    {
        var unpaidPeriods = await _periodBillingRepository.GetAllUnpaidPeriodBillings(loan.Id);

        if (unpaidPeriods.Count == 0) return;

        DistributePayments(loan, [payment], unpaidPeriods);
    }

    /// <summary>
    /// Распределяет Платежи по Расчётам за Период, а также изменяет значения по кредиту у: тела долга, задолженностей и штрафов 
    /// </summary>
    /// <param name="loan">Кредит</param>
    /// <param name="payments">Платежи для распределения</param>
    /// <param name="unpaidPeriods">Расчёты за Период, по которым происходит распределение</param>
    /// <exception cref="Errors.Payment.InvalidPaymentDistributionException">Выбрасывается, если после распределения,
    /// один из платежей не распределён полностью и период, по которому он распределялся, не оплачен</exception>
    private static void DistributePayments(Loan loan, List<Payment> payments, List<PeriodBilling> unpaidPeriods)
    {
        if (payments.Count == 0) return;
        if (unpaidPeriods.Count == 0) return;

#if DEBUG
        if (unpaidPeriods.Select(p => p.GetRemainingInterest() + p.GetRemainingLoanBodyPayoff()).Sum() > loan.Debt)
            throw new Exception("Debt is less than remainings. Pizda.");
#endif

        using var paymentEnumerator = payments.GetEnumerator();
        paymentEnumerator.MoveNext();
        var payment = paymentEnumerator.Current;

        using var periodEnumerator = unpaidPeriods.GetEnumerator();
        periodEnumerator.MoveNext();
        var period = periodEnumerator.Current;

        var paymentItems = loan.GetExpenseItemsForPeriod(period);

        while (true)
        {
            payment.DistributeOn(period, paymentItems);

            if (!payment.IsDistributed && !period.IsPaid)
                throw new Errors.Payment.InvalidPaymentDistributionException(
                    "The payment cannot be undistributed for an unpaid period");

            if (payment.IsDistributed)
            {
                if (!paymentEnumerator.MoveNext()) return;
                payment = paymentEnumerator.Current;
            }

            if (!period.IsPaid) continue;
            if (!periodEnumerator.MoveNext()) return;

            period = periodEnumerator.Current;
            paymentItems = loan.GetExpenseItemsForPeriod(period);
        }
    }
}