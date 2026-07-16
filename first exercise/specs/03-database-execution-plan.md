# Database Execution Plan — BJJManager (SQL Server)

Implements the database slice of `00-master-spec.md` §3. EF Core Code First migrations in
`backend/src/BJJManager.Infrastructure/Persistence/Migrations` are the only source of truth for
the schema — there are no hand-authored or generated SQL scripts to keep in sync with them.

## 1. Repo layout

```
/database
  README.md    # how the schema gets created/seeded and the demo credentials
```

No `/scripts` folder: a parallel set of SQL files was considered but dropped in favor of relying
on `dotnet ef database update`/`Database.MigrateAsync()` directly — one less artifact that can
silently drift from the actual EF model.

## 2. Schema (mirrors master spec §3 exactly)

| Table | PK | FKs | Notable columns |
|---|---|---|---|
| Users | Id (uniqueidentifier) | — | Name nvarchar(100), PasswordHash nvarchar(max), CreatedAt datetime2 |
| TrainingSessions | Id | UserId → Users | TrainingDate date, AcademyName nvarchar(200), Notes nvarchar(max) null, SelfEvaluation tinyint, CreatedAt datetime2 |
| TrainingMedia | Id | TrainingId → TrainingSessions (cascade delete) | FileName nvarchar(255), FileUrl nvarchar(max), MediaType tinyint |
| Techniques | Id | UserId → Users | Name nvarchar(200), Position nvarchar(100), Description nvarchar(max) |
| TechniqueSteps | Id | TechniqueId → Techniques (cascade delete) | [Order] int, Description nvarchar(max) |
| TechniqueMedia | Id | TechniqueId → Techniques (cascade delete) | FileName nvarchar(255), FileUrl nvarchar(max), MediaType tinyint |

Indexes:
- `IX_TrainingSessions_UserId_TrainingDate` on `(UserId, TrainingDate)` — supports the exact
  date/month/year filter queries.
- `IX_Techniques_UserId` on `(UserId)`.
- Unique constraint `UQ_TechniqueSteps_TechniqueId_Order` on `(TechniqueId, [Order])`.

`MediaType` is a tinyint mirroring the `Image = 0, Video = 1` enum — no separate lookup table
(kept simple; it's a fixed two-value domain concept, not reference data that changes).

IDs are generated application-side (`Guid.NewGuid()` in the Domain entity constructors per the
backend plan), and every entity is explicitly configured with `ValueGeneratedNever()` on `Id` in
`BjjManagerDbContext.OnModelCreating` — otherwise EF Core's default convention for Guid keys
treats a non-default key value on an entity newly attached to an already-tracked parent (e.g.
adding a media item to an existing technique) as an *existing* row and emits a no-op `UPDATE`
instead of an `INSERT`.

## 3. Workflow

1. Backend plan builds the EF Core model + `IEntityTypeConfiguration<T>` classes and the
   migrations (`dotnet ef migrations add <Name>`) whenever the model changes.
2. `Program.cs` calls `Database.MigrateAsync()` on startup (Development environment, real SQL
   Server provider only — integration tests swap in SQLite and create the schema via
   `EnsureCreated()` instead, since SQL Server migrations aren't valid SQLite DDL), so a fresh
   clone just needs an empty database created before the first run.
3. `DbSeeder.SeedAsync` (`BJJManager.Infrastructure/Persistence/DbSeeder.cs`), also called from
   `Program.cs` on startup, seeds one demo user plus a handful of sample `TrainingSessions` and
   `Techniques` (with steps) if no user named `demo` exists yet. It builds entities through their
   normal constructors and hashes the demo password through the real `IPasswordHasher` resolved
   from DI, so there's no manual hash-and-paste step to keep in sync with the hashing scheme.
4. `database/README.md` documents: create an empty `BJJManager` database on the
   `localhost\SQLEXPRESS02` instance (matches the provided connection string), then just run the
   API — migration and seeding happen automatically.

## 4. Definition of done for this slice

- A fresh `BJJManager` database plus `dotnet run` (no manual SQL) results in a fully migrated,
  seeded database.
- Re-running the app against an already-migrated, already-seeded database is a no-op on both
  fronts.
- Seed data is sufficient for a full demo walkthrough (login with demo credentials → dashboard
  shows non-zero totals → training/technique lists are populated) without requiring manual data
  entry first.
