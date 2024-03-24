namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanRepository
{
    Task<bool> IsExists(long loanId, long clientId, CancellationToken cancellationToken);
}