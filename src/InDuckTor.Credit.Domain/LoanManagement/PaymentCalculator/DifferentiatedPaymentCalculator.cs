namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

class DifferentiatedPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterestOnCurrentPeriod(Loan loan)
    {
        ArgumentNullException.ThrowIfNull(loan.LoanBilling.PeriodAccruals);
        
        var interest = _interestCalculator.InterestAccrual(loan.LoanBilling.LoanBody, loan.InterestRate);

        loan.LoanBilling.PeriodAccruals.InterestAccrual += interest;
    }
}