using InDuckTor.Credit.Domain.Billing.Payment.Extensions;
using InDuckTor.Credit.Domain.Billing.Payment.Models;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Payment;

public interface IPaymentService
{
    /// <summary>
    /// Распределяет Платёж для только что созданного Расчётного Периода. В конце операции все платежи должны распределиться.
    /// </summary>
    /// <param name="loanId">Id кредита, для которого происходит распределение</param>
    /// <param name="periodBilling">Расчёт за Период, по которому распределются Платежи</param>
    Task DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling);

    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="payment">Платёж, который нужно распределить</param>
    Task DistributePayment(Payment payment);

    Task<Payment> CreatePayment(NewPayment newPayment, CancellationToken cancellationToken);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPeriodBillingRepository _periodBillingRepository;
    private readonly ILoanRepository _loanRepository;

    public PaymentService(IPaymentRepository paymentRepository,
        IPeriodBillingRepository periodBillingRepository,
        ILoanRepository loanRepository)
    {
        _paymentRepository = paymentRepository;
        _periodBillingRepository = periodBillingRepository;
        _loanRepository = loanRepository;
    }

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
    public async Task DistributePaymentsForNewPeriod(long loanId, PeriodBilling periodBilling)
    {
        var payments = await _paymentRepository.GetAllNonDistributedPayments(loanId);
        DistributePayments(periodBilling.Loan, payments, [periodBilling]);
        if (!periodBilling.IsPaid) periodBilling.IsDebt = true;
    }

    /// <summary>
    /// Распределяет Платеж по Задолженностям, если такие имеются
    /// </summary>
    /// <param name="payment">Платёж, который нужно распределить</param>
    public async Task DistributePayment(Payment payment)
    {
        var unpaidPeriods = await _periodBillingRepository.GetAllUnpaidPeriodBillings(payment.LoanId);

        if (unpaidPeriods.Count == 0) return;

        var totalRemainingPayment = unpaidPeriods
            .Select(periodBilling => periodBilling.TotalRemainingPayment)
            .Sum();

        if (payment.PaymentToDistribute > totalRemainingPayment)
            throw Errors.Payment.InvalidRegularPaymentAmount.TooMuch();

        DistributePayments(unpaidPeriods[0].Loan, [payment], unpaidPeriods);
    }

    public async Task<Payment> CreatePayment(NewPayment newPayment, CancellationToken cancellationToken)
    {
        var isExists = await _loanRepository.IsExists(newPayment.LoanId,
            newPayment.ClientId,
            cancellationToken);

        if (!isExists) throw new Errors.Loan.NotFound("Loan with specified client id and loan id is not found");

        return new Payment
        {
            LoanId = newPayment.LoanId,
            ClientId = newPayment.ClientId,
            PaymentAmount = newPayment.PaymentAmount
        };
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

        using var paymentEnumerator = payments.GetEnumerator();
        paymentEnumerator.MoveNext();
        var payment = paymentEnumerator.Current;

        using var periodEnumerator = unpaidPeriods.GetEnumerator();
        periodEnumerator.MoveNext();
        var period = periodEnumerator.Current;

        var paymentItems = loan.GetBillingItemsForPeriod(period);

        while (true)
        {
            payment.DistributeOn(period, paymentItems);

            if (!payment.IsDistributed && !period.IsPaid)
                throw new Errors.Payment.InvalidPaymentDistributionException(
                    "Платёж не может быть не распределён при неоплаченном периоде");

            if (payment.IsDistributed)
            {
                if (!paymentEnumerator.MoveNext()) return;
                payment = paymentEnumerator.Current;
            }

            if (!period.IsPaid) continue;
            if (!periodEnumerator.MoveNext()) return;

            period = periodEnumerator.Current;
            paymentItems = loan.GetBillingItemsForPeriod(period);
        }
    }
}