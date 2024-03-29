namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

public class AnnuityPaymentCalculator : IPaymentCalculator
{
    private readonly Loan _loan;

    public AnnuityPaymentCalculator(Loan loan)
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
        periodAccruals.LoanBodyPayoff = GetExpectedBodyFromInterest(periodAccruals.InterestAccrual);
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        var periodAccruals = _loan.PeriodAccruals;
        ArgumentNullException.ThrowIfNull(periodAccruals);

        periodAccruals.InterestAccrual += _loan.CalculateTickInterest();
    }

    public decimal GetCurrentTotalPayment() => _loan.Debt + GetExpectedOneTimePayment();

    public decimal GetExpectedOneTimePayment()
    {
        var interest = GetExpectedInterest();
        var body = GetExpectedBodyFromInterest(interest);

        return body + interest;
    }

    public decimal GetExpectedBody()
    {
        var interest = GetExpectedInterest();
        var regularOneTimePayment = CalculateRegularOneTimePayment();
        var regularBody = regularOneTimePayment - interest;

        return Math.Min(_loan.BodyAfterPayoffs, regularBody);
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

    private decimal GetExpectedBodyFromInterest(decimal interest)
    {
        var regularOneTimePayment = CalculateRegularOneTimePayment();
        var regularBody = regularOneTimePayment - interest;

        return Math.Min(_loan.BodyAfterPayoffs, regularBody);
    }

    // Если ввести округление, то платёж также нужно будет распределять на округление.
    // То есть после распределения по процентам и телу, нужно будет распределить платёж по сумме округления.
    // Скорее всего сумма округления должна быть отдельной категорией, то есть отдельным столбцом.
    private decimal CalculateOneTimePayment()
    {
        var regularOneTimePayment = CalculateRegularOneTimePayment();

        // добавить расчёт тела на текущий период
        var periodInterest = _loan.CurrentBody * _loan.PeriodInterestRate;
        var regularPaymentBody = regularOneTimePayment - periodInterest;

        if (regularPaymentBody <= _loan.CurrentBody) return regularOneTimePayment;

        // Последний платёж
        return _loan.CurrentBody + periodInterest;
    }

    private decimal CalculateRegularOneTimePayment()
    {
        var periodInterestRate = (double)_loan.PeriodInterestRate;
        var k = (decimal)(periodInterestRate / (1 - Math.Pow(1 + periodInterestRate, -_loan.PlannedPaymentsNumber)));
        return _loan.BorrowedAmount * k;
    }

    private static PeriodAccruals NewPeriodAccruals(DateTime startDate, DateTime endDate) =>
        new()
        {
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            LoanBodyPayoff = 0,
            InterestAccrual = 0,
            ChargingForServices = 0
        };
}