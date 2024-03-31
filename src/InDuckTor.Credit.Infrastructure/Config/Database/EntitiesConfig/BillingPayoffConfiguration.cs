using InDuckTor.Credit.Domain.Billing.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class BillingPayoffConfiguration : IEntityTypeConfiguration<BillingPayoff>
{
    public void Configure(EntityTypeBuilder<BillingPayoff> builder)
    {
        builder.ToTable(nameof(BillingPayoff)).HasKey(l => l.Id);

        builder.HasOne(bp => bp.PeriodBilling).WithMany().IsRequired(false);
        builder.OwnsOne(bp => bp.ExpenseItems);
    }
}