using BJJManager.Domain.Common;
using BJJManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BJJManager.Infrastructure.Persistence;

public class BjjManagerDbContext : DbContext
{
    public BjjManagerDbContext(DbContextOptions<BjjManagerDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<TrainingSession> TrainingSessions => Set<TrainingSession>();
    public DbSet<TrainingMedia> TrainingMedia => Set<TrainingMedia>();
    public DbSet<Technique> Techniques => Set<Technique>();
    public DbSet<TechniqueStep> TechniqueSteps => Set<TechniqueStep>();
    public DbSet<TechniqueMedia> TechniqueMedia => Set<TechniqueMedia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BjjManagerDbContext).Assembly);

        // All entity Ids are client-generated Guids (assigned in the constructor), never
        // database-generated. Without this, EF Core's default "value generated on add"
        // convention for Guid keys makes it treat entities newly appended to an
        // already-tracked parent's collection (e.g. adding media to an existing technique)
        // as existing/Modified instead of Added, producing a no-op UPDATE instead of an INSERT.
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType).Property(nameof(Entity.Id)).ValueGeneratedNever();
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
