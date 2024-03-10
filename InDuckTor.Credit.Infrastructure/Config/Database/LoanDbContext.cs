using System.Reflection;
using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.Financing.Application;
using InDuckTor.Credit.Domain.Financing.Program;
using InDuckTor.Credit.Domain.LoanManagement;
using Microsoft.EntityFrameworkCore;

namespace InDuckTor.Credit.Infrastructure.Config.Database;

public class LoanDbContext : DbContext
{
    public string? Schema { get; }

    public LoanDbContext(DbContextOptions<LoanDbContext> dbContextOptions) : base(dbContextOptions)
    {
        Schema = dbContextOptions.Extensions.OfType<LoanDbContextOptionsExtension>()
            .FirstOrDefault()
            ?.Schema;
    }

    public virtual DbSet<Loan> Accounts { get; set; } = null!;
    public virtual DbSet<LoanApplication> Banks { get; set; } = null!;
    public virtual DbSet<LoanProgram> Currencies { get; set; } = null!;
    public virtual DbSet<Payment> FundsReservations { get; set; } = null!;
    public virtual DbSet<LoanBilling> Transactions { get; set; } = null!;

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
        configurationBuilder.Properties<DateTime>().HaveColumnType("timestamp without time zone");
        // configurationBuilder.Properties<AccountNumber>().HaveConversion<AccountNumberConverter>();
    }
}