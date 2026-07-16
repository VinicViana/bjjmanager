using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BJJManager.Infrastructure.Persistence.Configurations;

public class TechniqueConfiguration : IEntityTypeConfiguration<Technique>
{
    public void Configure(EntityTypeBuilder<Technique> builder)
    {
        builder.ToTable("Techniques");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Position).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).IsRequired();

        builder.HasIndex(t => t.UserId);

        builder.Metadata.FindNavigation(nameof(Technique.Steps))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
        builder.Metadata.FindNavigation(nameof(Technique.Media))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.Steps)
            .WithOne()
            .HasForeignKey(s => s.TechniqueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Media)
            .WithOne()
            .HasForeignKey(m => m.TechniqueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
