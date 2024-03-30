using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Exceptions;

namespace InDuckTor.Credit.Domain.LoanManagement.State;

public class ApprovedLoanState : ILoanState
{
    public ApprovedLoanState(Loan loan)
    {
        if (loan.State != ThisState) throw new Errors.Loan.InvalidLoanState(loan.Id, nameof(ApprovedLoanState), ThisState);
        Loan = loan;
    }

    private const LoanState ThisState = LoanState.Approved;
    private Loan Loan { get; }

    public void ChangeState(LoanState state)
    {
        switch (state)
        {
            case LoanState.Active:
                Loan.SetState(state);
                break;
            case LoanState.Approved:
                break;
            case LoanState.Closed:
            case LoanState.Sold:
                throw new Errors.Loan.InvalidLoanStateChange(
                    $"Cannot change state from {ThisState} to anything but {LoanState.Active}");
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
        Loan.LoanAccountNumber = accountNumber;
    }

    public bool IsCurrentPeriodEnded()
    {
        throw new Errors.Loan.InvalidLoanState(Loan.Id, nameof(IsCurrentPeriodEnded), ThisState);
    }

    public void ActivateLoan()
    {
        ChangeState(LoanState.Active);
        Loan.StartNewPeriod();
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