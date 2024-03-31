using System.ComponentModel.DataAnnotations;
using InDuckTor.Credit.Domain.Exceptions;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;
using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.CreditScore;

public record CreditScoreResponse([property: Required] int CreditScore);

public interface IGetCreditScore : IQuery<long, CreditScoreResponse>;

public class GetCreditScore(LoanDbContext context, ICreditScoreRepository creditScoreRepository) : IGetCreditScore
{
    public async Task<CreditScoreResponse> Execute(long clientId, CancellationToken ct)
    {
        var creditScore = await creditScoreRepository.GetOrCreateByClientId(clientId, ct)
                          ?? throw new Errors.CreditScore.NotFound(clientId);
        return new CreditScoreResponse(creditScore.Score);
    }
}