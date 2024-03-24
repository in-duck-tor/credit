using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class LoanRepository(LoanDbContext context) : ILoanRepository
{
    public async Task<bool> IsExists(long loanId, long clientId, CancellationToken cancellationToken)
    {
        return await context.Loans.AnyAsync(
            loan => loan.Id == loanId && loan.ClientId == clientId,
            cancellationToken
        );
    }
}