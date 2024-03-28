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
        var oneTimePayment = CalculateOneTimePayment();

        _loan.PeriodAccruals = NewPeriodAccruals(startDate, endDate, oneTimePayment);
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        var periodAccruals = _loan.PeriodAccruals;
        ArgumentNullException.ThrowIfNull(periodAccruals);
        
        var interestAccrual = _loan.CalculateTickInterest();
        // Добавить изменение тела, если это последний(ие) платёж(и)
        periodAccruals.InterestAccrual += interestAccrual;
        periodAccruals.LoanBodyPayoff -= interestAccrual;
    }

    public decimal GetPlannedOneTimePayment() => CalculateOneTimePayment();

    // Если ввести округление, то платёж также нужно будет распределять на округление.
    // То есть после распределения по процентам и телу, нужно будет распределить платёж по сумме округления.
    // Скорее всего сумма округления должна быть отдельной категорией, то есть отдельным столбцом.
    private decimal CalculateOneTimePayment()
    {
        var periodInterestRate = (double)_loan.GetPeriodInterestRate();
        var k = (decimal)(periodInterestRate / (1 - Math.Pow(1 + periodInterestRate, -_loan.PlannedPaymentsNumber)));
        var regularOneTimePayment = _loan.BorrowedAmount * k;

        // добавить расчёт тела на текущий период
        var periodInterest = _loan.CurrentBody * _loan.GetPeriodInterestRate();
        var regularPaymentBody = regularOneTimePayment - periodInterest;

        if (regularPaymentBody <= _loan.CurrentBody) return regularOneTimePayment;

        // Последний платёж
        return _loan.CurrentBody + periodInterest;
    }

    private static PeriodAccruals NewPeriodAccruals(DateTime startDate, DateTime endDate, decimal oneTimePayment) =>
        new()
        {
            PeriodStartDate = startDate,
            PeriodEndDate = endDate,
            OneTimePayment = oneTimePayment,
            LoanBodyPayoff = oneTimePayment,
            InterestAccrual = 0,
            ChargingForServices = 0
        };
}