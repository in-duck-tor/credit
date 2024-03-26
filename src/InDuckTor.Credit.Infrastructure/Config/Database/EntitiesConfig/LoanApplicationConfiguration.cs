using InDuckTor.Credit.Domain.Financing.Application;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder.ToTable(nameof(LoanApplication)).HasKey(l => l.Id);

        builder
            .HasOne(la => la.LoanProgram)
            .WithMany();
        // .HasForeignKey(la => la.LoanProgramId);
    }
}