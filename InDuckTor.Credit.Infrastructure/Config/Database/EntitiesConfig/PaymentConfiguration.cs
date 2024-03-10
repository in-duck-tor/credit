using InDuckTor.Credit.Domain.Billing.Payment;
using InDuckTor.Credit.Domain.LoanManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable(nameof(Payment)).HasKey(l => l.Id);
        builder.Property(p => p.Id).HasColumnName("Id");

        builder.HasOne<Loan>().WithMany().HasForeignKey(p => p.LoanId);
    }
}

public class PaymentDistributionConfiguration : IEntityTypeConfiguration<PaymentDistribution>
{
    public void Configure(EntityTypeBuilder<PaymentDistribution> builder)
    {
        builder.ToTable(nameof(Payment)).HasKey(pd => pd.Id);
        builder.Property(pd => pd.Id).HasColumnName("Id");

        builder.HasOne<Payment>()
            .WithOne(p => p.PaymentDistribution)
            .HasForeignKey<PaymentDistribution>(pd => pd.Id)
            .OnDelete(DeleteBehavior.Cascade)
            .Metadata.IsRequired = true;

        builder.HasMany(pd => pd.BillingsPayoffs).WithOne();
    }
}