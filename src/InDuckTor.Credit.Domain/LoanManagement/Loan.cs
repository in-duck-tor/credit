using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.LoanManagement.Models;

namespace InDuckTor.Credit.Domain.LoanManagement;

// todo: Переделать на конструктор, чтобы использовать приватные сеттеры
public class Loan
{
    public static TimeSpan InterestAccrualFrequency { get; } = new(0, 1, 0);

    public long Id { get; set; }

    public long ClientId { get; set; }

    /// <summary>
    /// <b>Ссудный счёт</b>
    /// </summary>
    public string LoanAccountNumber { get; private set; }

    /// <summary>
    /// <b>Счёт Клиента</b>
    /// </summary>
    public required string ClientAccountNumber { get; set; }

    /// <summary>
    /// <b>Сумма Кредита</b>: сколько заёмшик взял в долг
    /// </summary>
    public required decimal BorrowedAmount { get; init; }

    /// <summary>
    /// <b>Процентная ставка</b> в диапазоне от 0 до 1
    /// </summary>
    public required decimal InterestRate { get; init; }

    /// <summary>
    /// <b>Дата одобрения кредита</b>
    /// </summary>
    public required DateTime ApprovalDate { get; init; }

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

    // Чтобы выяснить, что, помимо регулярного начисления, также нужно закрыть расчётный период,
    // необходимо будет подтянуть информацию об этом из LoanBilling
    public TimeSpan? PeriodInterval { get; set; }

    public static Loan CreateNewLoan(NewLoan newLoan)
    {
        var loan = new Loan
        {
            ClientId = newLoan.ClientId,
            ClientAccountNumber = newLoan.ClientAccountNumber,
            BorrowedAmount = newLoan.BorrowedAmount,
            InterestRate = newLoan.InterestRate,
            ApprovalDate = newLoan.ApprovalDate,
            State = LoanState.Approved,
            PlannedPaymentsNumber = CalculatePaymentsNumber(newLoan),
            PaymentType = newLoan.PaymentType,
            PaymentScheduleType = newLoan.PaymentScheduleType,
            PeriodInterval = newLoan.PeriodInterval
        };

        loan.LoanBilling = new LoanBilling
        {
            LoanBody = loan.BorrowedAmount,
            Loan = loan,
        };

        return loan;
    }

    public void AttachLoanAccount(string accountNumber)
    {
        LoanAccountNumber = accountNumber;
    }

    public TimeSpan PeriodDuration()
    {
        ArgumentNullException.ThrowIfNull(PeriodInterval);

        // todo: Добавить вариант для календарного графика
        return PeriodInterval.Value;
    }

    public bool IsCurrentPeriodEnded()
    {
        // if (PaymentScheduleType == PaymentScheduleType.Calendar) return DateTime.UtcNow.Day == PeriodDay;
        ArgumentNullException.ThrowIfNull(PeriodInterval);
        return LoanBilling.CurrentPeriodStartDate().Add(PeriodInterval.Value) <= DateTime.UtcNow;
    }

    private static int CalculatePaymentsNumber(NewLoan newLoan)
    {
        if (newLoan.PaymentScheduleType == PaymentScheduleType.Calendar)
            return (int)Math.Round(newLoan.LoanTerm.Days / (double)30);

        ArgumentNullException.ThrowIfNull(newLoan.PeriodInterval);
        return (int)Math.Round(newLoan.LoanTerm / newLoan.PeriodInterval.Value);
    }
}

public static class TimeSpanExtensions
{
    public static string ToCron(this TimeSpan timeSpan)
    {
        List<string> cron = ["*", "*", "*", "*", "*"];

        string.Format("{1} {2} {3} {4} {5}",
            timeSpan.Days
        );
        return "";
    }
}

public class D
{
    public static void Main()
    {
        // var interestAccrualFreq = new TimeSpan(1, 0, 0, 0);
        var interestAccrualFreq = new TimeSpan(10, 0, 2, 30);
        var loanSpan = new TimeSpan(20, 0, 4, 28);

        var c = loanSpan / interestAccrualFreq;
        var cRounded = Math.Round(c);
        var realLoanSpan = interestAccrualFreq * cRounded;

        Console.WriteLine(c);
        Console.WriteLine(c % 1);
        Console.WriteLine(cRounded);
        Console.WriteLine(realLoanSpan);

        var time = new TimeSpan(-10, 10, 10);
        Console.WriteLine(time);
        Console.WriteLine(time.Duration());

        Console.WriteLine((int)Math.Round(new TimeSpan(860, 0, 0, 0).Days / (double)30));
    }
}

// В расписании должно быть: длительность расчётного периода/день, в который начинается расчётный период

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

// todo: Досрочное погашение