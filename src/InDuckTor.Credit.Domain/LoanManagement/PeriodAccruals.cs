namespace InDuckTor.Credit.Domain.LoanManagement;

/*
 * Аннуитетный:
 * OneTimePayment фиксирована
 * InterestAccrual увеличивается
 * LoanBodyPayoff уменьшается
 *
 * Дифференцированный:
 * OneTimePayment увеличивается
 * InterestAccrual увеличивается
 * LoanBodyPayoff фиксирована
 */

/// <summary>
/// <b>Начисления за Период</b>
/// </summary>
public class PeriodAccruals
{
    /// <summary>
    /// <b>Дата начала периода</b>
    /// </summary>
    public required DateTime PeriodStartDate { get; set; }

    /// <summary>
    /// <b>Дата конца периода</b>
    /// </summary>
    public required DateTime PeriodEndDate { get; set; }

    /// <summary>
    /// <b>Начисление процентов</b>
    /// </summary>
    public decimal InterestAccrual { get; set; }

    /// <summary>
    /// <b>Выплата по Телу Кредита</b>
    /// </summary>
    public decimal LoanBodyPayoff { get; set; }

    /// <summary>
    /// <b>Начисление платы за Услуги</b>
    /// </summary>
    public decimal ChargingForServices { get; set; }

    public decimal CurrentOneTimePayment => InterestAccrual + LoanBodyPayoff + ChargingForServices;
}