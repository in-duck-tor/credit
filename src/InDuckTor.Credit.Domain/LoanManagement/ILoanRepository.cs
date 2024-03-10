namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanRepository
{
    Task<Loan?> GetById(long loanId);
}