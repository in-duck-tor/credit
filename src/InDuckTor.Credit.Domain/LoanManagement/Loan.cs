using InDuckTor.Credit.Domain.BillingPeriod;

namespace InDuckTor.Credit.Domain.LoanManagement;

public class Loan
{
    public long Id { get; set; }

    /// <summary>
    /// <b>Сумма Кредита</b>: сколько заёмшик взял в долг
    /// </summary>
    public required decimal BorrowedAmount { get; set; }

    /// <summary>
    /// <b>Процентная ставка</b> в диапазоне от 0 до 1
    /// </summary>
    public required decimal InterestRate { get; set; }

    /// <summary>
    /// <b>Дата одобрения кредита</b>
    /// </summary>
    public required DateTime ApprovalDate { get; set; }

    /// <summary>
    /// <b>Дата начисления кредитных средств</b>
    /// </summary>
    public DateTime? BorrowingDate { get; set; }

    /// <summary>
    /// <b>Статус Кредита</b>
    /// </summary>
    public required LoanState State { get; set; }

    /// <summary>
    /// <b>Планируемое число платежей</b>
    /// </summary>
    public required int PlannedPaymentsNumber { get; set; }

    /// <summary>
    /// <b>Тип Платежа</b>
    /// </summary>
    public required PaymentType PaymentType { get; set; }

    /// <summary>
    /// <b>Тип Платёжного графика</b>
    /// </summary>
    public required PaymentScheduleType PaymentScheduleType { get; set; }

    /// <summary>
    /// <b>Остаток по Кредиту</b>
    /// </summary>
    public required decimal LoanBody { get; set; }

    /// <summary>
    /// <b>Задолженность по Кредиту</b>
    /// </summary>
    public decimal LoanDebt { get; set; }

    /// <summary>
    /// <b>Штраф по Задолженности</b>
    /// </summary>
    public decimal Penalty { get; set; }

    /// <summary>
    /// Процент Штрафа
    /// </summary>
    public const decimal PenaltyRate = 0.1m;

    /// <summary>
    /// <para><b>Начисления за текущий Период</b></para>
    /// <para>Если Кредит в состоянии<see cref="LoanState.Approved"/>, значение поля будет<c>null</c></para>
    /// </summary>
    public PeriodAccruals? PeriodAccruals { get; set; }

    /// <summary>
    /// <b>Расчёт за Период</b>
    /// </summary>
    public List<PeriodBilling> PeriodsBillings { get; set; } = [];

    public void AddPeriodBilling(PeriodBilling periodBilling)
    {
        PeriodsBillings.Add(periodBilling);
        // LoanBody -=
    }
}

/// <summary>
/// <b>Начисления за Период</b>
/// </summary>
public class PeriodAccruals
{
    /// <summary>
    /// <b>Дата начала периода</b>
    /// </summary>
    public DateTime PeriodStartDate { get; set; }

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

    /// <summary>
    /// <b>Сумма единовременного Платежа</b>
    /// </summary>
    public decimal OneTimePayment { get; set; }
}

/// <summary>
/// <b>Статус Кредита</b>
/// </summary>
public enum LoanState
{
    /// <summary>
    /// Кредит одобрен, но деньги не переведены на счёт заёмщика
    /// </summary>
    Approved,

    /// <summary>
    /// Кредит одобрен и деньги переведены на счёт заёмщика
    /// </summary>
    Active,

    /// <summary>
    /// Кредит погашен
    /// </summary>
    Closed
}

/// <summary>
/// <b>Тип Платежа по Кредиту</b>
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// <b>Аннуитетный Платёж</b>
    /// </summary>
    Annuity,

    /// <summary>
    /// <b>Дифференцированный Платёж</b>
    /// </summary>
    Differentiated
}

/// <summary>
/// <b>Платёжный график</b>. Определяет частоту создания новых Расчётных Периодов 
/// </summary>
public enum PaymentScheduleType
{
    /// <summary>
    /// <b>Календарный график</b>. Расчётный период привязан к конкретной дате
    /// <example>Если клиент взял кредит 10 числа какого-то месяца,
    /// то каждый новый Расчётный Период будет начинаться 10 числа следующего месяца</example>
    /// </summary>
    Calendar,

    /// <summary>
    /// <b>Интервальный график</b>. Расчётный период не привязан к конкретной дате и длится всегда фиксированное время
    /// </summary>
    Interval
}

// todo: Подключение расчётного счёта и создание ссудного счёта
// todo: Досрочное погашение
// todo: Перевод средств на расчётный счёт
// todo: Расчёт штрафов
// todo: 