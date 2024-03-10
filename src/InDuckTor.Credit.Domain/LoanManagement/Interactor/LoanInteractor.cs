using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement.Interactor;

public class LoanInteractor(Loan loan, IPaymentCalculator paymentCalculator)
{
    public Loan Loan { get; set; } = loan;
    private IPaymentCalculator PaymentCalculator { get; init; } = paymentCalculator;

    public void AccrueInterestOnCurrentPeriod()
    {
        PaymentCalculator.AccrueInterestOnCurrentPeriod(Loan);
    }

    public void ChargePenalty()
    {
        Loan.Penalty += Loan.LoanDebt * Loan.PenaltyRate;
    }
}