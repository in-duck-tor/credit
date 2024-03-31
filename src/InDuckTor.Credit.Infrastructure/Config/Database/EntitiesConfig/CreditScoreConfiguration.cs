using InDuckTor.Credit.Domain.LoanManagement.CreditScore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class CreditScoreConfiguration : IEntityTypeConfiguration<CreditScore>
{
    public void Configure(EntityTypeBuilder<CreditScore> builder)
    {
        builder.ToTable(nameof(CreditScore)).HasKey(cs => cs.ClientId);

        builder.Property(cs => cs.ClientId).ValueGeneratedNever();
    }
}