using InDuckTor.Credit.Feature.Repository;
using InDuckTor.Credit.WebApi.Configuration.Models;
using Refit;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class RefitConfigurationExtensions
{
    public static IServiceCollection ConfigureRefit(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("AccountsSettings").Get<AccountsSettings>();
        ArgumentNullException.ThrowIfNull(settings);

        serviceCollection
            .AddRefitClient<IAccountsRepositoryRefit>()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = new Uri(settings.BaseAddress));

        return serviceCollection;
    }
}