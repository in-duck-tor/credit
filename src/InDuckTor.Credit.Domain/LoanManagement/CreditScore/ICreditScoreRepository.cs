namespace InDuckTor.Credit.Domain.LoanManagement.CreditScore;

public interface ICreditScoreRepository
{
    Task<CreditScore> GetOrCreateByClientId(long id, CancellationToken cancellationToken);
}