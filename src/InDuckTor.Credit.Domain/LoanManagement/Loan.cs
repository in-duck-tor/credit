using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.Billing.Period;

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
    /// <b>Расчёт кредита</b>
    /// </summary>
    public LoanBilling LoanBilling { get; set; }

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
}

/// <summary>
/// <b>Статус Кредита</b>
/// </summary>
public enum LoanState
{
    /// <summary>
    /// Кредит одобрен, но деньги не переведены на счёт заёмщика
    /// </summary>
    [EnumMember(Value = "approved")] Approved,

    /// <summary>
    /// Кредит одобрен и деньги переведены на счёт заёмщика
    /// </summary>
    [EnumMember(Value = "active")] Active,

    /// <summary>
    /// Кредит погашен
    /// </summary>
    [EnumMember(Value = "closed")] Closed
}

/// <summary>
/// <b>Тип Платежа по Кредиту</b>
/// </summary>
public enum PaymentType
{
    /// <summary>
    /// <b>Аннуитетный Платёж</b>
    /// </summary>
    [EnumMember(Value = "annuity")] Annuity,

    /// <summary>
    /// <b>Дифференцированный Платёж</b>
    /// </summary>
    [EnumMember(Value = "differentiated")] Differentiated
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
    [EnumMember(Value = "calendar")] Calendar,

    /// <summary>
    /// <b>Интервальный график</b>. Расчётный период не привязан к конкретной дате и длится всегда фиксированное время
    /// </summary>
    [EnumMember(Value = "interval")] Interval
}

// todo: Подключение расчётного счёта и создание ссудного счёта
// todo: Досрочное погашение
// todo: Перевод средств на расчётный счёт
// todo: Расчёт штрафов
// todo: 