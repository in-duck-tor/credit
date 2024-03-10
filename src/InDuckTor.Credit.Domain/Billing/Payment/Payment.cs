using InDuckTor.Credit.Domain.Billing.Period;

namespace InDuckTor.Credit.Domain.Billing.Payment;

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

    public void DistributeOn(PeriodBilling periodBilling, List<IPrioritizedBillingItem> items)
    {
        if (periodBilling.IsPaid) return;

        var distributedPayment = PaymentDistribution.CalculateDistributedPaymentSum();

        var paymentToDistribute = PaymentAmount - distributedPayment;
        if (paymentToDistribute == 0) return;
        var paymentBillingItems = GetOrCreatePaymentBillingItems(periodBilling);

        foreach (var item in items)
        {
            if (periodBilling.IsPaid) continue;

            var min = decimal.Min(paymentToDistribute, periodBilling.GetRemainingInterest());
            item.ChangeAmount(-min);
            paymentToDistribute -= min;
            ChangeBillingItemAccordingToPriority(paymentBillingItems, item.Priority, min);
        }

        if (paymentToDistribute == 0) PaymentDistribution.IsDistributed = true;
    }

    private static void ChangeBillingItemAccordingToPriority(BillingItems billingItems,
        PaymentPriority paymentPriority,
        decimal amount)
    {
        switch (paymentPriority)
        {
            case PaymentPriority.DebtInterest:
            case PaymentPriority.RegularInterest:
                billingItems.ChangeInterest(amount);
                break;
            case PaymentPriority.DebtBody:
            case PaymentPriority.RegularBody:
                billingItems.ChangeLoanBodyPayoff(amount);
                break;
            case PaymentPriority.ChargingForServices:
                billingItems.ChangeChargingForServices(amount);
                break;
        }
    }

    private BillingItems GetOrCreatePaymentBillingItems(PeriodBilling periodBilling)
    {
        BillingItems paymentBillingItems;
        var billingsPayoffs = PaymentDistribution.BillingsPayoffs;

        if (billingsPayoffs.Count == 0 || billingsPayoffs.Last().PeriodBilling.Id != periodBilling.Id)
        {
            paymentBillingItems = new BillingItems(0, 0, 0);
            PaymentDistribution.BillingsPayoffs.Add(new BillingPayoff
            {
                PeriodBilling = periodBilling,
                BillingItems = paymentBillingItems
            });
        }
        else
        {
            paymentBillingItems = billingsPayoffs.Last().BillingItems;
        }

        return paymentBillingItems;
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