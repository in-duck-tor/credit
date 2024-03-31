using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.LoanManagement.Event;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class DomainEventConfiguration
{
    public static IServiceCollection ConfigureDomainEvents(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<IEventHandler<PenaltyCharged>, PenaltyChargedEventHandler>()
            .AddScoped<IEventDispatcher, EventDispatcher>();
    }
    
//     var concreteTypes = assemblies.SelectMany(assembly => assembly.ExportedTypes).Where(type => type is { IsClass: true, IsAbstract: false });
//
//         foreach (var type in concreteTypes)
//     {
//         if (!type.TryGetGenericInterfaceDefinition(typeof(IStrategy<,>), out var strategyInterface)) continue;
//
//         services.RegisterStrategy(type, strategyInterface)
//             .RegisterStrategyExecutor(type, strategyInterface, GetInterceptorTypesFor(type, strategyInterface));
//     }
//
//     return services;
}