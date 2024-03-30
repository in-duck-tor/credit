using System.Runtime.Serialization;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.LoanManagement.Models;
using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;
using InDuckTor.Credit.Domain.LoanManagement.State;

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
        PaymentCalculator = InitPaymentCalculator();
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

        PaymentCalculator = InitPaymentCalculator();
    }

    public long Id { get; set; }

    public long ClientId { get; init; }

    /// <summary>
    /// <b>Ссудный счёт</b>
    /// </summary>
    public string? LoanAccountNumber { get; protected internal set; }

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
    public LoanState State { get; private set; }

    /// <summary>
    /// <b>Планируемое число платежей</b>
    /// </summary>
    public int PlannedPaymentsNumber { get; init; }

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

    protected internal readonly IPaymentCalculator PaymentCalculator;

    private ILoanState? _stateInteractor;

    private ILoanState StateInteractor
    {
        get => _stateInteractor ??= InitState();
        set => _stateInteractor = value;
    }

    public bool IsRepaid => CurrentBody + Debt + Penalty == 0;
    public bool IsClientAhuel => PeriodsBillings.Count - PlannedPaymentsNumber >= 3;

    public TimeSpan PeriodDuration
    {
        // todo: Добавить вариант для календарного графика
        get
        {
            ArgumentNullException.ThrowIfNull(PeriodInterval);
            return PeriodInterval.Value;
        }
    }

    // Возмонжо стоит определять не годовой процент, а процент для минимального срока взятия кредита
    public decimal PeriodInterestRate => InterestRate / (decimal)(YearDuration / PeriodDuration);
    public decimal TickInterestRate => InterestRate / (decimal)(YearDuration / InterestAccrualFrequency);
    public ExpenseItem AccruedInterest => PeriodAccruals?.InterestAccrual ?? 0;

    public void StartNewPeriod() => StateInteractor.StartNewPeriod();

    public PeriodBilling ClosePeriod() => StateInteractor.ClosePeriod();

    public decimal GetCurrentTotalPayment() => StateInteractor.GetCurrentTotalPayment();
    public decimal GetExpectedOneTimePayment() => StateInteractor.GetExpectedOneTimePayment();

    public void AccrueInterestOnCurrentPeriod() => StateInteractor.AccrueInterestOnCurrentPeriod();

    public decimal CalculateTickInterest() => StateInteractor.CalculateTickInterest();

    public void ChargePenalty() => StateInteractor.ChargePenalty();

    public void AttachLoanAccount(string accountNumber) => StateInteractor.AttachLoanAccount(accountNumber);

    public bool IsCurrentPeriodEnded() => StateInteractor.IsCurrentPeriodEnded();

    public void ActivateLoan() => StateInteractor.ActivateLoan();

    public void CloseLoan() => StateInteractor.CloseLoan();

    public void SellToCollectors() => StateInteractor.SellToCollectors();

    protected internal void SetState(LoanState state)
    {
        State = state;
        StateInteractor = LoanStateFactory.CreateState(this);
    }

    private static int CalculatePaymentsNumber(NewLoan newLoan)
    {
        if (newLoan.PaymentScheduleType == PaymentScheduleType.Calendar)
            return (int)Math.Round(newLoan.LoanTerm.Days / (double)30);

        ArgumentNullException.ThrowIfNull(newLoan.PeriodInterval);
        
        var paymentsNumber = (int)Math.Round(newLoan.LoanTerm / newLoan.PeriodInterval.Value);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(paymentsNumber, 0);

        return paymentsNumber;
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

    private ILoanState InitState()
    {
        return LoanStateFactory.CreateState(this);
    }
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

// В расписании должно быть: длительность расчётного периода/день, в который начинается расчётный период
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