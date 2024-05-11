using InDuckTor.Shared.Idempotency.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InDuckTor.Credit.Infrastructure.Config.Database.EntitiesConfig;

public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
{
    public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
    {
        builder.ToTable(nameof(IdempotencyRecord));
        builder.HasKey(x => x.Key);

        builder.OwnsOne(x => x.CachedResponse, ownsBuilder =>
        {
            ownsBuilder.Property(x => x.Headers)
                .HasColumnType("jsonb");
        });
    }
}