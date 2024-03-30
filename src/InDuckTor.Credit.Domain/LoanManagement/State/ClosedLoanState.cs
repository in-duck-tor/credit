using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;

namespace InDuckTor.Credit.Domain.LoanManagement.State;

public class ClosedLoanState : ILoanState
{
    public ClosedLoanState(Loan loan)
    {
        Loan = loan;
    }

    private const LoanState ThisState = LoanState.Closed;
    private Loan Loan { get; }

    public void ChangeState(LoanState state)
    {
        switch (state)
        {
            case LoanState.Closed:
                break;
            case LoanState.Active:
            case LoanState.Sold:
            case LoanState.Approved:
                throw new Errors.Loan.InvalidLoanStateChange(
                    $"Cannot change state from {ThisState}");
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    public void StartNewPeriod()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(StartNewPeriod), ThisState);
    }

    public PeriodBilling ClosePeriod()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(ClosePeriod), ThisState);
    }

    public decimal GetCurrentTotalPayment()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(GetCurrentTotalPayment), ThisState);
    }

    public decimal GetExpectedOneTimePayment()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(GetExpectedOneTimePayment), ThisState);
    }

    public void AccrueInterestOnCurrentPeriod()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(AccrueInterestOnCurrentPeriod), ThisState);
    }

    public decimal CalculateTickInterest()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(CalculateTickInterest), ThisState);
    }

    public void ChargePenalty()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(ChargePenalty), ThisState);
    }

    public void AttachLoanAccount(string accountNumber)
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(AttachLoanAccount), ThisState);
    }

    public bool IsCurrentPeriodEnded()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(IsCurrentPeriodEnded), ThisState);
    }

    public void ActivateLoan()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(ActivateLoan), ThisState);
    }

    public void CloseLoan()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(CloseLoan), ThisState);
    }

    public void SellToCollectors()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(SellToCollectors), ThisState);
    }
}