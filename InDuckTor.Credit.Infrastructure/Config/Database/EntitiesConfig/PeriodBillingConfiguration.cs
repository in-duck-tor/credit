using InDuckTor.Credit.Domain.Billing.Period;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class PeriodBillingConfiguration : IEntityTypeConfiguration<PeriodBilling>
{
    public void Configure(EntityTypeBuilder<PeriodBilling> builder)
    {
        builder.ToTable(nameof(PeriodBilling)).HasKey(l => l.Id);

        builder.ComplexProperty(pb => pb.BillingItems, b => b.IsRequired());
        builder.OwnsOne(pb => pb.RemainingPayoff);
    }
}