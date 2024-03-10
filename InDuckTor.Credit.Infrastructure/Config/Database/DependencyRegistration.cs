using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public record DatabaseSettings(string Scheme);

public static class DependencyRegistration
{
    public static IServiceCollection AddLoanDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(nameof(DatabaseSettings));
        serviceCollection.Configure<DatabaseSettings>(configurationSection);
        var databaseSettings = configurationSection.Get<DatabaseSettings>();
        ArgumentNullException.ThrowIfNull(databaseSettings, nameof(configuration));

        return serviceCollection.AddDbContext<LoanDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("LoanDatabase"));
        });
    }
}
