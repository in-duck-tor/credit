using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement.PaymentCalculator;

namespace InDuckTor.Credit.Domain.LoanManagement.Interactor;

public class LoanInteractor(Loan loan, IPaymentCalculator paymentCalculator)
{
    public Loan Loan { get; } = loan;
    private IPaymentCalculator PaymentCalculator { get; } = paymentCalculator;

    public bool IsRepaid => Loan.LoanBilling.IsRepaid;

    public void AccrueInterestOnCurrentPeriod()
    {
        PaymentCalculator.AccrueInterestOnCurrentPeriod(Loan);
    }

    public void ChargePenalty()
    {
        Loan.LoanBilling.ChargePenalty();
    }

    public void CloseLoan()
    {
        if (!IsRepaid)
        {
            throw new Errors.Loan.InvalidLoanStateChange("Can't close the loan because it hasn't been repaid yet");
        }

        Loan.State = LoanState.Closed;
    }
}