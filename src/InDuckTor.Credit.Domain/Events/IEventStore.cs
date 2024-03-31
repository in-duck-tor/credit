namespace InDuckTor.Credit.Domain.Events;

public interface IEventStore
{
    IEnumerable<IEvent> GetEvents();
    void StoreEvent(IEvent @event);
    void ClearEvents();
}