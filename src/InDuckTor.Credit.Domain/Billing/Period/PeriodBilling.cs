using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing.Period;

/// <summary>
/// <b>Расчёт за Период</b>
/// </summary>
public class PeriodBilling
{
    public long Id { get; set; }

    public required Loan Loan { get; init; }

    /// <summary>
    /// <b>Дата начала периода</b>
    /// </summary>
    public required DateTime PeriodStartDate { get; init; }

    /// <summary>
    /// <b>Дата окончания периода</b>
    /// </summary>
    public required DateTime PeriodEndDate { get; init; }

    /// <summary>
    /// <b>Сумма единовременного Платежа</b>
    /// </summary>
    public required decimal OneTimePayment { get; init; }

    /// <summary>
    /// <b>Статьи расчёта</b>
    /// </summary>
    public required ExpenseItems ExpenseItems { get; init; }

    /// <summary>
    /// Имел ли пользователь задолженность по Периоду, к которому относится этот Расчёт
    /// </summary>
    public bool IsDebt { get; set; }

    /// <summary>
    /// <para>Сумма, которую осталось заплатить по Расчётному Периоду.</para>
    /// <para>Используется как утилитарное поле. Обозначает либо задолженность, либо,
    /// если <see cref="IsDebt"/> равен <c>false</c>, оставшуюся плату за Расчётный Период.</para>
    /// <para>Если плата за Расчётный Период полностью внесена, значение поля будет <c>null</c>.</para>
    /// </summary>
    public ExpenseItems? RemainingPayoff { get; set; }

    public bool IsPaid => RemainingPayoff == null || RemainingPayoff.GetTotalSum() == 0;

    public decimal TotalRemainingPayment =>
        GetRemainingInterest() + GetRemainingLoanBodyPayoff() + GetRemainingChargingForServices();

    public decimal GetRemainingInterest() => RemainingPayoff?.Interest ?? 0;

    public decimal GetRemainingLoanBodyPayoff() => RemainingPayoff?.LoanBodyPayoff ?? 0;

    public decimal GetRemainingChargingForServices() => RemainingPayoff?.ChargingForServices ?? 0;
}