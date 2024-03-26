using InDuckTor.Credit.Feature.Feature.Program.Model;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Feature.Program;

public interface IGetAllLoanPrograms : IQuery<Unit, List<LoanProgramResponse>>;

public class GetAllLoanPrograms(LoanDbContext context) : IGetAllLoanPrograms
{
    public async Task<List<LoanProgramResponse>> Execute(Unit input, CancellationToken ct)
    {
        var loanPrograms = await context.LoanPrograms.ToListAsync(cancellationToken: ct);
        return loanPrograms.Select(LoanProgramResponse.FromLoanProgram).ToList();
    }
}