using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BJJManager.Infrastructure.Persistence.Configurations;

public class TrainingSessionConfiguration : IEntityTypeConfiguration<TrainingSession>
{
    public void Configure(EntityTypeBuilder<TrainingSession> builder)
    {
        builder.ToTable("TrainingSessions");
        builder.HasKey(t => t.Id);

        builder.Property(t => t.AcademyName).HasMaxLength(200).IsRequired();
        builder.Property(t => t.Notes);
        builder.Property(t => t.SelfEvaluation).HasConversion<byte>().IsRequired();
        builder.Property(t => t.CreatedAtUtc).IsRequired();

        builder.HasIndex(t => new { t.UserId, t.TrainingDate });

        builder.Metadata.FindNavigation(nameof(TrainingSession.Media))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(t => t.Media)
            .WithOne()
            .HasForeignKey(m => m.TrainingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
