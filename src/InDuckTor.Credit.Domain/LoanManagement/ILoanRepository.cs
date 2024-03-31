namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanRepository
{
    Task<Loan?> GetById(long loanId, CancellationToken cancellationToken);
    Task<int> GetNumberOfPeriods(long loanId, CancellationToken cancellationToken);
    Task<bool> IsExists(long loanId, long clientId, CancellationToken cancellationToken);
}