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

        periodAccruals.InterestAccrual += _loan.CalculateTickInterest();
        periodAccruals.LoanBodyPayoff -= periodAccruals.InterestAccrual;
    }

    // todo: Решить проблему с сохранением платежей
    // todo: Решить проблему с бесконечными расчётными периодами: после того, как сумма погашений по телу станет равна остатку по кредиту,
    //  создавать расчётные периоды с нулевым погашением по телу.
    // todo: Генерализировать крайний случай с последним платежём, когда общая сумма меньше фиксированного платежа
    //  (проценты увеличиваются в ущерб телу, если общая их сумма равна фиксу)
    private decimal CalculateOneTimePayment()
    {
        var monthlyInterestRate = (double)_loan.MonthlyInterestRate;
        return _loan.BorrowedAmount * (decimal)Math.Ceiling(
            monthlyInterestRate / (1 - Math.Pow(1 + monthlyInterestRate, -_loan.PlannedPaymentsNumber))
        );
    }

    private static PeriodAccruals NewPeriodAccruals(DateTime startDate, DateTime endDate, decimal oneTimePayment) => new()
    {
        PeriodStartDate = startDate,
        PeriodEndDate = endDate,
        OneTimePayment = oneTimePayment,
        LoanBodyPayoff = oneTimePayment,
        InterestAccrual = 0,
        ChargingForServices = 0
    };
}