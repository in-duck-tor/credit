using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;

namespace InDuckTor.Credit.Domain.LoanManagement.Event;

public record PenaltyCharged(long ClientId) : IEvent;

public class PenaltyChargedEventHandler(ICreditScoreRepository creditScoreRepository) : IEventHandler<PenaltyCharged>
{
    private const long ScoreChange = -10;

    public async Task Handle(PenaltyCharged @event, CancellationToken cancellationToken)
    {
        var creditScore = await creditScoreRepository.GetOrCreateByClientId(@event.ClientId, cancellationToken)
                          ?? new CreditScore.CreditScore(@event.ClientId);
        creditScore.Score += ScoreChange;
    }
}