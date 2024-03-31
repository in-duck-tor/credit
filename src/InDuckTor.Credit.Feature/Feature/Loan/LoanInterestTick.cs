using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.LoanManagement.State;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Loan;

public interface ILoanInterestTick : ICommand<Unit, Unit>;

public class LoanInterestTick(LoanDbContext context, ILoanService loanService) : ILoanInterestTick
{
    public async Task<Unit> Execute(Unit input, CancellationToken ct)
    {
        var loans = await context.Loans
            // .Include(l => l.PeriodsBillings)
            .Where(loan => loan.State == LoanState.Active)
            .ToListAsync(cancellationToken: ct);

        foreach (var loan in loans)
        {
            await loanService.Tick(loan, ct);
            await context.SaveChangesAsync(ct);
        }

        return default;
    }
}