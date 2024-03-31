namespace InDuckTor.Credit.Domain.LoanManagement.CreditScore;

// Должен сложно создаватаься, исходя из кучи данных и вообще не должен находиться в этом сервисе, но так проще...
public class CreditScore
{
    private const int DefaultScore = 900;

    private CreditScore()
    {
        // EF Core thing...
    }

    public CreditScore(long clientId)
    {
        ClientId = clientId;
        Score = DefaultScore;
    }

    public CreditScore(long clientId, int score)
    {
        ClientId = clientId;
        Score = score;
    }

    public long ClientId { get; init; }

    public int Score { get; set; }
}