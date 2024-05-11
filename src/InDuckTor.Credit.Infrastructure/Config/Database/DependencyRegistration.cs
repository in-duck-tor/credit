using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public record DatabaseSettings(string Scheme);

public static class DependencyRegistration
{
    public static IServiceCollection AddLoanDbContext(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(nameof(DatabaseSettings));
        serviceCollection.Configure<DatabaseSettings>(configurationSection);
        var databaseSettings = configurationSection.Get<DatabaseSettings>();
        ArgumentNullException.ThrowIfNull(databaseSettings, nameof(configuration));

        var npgsqlDataSource = new NpgsqlDataSourceBuilder(configuration.GetConnectionString("LoanDatabase"))
            .EnableDynamicJson()
            .Build();

        return serviceCollection.AddDbContext<LoanDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseNpgsql(npgsqlDataSource);
        });
    }
}