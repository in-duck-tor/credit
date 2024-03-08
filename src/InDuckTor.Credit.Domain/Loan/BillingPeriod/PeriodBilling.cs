using System.Collections.Immutable;

namespace InDuckTor.Credit.Domain.Loan.BillingPeriod;

/// <summary>
/// <b>Расчёт за Период</b>
/// </summary>
public class PeriodBilling
{
    public long Id { get; set; }

    /// <summary>
    /// <b>Дата начала периода</b>
    /// </summary>
    public required DateTime PeriodStartDate { get; init; }

    /// <summary>
    /// <b>Дата начала периода</b>
    /// </summary>
    public required DateTime PeriodEndDate { get; init; }

    /// <summary>
    /// <b>Сумма единовременного Платежа</b>
    /// </summary>
    public required decimal OneTimePayment { get; init; }

    /// <summary>
    /// <b>Статьи расчёта</b>
    /// </summary>
    public required IImmutableList<BillingItems> BillingItems { get; init; }
    
    /// <summary>
    /// <para><b>Задолженность за Период</b></para>
    /// <para> Если Задолженности нет, значение будет<c>null</c> </para>
    /// </summary>
    public BillingItems? PeriodDebt { get; set; }
}