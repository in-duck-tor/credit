using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.Expenses.Extensions;

namespace InDuckTor.Credit.Domain.Billing.Payment;

/// <summary>
/// <b>Платёж</b>
/// </summary>
public class Payment
{
    public Payment(long loanId, long clientId, decimal paymentAmount)
    {
        Errors.Payment.InvalidRegularPaymentAmount.ThrowIfNotPositive(paymentAmount);

        LoanId = loanId;
        ClientId = clientId;
        PaymentAmount = paymentAmount;

        IsDistributed = false;
        BillingsPayoffs = [];
        Penalty = null;
    }

    public long Id { get; init; }
    public long LoanId { get; init; }
    public long ClientId { get; init; }

    /// <summary>
    /// <b>Сумма Платежа</b>
    /// </summary>
    public decimal PaymentAmount { get; init; }

    public bool IsDistributed { get; set; }

    /// <summary>
    /// Погашения за Период, относящиеся к данному Платежу
    /// </summary>
    public List<BillingPayoff> BillingsPayoffs { get; }

    /// <summary>
    /// Сумма Платежа, распределённая на оплату Штрафа 
    /// </summary>
    public ExpenseItem? Penalty { get; set; }

    public decimal PaymentToDistribute => PaymentAmount - CalculateDistributedPaymentSum();

    public void DistributeForPenalty(IPrioritizedExpenseItem item)
    {
        if (item.Priority != PaymentPriority.Penalty) return;
        if (IsDistributed) return;

        var paymentToDistribute = PaymentToDistribute;
        if (paymentToDistribute == 0) return;

        var paymentExpenseItems = CreatePaymentBillingItems();

        var min = decimal.Min(paymentToDistribute, item.Amount);
        item.ChangeAmount(-min);
        paymentToDistribute -= min;

        IncreasePaymentExpenseItem(paymentExpenseItems, item.Priority, min);

        if (paymentToDistribute == 0) IsDistributed = true;
    }

    public void DistributeOn(PeriodBilling periodBilling, List<IPrioritizedExpenseItem> items)
    {
        if (IsDistributed) return;

        var paymentToDistribute = PaymentToDistribute;
        if (paymentToDistribute == 0) return;

        var paymentExpenseItems = GetOrCreatePaymentBillingItems(periodBilling);

        foreach (var item in items)
        {
            if (paymentToDistribute == 0) break;

            var min = decimal.Min(paymentToDistribute, item.Amount);
            item.ChangeAmount(-min);
            paymentToDistribute -= min;

            IncreasePaymentExpenseItem(paymentExpenseItems, item.Priority, min);
        }

        if (paymentToDistribute == 0) IsDistributed = true;
    }

    private decimal CalculateDistributedPaymentSum() => BillingsPayoffs
        .Select(payoff => payoff.ExpenseItems.GetTotalSum())
        .Aggregate(0m, (accumulate, periodDistribution) => accumulate + periodDistribution) + (Penalty ?? 0m);

    private void IncreasePaymentExpenseItem(ExpenseItems paymentExpenseItems, PaymentPriority priority, decimal amount)
    {
        // todo: отрефакторить: перенести Penalty в один с paymentExpenseItems список. Для этого можно создать отдельный утилитарный класс PaymentExpenseItems
        if (priority == PaymentPriority.Penalty)
        {
            Penalty ??= ExpenseItem.Zero;
            Penalty.ChangeAmount(amount);
        }
        else paymentExpenseItems.ChangeBasedOnPriority(priority, amount);
    }

    private ExpenseItems CreatePaymentBillingItems()
    {
        var paymentExpenseItems = new ExpenseItems(0, 0, 0);
        BillingsPayoffs.Add(new BillingPayoff
        {
            PeriodBilling = null,
            ExpenseItems = paymentExpenseItems
        });

        return paymentExpenseItems;
    }

    private ExpenseItems GetOrCreatePaymentBillingItems(PeriodBilling periodBilling)
    {
        ExpenseItems paymentExpenseItems;

        if (BillingsPayoffs.Count == 0 || BillingsPayoffs.Last().PeriodBilling?.Id != periodBilling.Id)
        {
            paymentExpenseItems = new ExpenseItems(0, 0, 0);
            BillingsPayoffs.Add(new BillingPayoff
            {
                PeriodBilling = periodBilling,
                ExpenseItems = paymentExpenseItems
            });
        }
        else paymentExpenseItems = BillingsPayoffs.Last().ExpenseItems;

        return paymentExpenseItems;
    }
}

/// <summary>
/// <b>Погашение за Период</b>
/// </summary>
public class BillingPayoff
{
    public long Id { get; init; }

    /// <summary>
    /// Расчётный Период, к которому относится текущее Погашение 
    /// </summary>
    public required PeriodBilling? PeriodBilling { get; init; }

    /// <summary>
    /// Статьи, по которым распределась часть текущего платежа
    /// </summary>
    public required ExpenseItems ExpenseItems { get; init; }
}