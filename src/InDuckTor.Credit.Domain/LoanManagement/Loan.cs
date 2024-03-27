using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.LoanManagement.Models;
using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement;

public class Loan
{
    private const int DaysInRegularYear = 365;
    private const int DaysInLeapYear = 364;

    public static TimeSpan InterestAccrualFrequency { get; } = new(0, 1, 0);

    public Loan()
    {
        // EF Core constructor
        _paymentCalculator = InitPaymentCalculator();
    }

    public Loan(NewLoan newLoan)
    {
        ClientId = newLoan.ClientId;
        ClientAccountNumber = newLoan.ClientAccountNumber;
        BorrowedAmount = newLoan.BorrowedAmount;
        InterestRate = newLoan.InterestRate;
        PaymentType = newLoan.PaymentType;
        PaymentScheduleType = newLoan.PaymentScheduleType;
        PeriodInterval = newLoan.PeriodInterval;
        ApprovalDate = newLoan.ApprovalDate;

        State = LoanState.Approved;
        PlannedPaymentsNumber = CalculatePaymentsNumber(newLoan);
        Body = newLoan.BorrowedAmount;

        _paymentCalculator = InitPaymentCalculator();
    }

    public long Id { get; set; }

    public long ClientId { get; init; }

    /// <summary>
    /// <b>Ссудный счёт</b>
    /// </summary>
    public string? LoanAccountNumber { get; private set; }

    /// <summary>
    /// <b>Счёт Клиента</b>
    /// </summary>
    public string ClientAccountNumber { get; init; }

    /// <summary>
    /// <b>Сумма Кредита</b>: сколько заёмшик взял в долг
    /// </summary>
    public decimal BorrowedAmount { get; init; }

    /// <summary>
    /// <b>Процентная ставка</b> в диапазоне от 0 до 1
    /// </summary>
    public decimal InterestRate { get; init; }

    /// <summary>
    /// <b>Дата одобрения кредита</b>
    /// </summary>
    public DateTime ApprovalDate { get; init; }

    /// <summary>
    /// <b>Дата начисления кредитных средств</b>
    /// </summary>
    public DateTime? BorrowingDate { get; set; }

    /// <summary>
    /// <b>Статус Кредита</b>
    /// </summary>
    public LoanState State { get; private set; }

    /// <summary>
    /// <b>Планируемое число платежей</b>
    /// </summary>
    public int PlannedPaymentsNumber { get; private set; }

    /// <summary>
    /// <b>Тип Платежа</b>
    /// </summary>
    public PaymentType PaymentType { get; init; }

    /// <summary>
    /// <b>Тип Платёжного графика</b>
    /// </summary>
    public PaymentScheduleType PaymentScheduleType { get; init; }

    // Чтобы выяснить, что, помимо регулярного начисления, также нужно закрыть расчётный период,
    // необходимо будет подтянуть информацию об этом из LoanBilling
    public TimeSpan? PeriodInterval { get; init; }

    /// <summary>
    /// <b>Остаток по телу кредита</b>
    /// </summary>
    public ExpenseItem Body { get; set; }

    // todo: узнать, как работает погашение на самом деле: гасится полностью категория в рамках кредита или в рамках расчётного периода
    /// <summary>
    /// <b>Сумма Задолженности по Кредиту</b>
    /// </summary>
    public ExpenseItem Debt { get; private set; } = new();

    /// <summary>
    /// <b>Штраф по Задолженности</b>
    /// </summary>
    public ExpenseItem Penalty { get; private set; } = new();

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


    private readonly IPaymentCalculator _paymentCalculator;

    public bool IsRepaid => Body + Debt + Penalty == 0;

    public decimal MonthlyInterestRate => InterestRate / 12;

    public void StartNewPeriod()
    {
        if (PeriodAccruals != null && !IsCurrentPeriodEnded())
            throw Errors.Loan.CannotStartNewPeriod.NotEndedYet();
        _paymentCalculator.StartNewPeriod();
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        _paymentCalculator.AccrueInterestOnCurrentPeriod();
    }

    public decimal CalculateTickInterest() =>
        Body * InterestRate / (DateTime.IsLeapYear(DateTime.UtcNow.Year) ? DaysInLeapYear : DaysInRegularYear);

    public void ChargePenalty()
    {
        Penalty += Debt * PenaltyRate;
    }

    public void AddNewPeriodAndRecalculate(PeriodBilling periodBilling)
    {
        PeriodsBillings.Add(periodBilling);

        if (periodBilling.IsDebt)
        {
            Debt += periodBilling.GetRemainingInterest() + periodBilling.GetRemainingLoanBodyPayoff();
        }

        Body -= periodBilling.GetPaidLoanBody();
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
        ArgumentNullException.ThrowIfNull(PeriodAccruals);
        return PeriodAccruals.PeriodEndDate <= DateTime.UtcNow;
    }

    public void ActivateLoan()
    {
        State = LoanState.Active;
        StartNewPeriod();
    }

    public void CloseLoan()
    {
        if (!IsRepaid)
        {
            throw new Errors.Loan.InvalidLoanStateChange("Can't close the loan because it hasn't been repaid yet");
        }

        State = LoanState.Closed;
    }

    private static int CalculatePaymentsNumber(NewLoan newLoan)
    {
        if (newLoan.PaymentScheduleType == PaymentScheduleType.Calendar)
            return (int)Math.Round(newLoan.LoanTerm.Days / (double)30);

        ArgumentNullException.ThrowIfNull(newLoan.PeriodInterval);
        return (int)Math.Round(newLoan.LoanTerm / newLoan.PeriodInterval.Value);
    }

    private IPaymentCalculator InitPaymentCalculator()
    {
        return PaymentType switch
        {
            PaymentType.Annuity => new AnnuityPaymentCalculator(this),
            PaymentType.Differentiated => new DifferentiatedPaymentCalculator(this),
            _ => throw new ArgumentOutOfRangeException(nameof(PaymentType))
        };
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
    /// <b>Интервальный график</b>. Расчётный период не привязан к конкретной дате и длится всегда фиксированное время
    /// </summary>
    [EnumMember(Value = "interval")] Interval,

    /// <summary>
    /// <b>Календарный график</b>. Расчётный период привязан к конкретной дате
    /// <example>Если клиент взял кредит 10 числа какого-то месяца,
    /// то каждый новый Расчётный Период будет начинаться 10 числа следующего месяца</example>
    /// </summary>
    [EnumMember(Value = "calendar")] Calendar,
}

// todo: Досрочное погашение