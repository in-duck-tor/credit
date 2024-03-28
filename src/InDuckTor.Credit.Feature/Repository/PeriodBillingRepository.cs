using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class PeriodBillingRepository(LoanDbContext context) : IPeriodBillingRepository
{
    public async Task<List<PeriodBilling>> GetAllUnpaidPeriodBillings(long loanId)
    {
        return await context.PeriodsBillings
            .Include(pb => pb.Loan)
            .Where(pb => pb.Loan.Id == loanId && pb.RemainingPayoff != null)
            .ToListAsync();
    }
}