namespace InDuckTor.Credit.Domain.Events;

public interface IEventDispatcher
{
    Task Dispatch(IEvent @event, CancellationToken cancellationToken);
}