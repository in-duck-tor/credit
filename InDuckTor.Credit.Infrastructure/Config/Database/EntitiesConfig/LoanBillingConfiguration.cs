using InDuckTor.Credit.Domain.Billing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class LoanBillingConfiguration : IEntityTypeConfiguration<LoanBilling>
{
    public void Configure(EntityTypeBuilder<LoanBilling> builder)
    {
        builder.ToTable(nameof(LoanBilling)).HasKey(l => l.Id);

        builder.ComplexProperty(lb => lb.LoanBody, bi => bi.IsRequired());
        builder.ComplexProperty(lb => lb.LoanDebt, bi => bi.IsRequired());
        builder.ComplexProperty(lb => lb.Penalty, bi => bi.IsRequired());

        builder.OwnsOne(lb => lb.PeriodAccruals);

        builder
            .HasMany(lb => lb.PeriodsBillings)
            .WithOne(pb => pb.LoanBilling)
            .IsRequired();
    }
}