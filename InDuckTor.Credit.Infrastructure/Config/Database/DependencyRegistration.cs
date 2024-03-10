using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public record DatabaseSettings(string Scheme);

public static class DependencyRegistration
{
    public static IServiceCollection AddAccountsDbContext(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        var configurationSection = configuration.GetSection(nameof(DatabaseSettings));
        serviceCollection.Configure<DatabaseSettings>(configurationSection);
        var databaseSettings = configurationSection.Get<DatabaseSettings>();
        ArgumentNullException.ThrowIfNull(databaseSettings, nameof(configuration));

        return serviceCollection.AddDbContext<LoanDbContext>(optionsBuilder =>
        {
            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(new LoanDbContextOptionsExtension(databaseSettings.Scheme));

            optionsBuilder.UseNpgsql(configuration.GetConnectionString("LoanDatabase"));
        });
    }
}

internal class LoanDbContextOptionsExtension(string? schema = null) : IDbContextOptionsExtension
{
    public string? Schema { get; private set; } = schema;
    private DbContextOptionsExtensionInfo? _info;

    public void ApplyServices(IServiceCollection services)
    {
    }

    public void Validate(IDbContextOptions options)
    {
    }

    public DbContextOptionsExtensionInfo Info => _info ??= new LoanDbContextOptionsExtensionInfo(this);
}

internal class LoanDbContextOptionsExtensionInfo(IDbContextOptionsExtension extension)
    : DbContextOptionsExtensionInfo(extension)
{
    public override int GetServiceProviderHashCode() => 0;

    public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other) => other is LoanDbContextOptionsExtensionInfo;

    public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
    }

    public override bool IsDatabaseProvider => false;
    public override string LogFragment => string.Empty;
}