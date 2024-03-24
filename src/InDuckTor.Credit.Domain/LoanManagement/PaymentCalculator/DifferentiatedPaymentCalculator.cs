namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

public class DifferentiatedPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();
    private readonly Loan _loan;

    public DifferentiatedPaymentCalculator(Loan loan)
    {
        _loan = loan;
    }

    // todo: Подумать над тем, чтобы перенести оба класса в расчёт в виде стратегии и добавить в них ссылку на расчёт
    // todo: Добавить логику расчёта фиксированного единовременного платежа (для аннуитетного) и фиксированного тела (для диффиринцированного)
    public void AccrueInterestOnCurrentPeriod()
    {
        ArgumentNullException.ThrowIfNull(_loan.LoanBilling.PeriodAccruals);

        var interest = _interestCalculator.InterestAccrual(_loan.LoanBilling.LoanBody, _loan.InterestRate);

        _loan.LoanBilling.PeriodAccruals.InterestAccrual += interest;
        _loan.LoanBilling.PeriodAccruals.OneTimePayment += interest;
    }
}