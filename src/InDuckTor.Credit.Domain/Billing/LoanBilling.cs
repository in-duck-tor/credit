using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;

namespace InDuckTor.Credit.Domain.Billing;

public class LoanBilling
{
    public long Id { get; set; }

    public required Loan Loan { get; set; }

    public required BillingItem LoanBody { get; set; }

    // todo: узнать, как работает погашение на самом деле: гасится полностью категория в рамках кредита или в рамках расчётного периода
    /// <summary>
    /// <b>Задолженность по Кредиту</b>
    /// </summary>
    public BillingItem LoanDebt { get; private set; } = new();

    /// <summary>
    /// <b>Штраф по Задолженности</b>
    /// </summary>
    public BillingItem Penalty { get; private set; } = new();

    /// <summary>
    /// Процент Штрафа
    /// </summary>
    public const decimal PenaltyRate = 0.1m;

    public List<PeriodBilling> PeriodsBillings { get; set; } = [];

    /// <summary>
    /// <para><b>Начисления за текущий Период</b></para>
    /// <para>Если Кредит в состоянии<see cref="LoanState.Approved"/>, значение поля будет<c>null</c></para>
    /// </summary>
    public PeriodAccruals? PeriodAccruals { get; set; }

    public bool IsRepaid => LoanBody + LoanDebt + Penalty == 0;

    public void ChargePenalty()
    {
        Penalty += LoanDebt * PenaltyRate;
    }

    public void AddNewPeriodAndRecalculate(PeriodBilling periodBilling)
    {
        PeriodsBillings.Add(periodBilling);

        if (periodBilling.IsDebt)
        {
            LoanDebt += periodBilling.GetRemainingInterest() + periodBilling.GetRemainingLoanBodyPayoff();
        }

        LoanBody -= periodBilling.GetPaidLoanBody();
    }

    public List<IPrioritizedBillingItem> GetBillingItemsForPeriod(PeriodBilling periodBilling)
    {
        if (periodBilling.LoanBilling.Id != Id)
            throw Errors.EntitiesIsNotRelatedException.WithNames(nameof(LoanBilling), nameof(PeriodBilling));

        List<IPrioritizedBillingItem> items = [];

        if (periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.DebtInterest)
                .ChainWith(LoanDebt));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.DebtBody)
                .ChainWith(LoanBody)
                .ChainWith(LoanDebt));
        }

        items.Add(new PrioritizedBillingItem(PaymentPriority.Penalty, Penalty));

        if (!periodBilling.IsDebt)
        {
            items.Add(periodBilling.GetInterestItem(PaymentPriority.RegularInterest));
            items.Add(periodBilling.GetLoanBodyItem(PaymentPriority.RegularBody)
                .ChainWith(LoanBody));
        }

        items.Add(periodBilling.GetServicesItem(PaymentPriority.ChargingForServices));

        return items;
    }

    public void StartNewPeriod()
    {
        var now = DateTime.UtcNow;
        if (PeriodAccruals != null && PeriodAccruals.PeriodEndDate > now) return;
        // todo: переделать на generic получение
        ArgumentNullException.ThrowIfNull(Loan.PeriodInterval);

        var startDate = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, now.Kind);
        var endDate = now + Loan.PeriodInterval.Value;

        if (Loan.PaymentType == PaymentType.Annuity)
        {
            PeriodAccruals = new PeriodAccruals
            {
                PeriodStartDate = startDate,
                PeriodEndDate = endDate,
                OneTimePayment = 0
            };
        }
        else
        {
            PeriodAccruals = new PeriodAccruals
            {
                PeriodStartDate = startDate,
                PeriodEndDate = endDate,
                OneTimePayment = 0
            };
        }
    }

    public DateTime CurrentPeriodStartDate()
    {
        ArgumentNullException.ThrowIfNull(PeriodAccruals);
        return PeriodAccruals.PeriodStartDate;
    }
}

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
    public decimal InterestAccrual { get; set; } = 0;

    /// <summary>
    /// <b>Выплата по Телу Кредита</b>
    /// </summary>
    public decimal LoanBodyPayoff { get; set; } = 0;

    /// <summary>
    /// <b>Начисление платы за Услуги</b>
    /// </summary>
    public decimal ChargingForServices { get; set; } = 0;

    /// <summary>
    /// <b>Сумма единовременного Платежа</b>
    /// </summary>
    public decimal OneTimePayment { get; set; }
}