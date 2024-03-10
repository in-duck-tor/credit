using InDuckTor.Credit.Domain.Billing;
using InDuckTor.Credit.Domain.LoanManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable(nameof(Loan)).HasKey(l => l.Id);

        builder
            .HasOne(l => l.LoanBilling)
            .WithOne(lb => lb.Loan)
            .HasForeignKey<LoanBilling>(l => l.Id)
            .IsRequired();
    }
}