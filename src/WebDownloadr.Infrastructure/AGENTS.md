---
extends: ../../AGENTS.md
layer: Infrastructure
---

# AGENTS.md – Infrastructure Layer

This layer implements external dependencies and data access. It may reference Core and UseCases, but never the Web layer.

## Allowed Dependencies

- WebDownloadr.Core
- WebDownloadr.UseCases
- Entity Framework Core
- External SDKs (HTTP clients, cloud SDKs)
- Serilog or Microsoft.Extensions.Logging

## Prohibited

- Direct references to Web project code
- Placing business rules in repository classes

## Patterns

### EF Repository Implementation

```csharp
namespace WebDownloadr.Infrastructure.Data;

public sealed class EfRepository<T> : IRepository<T>
    where T : class, IAggregateRoot
{
    private readonly AppDbContext _db;
    private readonly ILogger<EfRepository<T>> _logger;

    public EfRepository(AppDbContext db,
                        ILogger<EfRepository<T>> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Persisting {EntityType} (Id ={EntityId})",
            typeof(T).Name,
            (entity as EntityBase)?.Id);

        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
    }
}
```

### AppDbContext Configuration

```csharp
public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
```

## Guidelines

- DbContext lives in `Data/` and is registered in `InfrastructureServiceExtensions`.
- Use Fluent API configurations instead of data annotations.
- Keep repository methods thin and focused on persistence.
- Store migrations under `Data/Migrations` and do not edit generated files.
- Integration tests may spin up database containers via TestContainers.

## Anti‑Patterns

- Referencing Web project types
- Catching and swallowing all exceptions

## Troubleshooting

- **Connection failures** – Verify connection strings and that the database server is reachable.
- **Migration conflicts** – Regenerate migrations instead of hand-editing when schemas diverge.

## Performance Considerations

- Prefer `AsNoTracking()` for read-only queries.
- Use bulk operations or batching when inserting large datasets.

## Security Guidelines

- Use parameterised queries or EF Core to avoid injection attacks.
- Store credentials outside source control and load via `IConfiguration`.

## Monitoring & Logging

- Log SQL commands at `Debug` level only when troubleshooting.
- Surface repository metrics (e.g., query duration) via `ILogger` or APM tools.
