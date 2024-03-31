using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;

namespace InDuckTor.Credit.Domain.LoanManagement.Event;

public record PeriodNotPaid(long ClientId, decimal ExpectedPayment, decimal RemainingPayment) : IEvent;

public class PeriodNotPaidEventHandler(ICreditScoreRepository creditScoreRepository) : IEventHandler<PeriodNotPaid>
{
    private const int ScoreChange = -7;

    public async Task Handle(PeriodNotPaid @event, CancellationToken cancellationToken)
    {
        var creditScore = await creditScoreRepository.GetOrCreateByClientId(@event.ClientId, cancellationToken)
                          ?? new CreditScore.CreditScore(@event.ClientId);

        var ratio = (double)(@event.RemainingPayment / @event.ExpectedPayment);
        var increase = F(ratio);
        creditScore.Score += ScoreChange + increase;
    }

    private static int F(double x) => (int)Math.Ceiling(Math.Pow(x, 0.25) * (-x + 1));
}