namespace InDuckTor.Credit.Domain.Loan.PaymentSystem;

class AnnuityPaymentSystem : IPaymentSystem
{
    private readonly InterestCalculator _interestCalculator = new();

    public void AccrueInterest(Loan loan)
    {
        ArgumentNullException.ThrowIfNull(loan.PeriodAccruals);

        var interest = _interestCalculator.InterestAccrual(loan.LoanBody, loan.InterestRate);

        loan.PeriodAccruals.InterestAccrual += interest;
        loan.PeriodAccruals.LoanBodyPayoff = loan.PeriodAccruals.OneTimePayment - loan.PeriodAccruals.InterestAccrual;
    }
}
