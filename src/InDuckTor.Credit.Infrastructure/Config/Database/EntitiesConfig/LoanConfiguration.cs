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
        
        builder.ComplexProperty(lb => lb.CurrentBody, bi => bi.IsRequired());
        builder.ComplexProperty(lb => lb.Debt, bi => bi.IsRequired());
        builder.ComplexProperty(lb => lb.Penalty, bi => bi.IsRequired());

        builder.OwnsOne(lb => lb.PeriodAccruals);
    }
}