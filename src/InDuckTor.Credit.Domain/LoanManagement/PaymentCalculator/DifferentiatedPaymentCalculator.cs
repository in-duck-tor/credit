namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

class DifferentiatedPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterestOnCurrentPeriod(Loan loan)
    {
        ArgumentNullException.ThrowIfNull(loan.PeriodAccruals);

        var interest = _interestCalculator.InterestAccrual(loan.LoanBody, loan.InterestRate);

        loan.PeriodAccruals.InterestAccrual += interest;
    }
}