using System.Reflection;
using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Events;
using InDuckTor.Credit.Domain.Expenses;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Domain.LoanManagement.CreditScore;
using InDuckTor.Credit.Infrastructure.Config.Database.Converters;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public class LoanDbContext : DbContext
{
    public string? Schema { get; }
    private readonly IEventDispatcher _eventDispatcher;

    public LoanDbContext(DbContextOptions<LoanDbContext> dbContextOptions, IEventDispatcher eventDispatcher) : base(
        dbContextOptions)
    {
        Schema = "credit";
        _eventDispatcher = eventDispatcher;
    }

    public virtual DbSet<Loan> Loans { get; set; } = null!;
    public virtual DbSet<PeriodBilling> PeriodsBillings { get; set; } = null!;
    public virtual DbSet<Payment> Payments { get; set; } = null!;
    public virtual DbSet<LoanApplication> LoanApplications { get; set; } = null!;
    public virtual DbSet<LoanProgram> LoanPrograms { get; set; } = null!;
    public virtual DbSet<CreditScore> CreditScores { get; set; } = null!;

    public const string LoanPersonalCodeSequenceName = "loan_personal_code_seq";

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEvents(cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.HasSequence<long>(LoanPersonalCodeSequenceName);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.Properties<ExpenseItem>().HaveConversion<BillingItemConverter>();
    }

    private async Task PublishDomainEvents(CancellationToken cancellationToken)
    {
        var eventStores = ChangeTracker
            .Entries<IEventStore>()
            .Select(e => e.Entity)
            .ToList();
        var domainEvents = eventStores.SelectMany(e => e.GetEvents());

        foreach (var @event in domainEvents)
        {
            await _eventDispatcher.Dispatch(@event, cancellationToken);
        }

        foreach (var eventStore in eventStores) eventStore.ClearEvents();
    }
}