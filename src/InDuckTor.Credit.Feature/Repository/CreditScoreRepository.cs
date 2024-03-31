using InDuckTor.Credit.Domain.LoanManagement.CreditScore;
using InDuckTor.Credit.Infrastructure.Config.Database;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Feature.Repository;

public class CreditScoreRepository : ICreditScoreRepository
{
    private readonly LoanDbContext _context;

    public CreditScoreRepository(LoanDbContext context)
    {
        _context = context;
    }

    public async Task<CreditScore> GetOrCreateByClientId(long id, CancellationToken cancellationToken)
    {
        var creditScore = await _context.CreditScores.FirstOrDefaultAsync(cs => cs.ClientId == id, cancellationToken);

        if (creditScore != null) return creditScore;

        creditScore = new CreditScore(id);
        _context.Add(creditScore);

        return creditScore;
    }
}