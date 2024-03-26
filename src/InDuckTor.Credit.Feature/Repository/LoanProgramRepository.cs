using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Infrastructure.Config.Database;

namespace InDuckTor.Credit.Feature.Repository;

public class LoanProgramRepository(LoanDbContext context) : ILoanProgramRepository
{
    public async Task<LoanProgram?> GetLoanProgramById(long id)
    {
        return await context.LoanPrograms.FindAsync(id);
    }
}