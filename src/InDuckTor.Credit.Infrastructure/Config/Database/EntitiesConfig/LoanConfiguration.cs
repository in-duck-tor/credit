using InDuckTor.Credit.Domain.LoanManagement;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable(nameof(Loan)).HasKey(l => l.Id);

        builder.ComplexProperty(l => l.CurrentBody, bi => bi.IsRequired());
        builder.ComplexProperty(l => l.BodyAfterPayoffs, bi => bi.IsRequired());
        builder.ComplexProperty(l => l.Debt, bi => bi.IsRequired());
        builder.ComplexProperty(l => l.Penalty, bi => bi.IsRequired());

        builder.OwnsOne(lb => lb.PeriodAccruals);
    }
}