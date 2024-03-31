using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class LoanRepository(LoanDbContext context) : ILoanRepository
{
    public async Task<Loan?> GetById(long loanId, CancellationToken cancellationToken)
    {
        return await context.Loans.FindAsync([loanId], cancellationToken: cancellationToken);
    }

    public async Task<int> GetNumberOfPeriods(long loanId, CancellationToken cancellationToken)
    {
        return await context.PeriodsBillings.CountAsync(
            pb => pb.Loan.Id == loanId,
            cancellationToken);
    }

    public async Task<bool> IsExists(long loanId, long clientId, CancellationToken cancellationToken)
    {
        return await context.Loans.AnyAsync(
            loan => loan.Id == loanId && loan.ClientId == clientId,
            cancellationToken
        );
    }
}