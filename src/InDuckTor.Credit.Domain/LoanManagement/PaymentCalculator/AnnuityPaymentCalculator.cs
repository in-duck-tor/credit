namespace InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

class AnnuityPaymentCalculator : IPaymentCalculator
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterestOnCurrentPeriod(Loan loan)
    {
        var billing = loan.LoanBilling;
        ArgumentNullException.ThrowIfNull(billing.PeriodAccruals);

        var interest = _interestCalculator.InterestAccrual(loan.LoanBilling.LoanBody, loan.InterestRate);

        billing.PeriodAccruals.InterestAccrual += interest;
        billing.PeriodAccruals.LoanBodyPayoff = billing.PeriodAccruals.OneTimePayment - billing.PeriodAccruals.InterestAccrual;
    }
}