using InDuckTor.Credit.Domain.BillingPeriod;

namespace InDuckTor.Credit.Domain.Payment;

/// <summary>
/// <b>Платёж</b>
/// </summary>
public class Payment
{
    public long Id { get; set; }

    /// <summary>
    /// <b>Сумма Платежа</b>
    /// </summary>
    public required decimal PaymentAmount { get; set; }

    /// <summary>
    /// Распределение Платежа формируется в конце Расчётного Периода
    /// </summary>
    public PaymentDistribution PaymentDistribution { get; set; } = new();

    public bool IsDistributed => PaymentDistribution.IsDistributed;

    // todo Добавить расчёт штрафов
    public void DistributeOn(PeriodBilling periodBilling)
    {
        if (periodBilling.IsPaid) return;

        var distributedPayment = PaymentDistribution.CalculateDistributedPaymentSum();
        
        var paymentToDistribute = PaymentAmount - distributedPayment;
        if (paymentToDistribute == 0) return;
        
        var paymentBillingItems = GetOrCreatePaymentBillingItems(periodBilling.Id);

        paymentToDistribute = InternalDistributePayment(periodBilling, paymentBillingItems, paymentToDistribute);

        if (paymentToDistribute == 0) PaymentDistribution.IsDistributed = true;
    }

    private BillingItems GetOrCreatePaymentBillingItems(long periodBillingId)
    {
        BillingItems paymentBillingItems;
        var billingsPayoffs = PaymentDistribution.BillingsPayoffs;

        if (billingsPayoffs.Count == 0 || billingsPayoffs.Last().PeriodBilling.Id != periodBillingId)
        {
            paymentBillingItems = new BillingItems(0, 0, 0);
        }
        else
        {
            paymentBillingItems = billingsPayoffs.Last().BillingItems;
        }

        return paymentBillingItems;
    }

    /// <summary>
    /// Распределяет <see cref="paymentToDistribute"/> по Статьям расчёта,
    /// уменьшая значения в periodPayoff и увеличивая в paymentBillingItems 
    /// </summary>
    /// <param name="periodBilling">Статьи расчёта для Расчёта по Периоду</param>
    /// <param name="paymentBillingItems">Статьи расчёта для Платежа</param>
    /// <param name="paymentToDistribute">Оставшаяся нераспределённая сумма</param>
    /// <returns>Нераспределённая сумма</returns>
    private decimal InternalDistributePayment(PeriodBilling periodBilling,
        BillingItems paymentBillingItems,
        decimal paymentToDistribute)
    {
        if (!periodBilling.IsPaid)
        {
            var min = decimal.Min(paymentToDistribute, periodBilling.GetRemainingInterest());
            periodBilling.ChangeInterest(-min);
            paymentToDistribute -= min;
            paymentBillingItems.ChangeInterest(min);
        }

        if (paymentToDistribute != 0 && periodBilling.IsPaid)
        {
            var min = decimal.Min(paymentToDistribute, periodBilling.GetRemainingLoanBodyPayoff());
            periodBilling.ChangeLoanBodyPayoff(-min);
            paymentToDistribute -= min;
            paymentBillingItems.ChangeLoanBodyPayoff(min);
        }

        if (paymentToDistribute != 0 && periodBilling.IsPaid)
        {
            var min = decimal.Min(paymentToDistribute, periodBilling.GetRemainingChargingForServices());
            periodBilling.ChangeChargingForServices(-min);
            paymentToDistribute -= min;
            paymentBillingItems.ChangeChargingForServices(min);
        }

        return paymentToDistribute;
    }
}

/// <summary>
/// <b>Распределение Платежа</b> по различным категориям
/// </summary>
public class PaymentDistribution
{
    public long Id { get; set; }

    public bool IsDistributed { get; set; }

    /// <summary>
    /// Погашения за Период, относящиеся к данному Платежу
    /// </summary>
    public List<BillingPayoff> BillingsPayoffs { get; } = [];

    /// <summary>
    /// Сумма Платежа, распределённая на оплату Штрафа 
    /// </summary>
    public decimal? Penalty { get; set; }

    public decimal CalculateDistributedPaymentSum() => BillingsPayoffs
        .Select(payoff => payoff.BillingItems.GetTotalSum())
        .Aggregate((accumulate, periodDistribution) => accumulate + periodDistribution) + Penalty ?? 0;
}

/// <summary>
/// <b>Погашение за Период</b>
/// </summary>
public class BillingPayoff
{
    public long Id { get; set; }

    /// <summary>
    /// Расчётный Период, к которому относится текущее Погашение 
    /// </summary>
    public required PeriodBilling PeriodBilling { get; set; }

    /// <summary>
    /// Статьи, по которым распределась часть текущего платежа
    /// </summary>
    public required BillingItems BillingItems { get; set; }
}