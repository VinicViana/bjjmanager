using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BJJManager.Infrastructure.Persistence.Configurations;

public class TechniqueStepConfiguration : IEntityTypeConfiguration<TechniqueStep>
{
    public void Configure(EntityTypeBuilder<TechniqueStep> builder)
    {
        builder.ToTable("TechniqueSteps");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Description).IsRequired();
        builder.Property(s => s.Order).IsRequired();

        builder.HasIndex(s => new { s.TechniqueId, s.Order }).IsUnique();
    }
}
