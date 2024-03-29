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

    private static readonly TimeSpan YearDuration = TimeSpan.FromDays(
        DateTime.IsLeapYear(DateTime.UtcNow.Year) ? DaysInLeapYear : DaysInRegularYear
    );

    public static TimeSpan InterestAccrualFrequency { get; } = new(0, 1, 0);

    // ReSharper disable once UnusedMember.Local
#pragma warning disable CS8618
    private Loan()
#pragma warning restore CS8618
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
        CurrentBody = newLoan.BorrowedAmount;
        BodyAfterPayoffs = newLoan.BorrowedAmount;
        Debt = ExpenseItem.Zero;
        Penalty = ExpenseItem.Zero;

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
    /// <b>Годовая Процентная ставка</b> в диапазоне от 0 до 1
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
    // todo: реализовать паттерн State для всех состояний, чтобы избежать использования методов, запрещённых для разных состояний кредита
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
    public ExpenseItem CurrentBody { get; init; }

    public ExpenseItem BodyAfterPayoffs { get; init; }

    // todo: узнать, как работает погашение на самом деле: гасится полностью категория в рамках кредита или в рамках расчётного периода
    /// <summary>
    /// <b>Сумма Задолженности по Кредиту</b>. В неё входит задолженность по телу и процентам.
    /// </summary>
    public ExpenseItem Debt { get; init; }

    /// <summary>
    /// <b>Штраф по Задолженности</b>
    /// </summary>
    public ExpenseItem Penalty { get; init; }

    /// <summary>
    /// Процент Штрафа
    /// </summary>
    public const decimal PenaltyRate = 0.1m;

    public List<PeriodBilling> PeriodsBillings { get; init; } = [];

    /// <summary>
    /// <para><b>Начисления за текущий Период</b></para>
    /// <para>Если Кредит в состоянии<see cref="LoanState.Approved"/>, значение поля будет<c>null</c></para>
    /// </summary>
    public PeriodAccruals? PeriodAccruals { get; set; }


    private readonly IPaymentCalculator _paymentCalculator;

    public bool IsRepaid => CurrentBody + Debt + Penalty == 0;

    public TimeSpan PeriodDuration
    {
        get
        {
            ArgumentNullException.ThrowIfNull(PeriodInterval);

            // todo: Добавить вариант для календарного графика
            return PeriodInterval.Value;
        }
    }

    // Возмонжо стоит определять не годовой процент, а процент для минимального срока взятия кредита
    public decimal PeriodInterestRate => InterestRate / (decimal)(YearDuration / PeriodDuration);
    public decimal TickInterestRate => InterestRate / (decimal)(YearDuration / InterestAccrualFrequency);
    public ExpenseItem AccruedInterest => PeriodAccruals?.InterestAccrual ?? 0;

    public void StartNewPeriod()
    {
        if (State == LoanState.Closed) throw Errors.Loan.InvalidLoanState.Closed(Id);
        if (PeriodAccruals != null && !IsCurrentPeriodEnded())
            throw Errors.Loan.CannotStartNewPeriod.NotEndedYet();
        _paymentCalculator.StartNewPeriod();
    }

    public PeriodBilling ClosePeriod()
    {
        ArgumentNullException.ThrowIfNull(PeriodAccruals);

        _paymentCalculator.ClosePeriod();

        var billingItems = new ExpenseItems(
            PeriodAccruals.InterestAccrual,
            PeriodAccruals.LoanBodyPayoff,
            PeriodAccruals.ChargingForServices);

        var periodBilling = new PeriodBilling
        {
            Loan = this,
            PeriodStartDate = PeriodAccruals.PeriodStartDate,
            PeriodEndDate = PeriodAccruals.PeriodEndDate,
            OneTimePayment = PeriodAccruals.CurrentOneTimePayment,
            ExpenseItems = billingItems,
            RemainingPayoff = billingItems.DeepCopy(),
        };

        PeriodsBillings.Add(periodBilling);
        BodyAfterPayoffs.ChangeAmount(-periodBilling.ExpenseItems.LoanBodyPayoff);

        return periodBilling;
    }

    public decimal GetCurrentTotalPayment() => _paymentCalculator.GetCurrentTotalPayment();
    public decimal GetExpectedOneTimePayment() => _paymentCalculator.GetExpectedOneTimePayment();

    public void AccrueInterestOnCurrentPeriod() => _paymentCalculator.AccrueInterestOnCurrentPeriod();

    public decimal CalculateTickInterest() => CurrentBody * TickInterestRate;

    public void ChargePenalty()
    {
        Penalty.ChangeAmount(Debt * PenaltyRate);
    }

    public void AttachLoanAccount(string accountNumber)
    {
        LoanAccountNumber = accountNumber;
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
            throw new Errors.Loan.InvalidLoanStateChange("Can't close the loan because it hasn't been repaid yet");

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