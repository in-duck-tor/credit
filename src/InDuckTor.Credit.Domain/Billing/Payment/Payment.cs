using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Expenses;

namespace InDuckTor.Credit.Domain.Billing.Payment;

/// <summary>
/// <b>Платёж</b>
/// </summary>
public class Payment
{
    public long Id { get; set; }

    public required long LoanId { get; init; }
    public required long ClientId { get; init; }

    /// <summary>
    /// <b>Сумма Платежа</b>
    /// </summary>
    public required decimal PaymentAmount { get; init; }

    /// <summary>
    /// Распределение Платежа формируется в конце Расчётного Периода
    /// </summary>
    public PaymentDistribution PaymentDistribution { get; init; } = new();

    public bool IsDistributed => PaymentDistribution.IsDistributed;

    public decimal PaymentToDistribute => PaymentAmount - PaymentDistribution.CalculateDistributedPaymentSum();

    public void DistributeOn(PeriodBilling periodBilling, List<IPrioritizedExpenseItem> items)
    {
        if (IsDistributed || periodBilling.IsPaid) return;

        var paymentToDistribute = PaymentToDistribute;
        if (paymentToDistribute == 0) return;

        var paymentExpenseItems = GetOrCreatePaymentBillingItems(periodBilling);

        foreach (var item in items)
        {
            if (periodBilling.IsPaid) continue;

            var min = decimal.Min(paymentToDistribute, item.Amount);
            item.ChangeAmount(-min);
            paymentToDistribute -= min;

            IncreasePaymentExpenseItems(paymentExpenseItems, item.Priority, min);
        }

        if (paymentToDistribute == 0) PaymentDistribution.IsDistributed = true;
    }

    private void IncreasePaymentExpenseItems(ExpenseItems paymentExpenseItems, PaymentPriority priority, decimal amount)
    {
        // todo: отрефакторить: перенести Penalty в один с paymentExpenseItems список. Для этого можно создать отдельный утилитарный класс PaymentExpenseItems
        if (priority == PaymentPriority.Penalty) PaymentDistribution.Penalty?.ChangeAmount(amount);
        else paymentExpenseItems.ChangeBasedOnPriority(priority, amount);
    }

    private ExpenseItems GetOrCreatePaymentBillingItems(PeriodBilling periodBilling)
    {
        ExpenseItems paymentExpenseItems;
        var billingsPayoffs = PaymentDistribution.BillingsPayoffs;

        if (billingsPayoffs.Count == 0 || billingsPayoffs.Last().PeriodBilling.Id != periodBilling.Id)
        {
            paymentExpenseItems = new ExpenseItems(0, 0, 0);
            PaymentDistribution.BillingsPayoffs.Add(new BillingPayoff
            {
                PeriodBilling = periodBilling,
                ExpenseItems = paymentExpenseItems
            });
        }
        else
        {
            paymentExpenseItems = billingsPayoffs.Last().ExpenseItems;
        }

        return paymentExpenseItems;
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
    public ExpenseItem? Penalty { get; set; }

    public decimal CalculateDistributedPaymentSum() => BillingsPayoffs
        .Select(payoff => payoff.ExpenseItems.GetTotalSum())
        .Aggregate(0m, (accumulate, periodDistribution) => accumulate + periodDistribution) + (Penalty?.Amount ?? 0m);
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
    public required ExpenseItems ExpenseItems { get; set; }
}