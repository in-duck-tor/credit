namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

public class DifferentiatedPaymentCalculator : IPaymentCalculator
{
    private readonly Loan _loan;

    public DifferentiatedPaymentCalculator(Loan loan)
    {
        _loan = loan;
    }

    public void StartNewPeriod()
    {
        var (startDate, endDate) = _loan.GetNewPeriodDates();
        _loan.PeriodAccruals = NewPeriodAccruals(startDate, endDate);
    }

    public void ClosePeriod()
    {
        var periodAccruals = _loan.PeriodAccruals;
        ArgumentNullException.ThrowIfNull(periodAccruals);
        periodAccruals.LoanBodyPayoff = GetExpectedBody();
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        ArgumentNullException.ThrowIfNull(_loan.PeriodAccruals);

        // Можно добавить начисление процентов за несколько тиков (если мы не успели обработать вовремя).
        // Тогда нужно будет также добавить закрытие нескольких расчётных периодов
        _loan.PeriodAccruals.InterestAccrual += _loan.CalculateTickInterest();
    }

    public decimal GetCurrentTotalPayment() => _loan.Debt + GetExpectedOneTimePayment();

    public decimal GetExpectedOneTimePayment() => GetExpectedBody() + GetExpectedInterest();

    public decimal GetExpectedBody()
    {
        var regularFixedBody = _loan.BorrowedAmount / _loan.PlannedPaymentsNumber;
        if (regularFixedBody <= _loan.BodyAfterPayoffs) return regularFixedBody;

        // Последний платёж
        return _loan.BodyAfterPayoffs;
    }

    public decimal GetExpectedInterest()
    {
        var periodAccruals = _loan.PeriodAccruals;
        var accruedInterest = periodAccruals?.InterestAccrual ?? 0;
        var periodTimeLeft = (periodAccruals?.PeriodEndDate - DateTime.UtcNow)?.Duration() ?? _loan.PeriodDuration;
        var numberOfAccrualsLeft = (int)(periodTimeLeft / Loan.InterestAccrualFrequency);
        var interestAfterPayoffs = _loan.BodyAfterPayoffs * _loan.TickInterestRate * numberOfAccrualsLeft;

        return accruedInterest + interestAfterPayoffs;
    }

    private static PeriodAccruals NewPeriodAccruals(DateTime startDate, DateTime endDate) => new()
    {
        PeriodStartDate = startDate,
        PeriodEndDate = endDate,
        LoanBodyPayoff = 0,
        InterestAccrual = 0,
        ChargingForServices = 0
    };
}