using Microsoft.Extensions.DependencyInjection;

namespace InDuckTor.Credit.Domain.Events;

public class EventDispatcher : IEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public EventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task Dispatch(IEvent @event, CancellationToken cancellationToken)
    {
        var handlerType = typeof(IEventHandler<>).MakeGenericType(@event.GetType());

        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            if (handler == null) continue;
            var task = handlerType.GetMethod(nameof(IEventHandler<IEvent>.Handle))
                ?.Invoke(handler, [@event, cancellationToken]) as Task;
            ArgumentNullException.ThrowIfNull(task);
            await task;
        }
    }
}