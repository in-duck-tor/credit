using InDuckTor.Credit.Domain.Billing.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(nameof(Payment)).HasKey(l => l.Id);

        builder.ComplexProperty(p => p.PaymentDistribution, b => b.IsRequired());
    }
}