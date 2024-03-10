using System.Reflection;
using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Billing.Period;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;
using InDuckTor.Credit.Infrastructure.Config.Database.Converters;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public class LoanDbContext : DbContext
{
    public string? Schema { get; }

    public LoanDbContext(DbContextOptions<LoanDbContext> dbContextOptions) : base(dbContextOptions)
    {
        Schema = "credit";
    }

    public virtual DbSet<Loan> Loans { get; set; } = null!;
    public virtual DbSet<LoanBilling> LoanBillings { get; set; } = null!;
    public virtual DbSet<PeriodBilling> PeriodsBillings { get; set; } = null!;
    public virtual DbSet<Payment> Payments { get; set; } = null!;
    public virtual DbSet<LoanApplication> LoanApplications { get; set; } = null!;
    public virtual DbSet<LoanProgram> LoanPrograms { get; set; } = null!;

    public const string LoanPersonalCodeSequenceName = "loan_personal_code_seq";

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
        configurationBuilder.Properties<BillingItem>().HaveConversion<BillingItemConverter>();
    }
}