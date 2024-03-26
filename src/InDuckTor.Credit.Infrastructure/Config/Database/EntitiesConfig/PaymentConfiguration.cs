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
        builder.HasMany(pd => pd.BillingsPayoffs).WithOne();
    }
}