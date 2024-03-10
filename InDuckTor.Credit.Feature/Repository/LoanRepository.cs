using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;

namespace InDuckTor.Credit.Feature.Repository;

public class LoanRepository(LoanDbContext context) : ILoanRepository
{
    public async Task<Loan?> GetById(long loanId)
    {
        return await context.Loans.FindAsync(loanId);
    }
}