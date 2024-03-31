using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;

namespace InDuckTor.Credit.Domain.LoanManagement.Event;

public record PeriodPaid(long ClientId, TimeSpan PeriodDuration, TimeSpan TimeUntilPeriodEnd) : IEvent;

public class PeriodPaidEventHandler(ICreditScoreRepository creditScoreRepository) : IEventHandler<PeriodPaid>
{
    private const int ScoreChange = 5;

    public async Task Handle(PeriodPaid @event, CancellationToken cancellationToken)
    {
        var creditScore = await creditScoreRepository.GetOrCreateByClientId(@event.ClientId, cancellationToken);

        var ratio = Math.Max(1, @event.TimeUntilPeriodEnd / @event.PeriodDuration * 10);
        var increase = F(ratio);
        creditScore.Score += ScoreChange + increase;
    }

    private static int F(double x) => (int)Math.Ceiling(x);
}