# Backend Execution Plan — BJJManager API

Implements the backend slice of `00-master-spec.md`. Do not redefine entities/endpoints/rules
here — reference the master spec section numbers instead.

## 1. Solution layout (Clean Architecture, monorepo path `/backend`)

```
/backend
  BJJManager.sln
  /src
    BJJManager.Domain/          # entities, enums, domain exceptions — zero dependencies
    BJJManager.Application/     # MediatR commands/queries/handlers, DTOs, FluentValidation validators,
                                 # repository & service interfaces (IUserRepository, ITrainingSessionRepository,
                                 # ITechniqueRepository, IPasswordHasher, IJwtTokenGenerator, ICurrentUserService)
    BJJManager.Infrastructure/   # EF Core DbContext, migrations, repository implementations,
                                 # JwtTokenGenerator, PasswordHasher wrapper, DI registration
    BJJManager.WebApi/           # Controllers (thin), Program.cs, JWT bearer auth, Swagger, CORS,
                                 # global exception -> ProblemDetails middleware
  /tests
    BJJManager.Domain.Tests/
    BJJManager.Application.Tests/
    BJJManager.WebApi.IntegrationTests/   # WebApplicationFactory + EF Core SQLite/InMemory provider
```

Dependency direction: WebApi → Application + Infrastructure; Infrastructure → Application →
Domain. Application never references Infrastructure or WebApi, keeping the business logic layer
independent of the data layer and API.

## 2. Build order (TDD: red → green → refactor per step)

1. **Domain**: `User`, `TrainingSession`, `TrainingMedia`, `Technique`, `TechniqueStep`,
   `TechniqueMedia`, `MediaType` enum, `SelfEvaluation` enum. Unit tests first for the one real
   domain invariant: a `Technique` cannot be constructed/left with zero steps, and removing the
   last remaining step must be rejected at the aggregate level.
2. **Application — Auth**:
   - `RegisterUserCommand` (+handler, +validator: name required, password ≥6 chars) → hashes via
     `IPasswordHasher`, persists via `IUserRepository`.
   - `LoginUserCommand` (+handler, +validator) → looks up by name, verifies hash, issues JWT via
     `IJwtTokenGenerator`. Ambiguous "name not unique" case: on multiple users sharing a name,
     attempt verification against each match; succeed on first hash match, else 401. (Simple,
     avoids requiring uniqueness the spec explicitly says isn't required.)
   - Tests against mocked repository/hasher/token generator — no EF, no HTTP.
3. **Application — Trainings**: `CreateTrainingCommand`, `UpdateTrainingCommand`,
   `DeleteTrainingCommand`, `AddTrainingMediaCommand`, `RemoveTrainingMediaCommand`,
   `GetTrainingsQuery` (date/month/year filters), `GetTrainingByIdQuery`. Every handler takes
   `UserId` from `ICurrentUserService` (populated by WebApi from JWT claims), never trusts a
   client-supplied user id. Not-owned-or-missing → domain-level `NotFoundException` mapped to 404.
4. **Application — Techniques**: `CreateTechniqueCommand` (requires ≥1 step in the request DTO —
   validator rejects empty step list), `UpdateTechniqueCommand` (replaces steps wholesale,
   handler renumbers `Order` 1..N from the submitted list order), `DeleteTechniqueCommand`,
   media add/remove commands, `GetTechniquesQuery`, `GetTechniqueByIdQuery`.
5. **Application — Dashboard**: `GetDashboardSummaryQuery` → counts via repositories.
6. **Application pipeline behavior**: `ValidationBehavior<TRequest,TResponse>` running
   FluentValidation validators before handlers execute; unhandled validation errors surface as
   400 with field-level messages.
7. **Infrastructure**: `BjjManagerDbContext` (EF Core), entity configurations (Fluent API, one
   `IEntityTypeConfiguration<T>` per entity — see database plan for column shapes), repository
   implementations, `EfPasswordHasher` wrapping `Microsoft.AspNetCore.Identity.PasswordHasher<User>`,
   `JwtTokenGenerator` (HS256, claims: `NameIdentifier`=UserId, `Name`), migrations.
8. **WebApi**: `AuthController`, `TrainingsController`, `TechniquesController`,
   `DashboardController` — each action just constructs a MediatR request and returns
   `Ok`/`Created`/`NoContent`/`NotFound` based on the result; JWT bearer middleware configured
   from `Jwt:Key/Issuer/Audience/ExpiryMinutes` in `appsettings.json`; global exception middleware
   maps `ValidationException`→400, `NotFoundException`→404, `UnauthorizedAccessException`→401,
   anything else→500 ProblemDetails; CORS policy allowing the Angular dev origin
   (`http://localhost:4200`); Swagger/OpenAPI with a bearer auth scheme configured for manual
   token testing.
9. **Integration tests**: one happy-path + one authz-violation test per controller action, using
   `WebApplicationFactory<Program>` against an EF Core in-memory/SQLite provider seeded per test.

## 3. Cross-references

- Connection string: see repo's `Chaves e StringConnections.txt` → goes into
  `appsettings.Development.json` (`ConnectionStrings:DefaultConnection`), not hardcoded.
- DB schema/seed: `03-database-execution-plan.md` — Infrastructure's EF migrations are the only
  source of truth for the schema; `Program.cs` applies them and runs `DbSeeder` automatically on
  startup (see that plan for the exact workflow).
- Frontend contract expectations (routes, DTO shapes, error format): `02-frontend-execution-plan.md`
  and §4 of the master spec.

## 4. Definition of done for this slice

- All Application handlers have unit tests written before the handler code (or at minimum,
  committed alongside with clear red→green history if TDD strict-ordering isn't practical to
  demonstrate commit-by-commit).
- Domain, Application, and WebApi/Infrastructure each have their own test project passing.
- `dotnet test` green across the solution.
- Swagger UI can register, log in, and call every endpoint manually with the returned bearer
  token.
- No code comments anywhere in `/backend`.
