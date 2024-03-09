using System.Runtime.CompilerServices;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.BillingPeriod;

/// <summary>
/// <b>Расчёт за Период</b>
/// </summary>
public class PeriodBilling
{
    public long Id { get; set; }

    public required Loan Loan { get; set; }

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
    public required BillingItems BillingItems { get; init; }

    /// <summary>
    /// Имел ли пользователь задолженность по Периоду, к которому относится этот Расчёт
    /// </summary>
    public bool IsDebt { get; set; } = false;

    /// <summary>
    /// <para>Сумма, которую осталось заплатить по Расчётному Периоду.</para>
    /// <para>Используется как утилитарное поле. Обозначает либо задолженность, либо,
    /// если <see cref="IsDebt"/> равен <c>false</c>, оставшуюся плату за Расчётный Период.</para>
    /// <para>Если плата за Расчётный Период полностью внесена, значение поля будет <c>null</c>.</para>
    /// </summary>
    public BillingItems? RemainingPayoff { get; set; }

    public bool IsPaid => RemainingPayoff == null;

    public decimal GetRemainingInterest()
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        return RemainingPayoff.Interest;
    }
    
    public decimal GetRemainingLoanBodyPayoff()
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        return RemainingPayoff.LoanBodyPayoff;
    }
    
    public decimal GetRemainingChargingForServices()
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        return RemainingPayoff.ChargingForServices;
    }

    public void ChangeInterest(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        RemainingPayoff.ChangeInterest(amount);
        RemoveRemainingPayoffIfEmpty();
    }

    public void ChangeLoanBodyPayoff(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        RemainingPayoff.ChangeLoanBodyPayoff(amount);
        RemoveRemainingPayoffIfEmpty();
    }

    public void ChangeChargingForServices(decimal amount)
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        RemainingPayoff.ChangeChargingForServices(amount);
        RemoveRemainingPayoffIfEmpty();
    }

    private void RemoveRemainingPayoffIfEmpty()
    {
        ArgumentNullException.ThrowIfNull(RemainingPayoff);
        if (RemainingPayoff.GetTotalSum() == 0) RemainingPayoff = null;
    }
}