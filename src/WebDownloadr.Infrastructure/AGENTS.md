# WebDownloadr Infrastructure Layer Rules

> **Scope**: /src/<SolutionName>.Infrastructure and all sub‑folders — the Infrastructure layer.
> **Inheritance**: Extends root‑level AGENTS.md. Every rule here adds to or overrides global guidance.

## 1  Layer Purpose & Boundaries

- **Infrastructure** is the **outermost layer**. It implements abstractions defined in **Core** (repositories, Unit‑of‑Work, file/email services) *and* in **UseCases** (query‑service interfaces, external API clients).
- **Allowed project references**: `Core`, `UseCases`, third‑party NuGets (EF Core, Dapper, Serilog, Polly, etc.).
- **Must not reference**: `Web`, any UI layer, or production secrets (use configuration).

---

## 2  Folder & File Layout (template v10)

```text
/Infrastructure/
  ├─ Data/
  │     - AppDbContext.cs
  │     - EfRepository.cs
  │     ├─ Configurations/            # IEntityTypeConfiguration<>
  │     │     - ContributorConfiguration.cs
  │     └─ Migrations/                # EF Core migrations
  │
  ├─ Email/
  │     - SmtpEmailSender.cs
  │     - MailserverConfiguration.cs  # bound via IOptions
  │
  ├─ DependencyInjection.cs           # IServiceCollection.AddInfrastructure()
  │
  └─ README.md                        # optional – quick dev notes
```

**Notes**

- Persistence code (repositories, unit‑of‑work, interceptors) lives in the **Data/** folder; the current template no longer has a separate `Repositories/` folder.
- External adapters each get their own top‑level folder; the shipped template includes only **Email** by default.
- If you add more adapters (e.g., `Files/`), follow the same flat folder pattern and keep one public type per file.

## 3  Data Access (EF Core)  Data Access (EF Core)

### 3.1  AppDbContext

```csharp
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project>   Projects   => Set<Project>();
    public DbSet<ToDoItem>  ToDoItems  => Set<ToDoItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        // Soft‑delete filter example:
        b.Entity<Project>().HasQueryFilter(p => !p.IsDeleted);
    }
}
```

### 3.2  Generic EF Repository

```csharp
public sealed class EfRepository<T>(AppDbContext db) : IRepository<T> where T : class, IAggregateRoot
{
    public Task<T?>   GetByIdAsync(int id, CancellationToken ct) => db.Set<T>().FindAsync([id], ct).AsTask();
    public async Task<T> AddAsync(T entity, CancellationToken ct)
    {
        await db.Set<T>().AddAsync(entity, ct);
        return entity;
    }
    public Task   DeleteAsync(T entity, CancellationToken ct)
        => Task.FromResult(db.Set<T>().Remove(entity));
}
```

### 3.3  Unit‑of‑Work

```csharp
public sealed class EfUnitOfWork(AppDbContext db) : IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
```

---

## 4  Query‑Service Implementation (Projects)

```csharp
internal sealed class ListProjectsShallowQueryService(AppDbContext db) : IListProjectsShallowQueryService
{
    public async Task<IReadOnlyList<ProjectDto>> ListAsync(int? skip, int? take, CancellationToken ct)
    {
        var query = db.Projects.AsNoTracking()
                     .Select(p => new ProjectDto
                     {
                         Id         = p.Id,
                         Name       = p.Name.Value,
                         ItemsCount = p.Items.Count,
                         CreatedOn  = p.CreatedOn
                     })
                     .OrderBy(p => p.Name);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return await query.ToListAsync(ct);
    }
}
```

*Implements the interface from UseCases; registered in DI below.*

---

## 5  Dependency‑Injection Extension

```csharp
public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        // DbContext
        var conn = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(conn)); // swap for SqlServer or Postgre

        // Repos & UoW
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<IUnitOfWork, EfUnitOfWork>();

        // Query Services
        services.Scan(selector => selector
            .FromAssemblyOf<ListProjectsShallowQueryService>()
            .AddClasses(c => c.InNamespaces("*.QueryServices"))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        // External services
        services.AddScoped<IEmailSender, SmtpEmailSender>();

        return services;
    }
}
```

---

## 6  Migrations & Seeding

1. **Add a migration** (from solution root):

   ```bash
   dotnet ef migrations add InitialCreate -p src/<Solution>.Infrastructure -s src/<Solution>.Web -o Data/Migrations
   ```
2. **Apply migrations** on startup (`Web` project) with `db.Database.Migrate();`
3. Seed development data in `AppDbContextSeed.SeedAsync()` and call it from `Program` after migration.

---

## 7  Testing Guidance

- **DbContext‑level tests** – use `SqliteConnection("DataSource=:memory:")` and keep connection open.
- **Repository tests** – verify CRUD + specification filters.
- **QueryService tests** – run against SQLite in‑memory or mocked DbContext via EFCore.Testing.

### 7.1  Sample Integration Test (SQLite in‑memory)

```csharp
public class EfRepositoryAddTests
{
    [Fact]
    public async Task AddsEntityAndSaves()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        using var context = new AppDbContext(options);
        context.Database.EnsureCreated();

        var repo = new EfRepository<Project>(context);
        var project = new Project(ProjectName.From("Test"));

        await repo.AddAsync(project, CancellationToken.None);
        await context.SaveChangesAsync();

        Assert.NotEqual(0, project.Id);
    }
}
```

---

## 8  Agent Post‑Generation Steps

1. `dotnet build src/<SolutionName>.Infrastructure -c Release` — zero warnings.
2. `dotnet test tests/IntegrationTests --filter Category=Infrastructure` — all green.
3. `dotnet format --verify-no-changes` — style passes.

---

## 9  Checklist for Agents (tick before committing)

-

---

*End of Infrastructure guidelines*
