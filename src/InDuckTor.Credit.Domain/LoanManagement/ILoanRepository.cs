namespace InDuckTor.Credit.Domain.LoanManagement;

public interface ILoanRepository
{
    Loan GetById(long loanId);
}