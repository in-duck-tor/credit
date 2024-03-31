namespace InDuckTor.Credit.Domain.Events;

public interface IEventHandler<in T> where T : IEvent
{
    Task Handle(T @event, CancellationToken cancellationToken);
}

// Просрок платежа, понижает (при начислении штрафов)
// Платёж вовремя, повышает