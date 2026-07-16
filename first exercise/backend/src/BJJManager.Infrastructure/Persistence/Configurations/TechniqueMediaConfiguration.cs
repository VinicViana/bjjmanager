using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BJJManager.Infrastructure.Persistence.Configurations;

public class TechniqueMediaConfiguration : IEntityTypeConfiguration<TechniqueMedia>
{
    public void Configure(EntityTypeBuilder<TechniqueMedia> builder)
    {
        builder.ToTable("TechniqueMedia");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FileName).HasMaxLength(255).IsRequired();
        builder.Property(m => m.FileUrl).IsRequired();
        builder.Property(m => m.MediaType).HasConversion<byte>().IsRequired();
    }
}
