using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;
using InDuckTor.Shared.Strategies.Interceptors;

namespace InDuckTor.Credit.Feature.Feature.CreditScore;

public record CreditScoreResponse(int CreditScore);

public interface IGetCreditScore : IQuery<long, CreditScoreResponse>;

public class GetCreditScore(LoanDbContext context) : IGetCreditScore
{
    public async Task<CreditScoreResponse> Execute(long clientId, CancellationToken ct)
    {
        var creditScore = await context.CreditScores.FindAsync([clientId], ct)
                          ?? throw new Errors.CreditScore.NotFound(clientId);
        return new CreditScoreResponse(creditScore.Score);
    }
}