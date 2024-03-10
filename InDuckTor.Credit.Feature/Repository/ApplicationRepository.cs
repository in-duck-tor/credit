using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Infrastructure.Config.Database;

namespace InDuckTor.Credit.Feature.Repository;

public class ApplicationRepository(LoanDbContext context) : IApplicationRepository
{
    public async Task<LoanApplication?> GetApplicationById(long id)
    {
        return await context.LoanApplications.FindAsync(id);
    }
}