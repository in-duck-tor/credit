namespace InDuckTor.Credit.Domain.LoanManagement.State;

public static class LoanStateFactory
{
    public static ILoanState CreateState(Loan loan)
    {
        return loan.State switch
        {
            LoanState.Approved => new ApprovedLoanState(loan),
            LoanState.Active => new ActiveLoanState(loan),
            LoanState.Closed => new ClosedLoanState(loan),
            LoanState.Sold => new SoldLoanState(loan),
            _ => throw new ArgumentOutOfRangeException(nameof(loan))
        };
    }
}