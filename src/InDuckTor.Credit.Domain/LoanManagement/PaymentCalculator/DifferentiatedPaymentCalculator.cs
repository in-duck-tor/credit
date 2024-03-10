namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

class DifferentiatedPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();

    // todo: Подумать над тем, чтобы избавиться от этих классов и перенести их в расчёт
    public void AccrueInterestOnCurrentPeriod(Loan loan)
    {
        ArgumentNullException.ThrowIfNull(loan.LoanBilling.PeriodAccruals);
        
        var interest = _interestCalculator.InterestAccrual(loan.LoanBilling.LoanBody, loan.InterestRate);

        loan.LoanBilling.PeriodAccruals.InterestAccrual += interest;
    }
}