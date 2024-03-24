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
        _loan.PeriodAccruals = NewPeriodAccruals(startDate, endDate, _loan.Body);
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        ArgumentNullException.ThrowIfNull(_loan.PeriodAccruals);

        // Можно добавить начисление процентов за несколько тиков (если мы не успели обработать вовремя).
        // Тогда нужно будет также добавить закрытие нескольких расчётных периодов
        var interest = _loan.CalculateTickInterest();

        _loan.PeriodAccruals.InterestAccrual += interest;
        _loan.PeriodAccruals.OneTimePayment += interest;
    }

    private static PeriodAccruals NewPeriodAccruals(DateTime startDate, DateTime endDate, decimal fixedBody) => new()
    {
        PeriodStartDate = startDate,
        PeriodEndDate = endDate,
        OneTimePayment = fixedBody,
        LoanBodyPayoff = fixedBody,
        InterestAccrual = 0,
        ChargingForServices = 0
    };
}