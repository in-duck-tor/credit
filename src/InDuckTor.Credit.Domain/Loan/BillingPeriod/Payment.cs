using InDuckTor.Credit.Domain.Loan.BillingPeriod;

namespace InDuckTor.Credit.Domain.Loan;

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
    public PaymentDistribution? PaymentDistribution { get; set; }
}

/// <summary>
/// <b>Распределение Платежа</b> по различным категориям
/// </summary>
public class PaymentDistribution
{
    public long Id { get; set; }

    /// <summary>
    /// Погашения за Период, относящиеся к данному Платежу
    /// </summary>
    public List<BillingPayoff> BillingsPayoffs { get; init; } = [];

    /// <summary>
    /// Сумма Платежа, распределённая на оплату Штрафа 
    /// </summary>
    public decimal? Penalty { get; set; }
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