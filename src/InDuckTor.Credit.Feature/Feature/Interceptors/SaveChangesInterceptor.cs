using InDuckTor.Credit.Infrastructure.Config.Database;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.Feature.Feature.Interceptors;

public class SaveChangesInterceptor<T1, T2>(LoanDbContext context) : IStrategyInterceptor<T1, T2>
{
    public async Task<T2> Intercept(T1 input, IStrategy<T1, T2>.Delegate next, CancellationToken ct)
    {
        var result = await next(input, ct);
        await context.SaveChangesAsync(ct);
        return result;
    }
}