using Hangfire;
using Hangfire.PostgreSql;
using InDuckTor.Credit.Feature.Feature.Loan;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class BackgroundJobsConfiguration
{
    public static void RunBackgroundJobs(this WebApplication app)
    {
        var loanInterestTick = app.Services.GetRequiredService<ILoanInterestTick>();
        AddLoanTickJob(loanInterestTick);
    }

    private static void PopolnitSchetKlienta()
    {
        
    }
    
    private static void AddLoanTickJob(ILoanInterestTick loanInterestTick)
    {
        RecurringJob.AddOrUpdate(
            "loanTick",
            () => loanInterestTick.Execute(default, default),
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