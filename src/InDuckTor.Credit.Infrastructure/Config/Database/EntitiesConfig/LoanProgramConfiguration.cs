using InDuckTor.Credit.Domain.Financing.Program;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class LoanProgramConfiguration : IEntityTypeConfiguration<LoanProgram>
{
    public void Configure(EntityTypeBuilder<LoanProgram> builder)
    {
        builder.ToTable(nameof(LoanProgram)).HasKey(lp => lp.Id);
    }
}