using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;

namespace InDuckTor.Credit.Domain.LoanManagement.Event;

public record PenaltyCharged(long ClientId, decimal BorrowedAmount, decimal Debt) : IEvent;

public class PenaltyChargedEventHandler(ICreditScoreRepository creditScoreRepository) : IEventHandler<PenaltyCharged>
{
    private const int ScoreChange = -10;

    public async Task Handle(PenaltyCharged @event, CancellationToken cancellationToken)
    {
        var creditScore = await creditScoreRepository.GetOrCreateByClientId(@event.ClientId, cancellationToken);

        var ratio = (double)(@event.Debt / @event.BorrowedAmount);
        var increase = F(ratio);
        creditScore.Score += ScoreChange + increase;
    }

    private static int F(double x) => (int)Math.Ceiling(Math.Pow(x, 0.25) * (-x + 1));
}