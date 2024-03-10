using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.LoanManagement.Accounts;
using InDuckTor.Credit.Domain.LoanManagement.Interactor;
using InDuckTor.Credit.Feature.Repository;
using InDuckTor.Credit.WebApi.Configuration.Models;

namespace InDuckTor.Credit.WebApi.Configuration;

public static class DomainConfigurationExtensions
{
    public static IServiceCollection ConfigureDomain(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        return serviceCollection
            .ConfigureDomainServices()
            .ConfigureRepositories()
            .AddAccountsRepositoryConfig(configuration)
            .AddScoped<ILoanInteractorFactory, LoanInteractorFactory>();
    }

    private static IServiceCollection ConfigureDomainServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<ILoanService, LoanService>()
            .AddScoped<IApplicationService, ApplicationService>()
            .AddScoped<ILoanProgramService, LoanProgramService>()
            .AddScoped<PeriodService>()
            .AddScoped<IPaymentService, PaymentService>();
    }

    private static IServiceCollection ConfigureRepositories(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddScoped<ILoanRepository, LoanRepository>()
            .AddScoped<IApplicationRepository, ApplicationRepository>()
            .AddScoped<IPaymentRepository, PaymentRepository>()
            .AddScoped<ILoanProgramRepository, LoanProgramRepository>()
            .AddScoped<IPeriodBillingRepository, PeriodBillingRepository>()
            .AddScoped<IAccountsRepository, AccountsRepository>();
    }

    private static IServiceCollection AddAccountsRepositoryConfig(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var settings = configuration.GetSection("AccountsSettings").Get<AccountsSettings>();
        ArgumentNullException.ThrowIfNull(settings);

        return serviceCollection.AddSingleton(new AccountsRepositoryConfig(settings.Token));
    }
}