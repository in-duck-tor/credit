using Hangfire;
using Hangfire.PostgreSql;
using InDuckTor.Credit.Feature.Feature.Loan;
using InDuckTor.Shared.Models;
using InDuckTor.Shared.Strategies;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class BackgroundJobsConfiguration
{
    public static void UseHangfire(this WebApplication app)
    {
        GlobalConfiguration.Configuration.UseActivator(new HangfireActivator(app.Services));

        app.UseHangfireDashboard();
        app.MapHangfireDashboard();

        AddLoanTickJob();
    }

    private static void AddLoanTickJob()
    {
        RecurringJob.AddOrUpdate(
            "loanTick",
            (IExecutor<ILoanInterestTick, Unit, Unit> loanInterestTick) => loanInterestTick.Execute(default, default),
            Cron.Minutely);
    }

    public static IServiceCollection ConfigureHangfire(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection.AddHangfire(cfg => cfg
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(configure =>
            {
                configure.UseNpgsqlConnection(configuration.GetConnectionString("LoanDatabase"));
            })
        );
        // Add the processing server as IHostedService
        serviceCollection.AddHangfireServer();

        return serviceCollection;
    }
}

public class HangfireActivator : JobActivator
{
    private readonly IServiceProvider _serviceProvider;

    public HangfireActivator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override object? ActivateJob(Type type)
    {
        return _serviceProvider.GetService(type);
    }
}