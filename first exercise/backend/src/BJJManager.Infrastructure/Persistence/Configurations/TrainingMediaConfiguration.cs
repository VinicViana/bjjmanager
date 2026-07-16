using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BJJManager.Infrastructure.Persistence.Configurations;

public class TrainingMediaConfiguration : IEntityTypeConfiguration<TrainingMedia>
{
    public void Configure(EntityTypeBuilder<TrainingMedia> builder)
    {
        builder.ToTable("TrainingMedia");
        builder.HasKey(m => m.Id);

        builder.Property(m => m.FileName).HasMaxLength(255).IsRequired();
        builder.Property(m => m.FileUrl).IsRequired();
        builder.Property(m => m.MediaType).HasConversion<byte>().IsRequired();
    }
}
