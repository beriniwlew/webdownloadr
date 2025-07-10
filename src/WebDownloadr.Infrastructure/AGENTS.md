---
inherits: ../../AGENTS.md
layer: Infrastructure
scope: Persistence, EF Core, external integrations
---

# WebDownloadr Infrastructure Layer Rules

This file adds guidance for `src/WebDownloadr.Infrastructure`.

## Layer Dependencies

- **Can reference** `WebDownloadr.Core` and `WebDownloadr.UseCases`.
- **Cannot reference** the Web project.
- External packages such as EF Core or HTTP/SMTP clients belong here.

## Repository Implementation Pattern

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

## Database & Migrations

- Migrations live in `Data/Migrations`.
- Use `dotnet ef migrations add <Name>` from the Web project folder.
- Do **not** hand-edit generated migration files.

## External Services

- Implement outbound adapters (e.g., HTTP downloaders, email senders).
- Register services via `InfrastructureServiceExtensions`.

## Testing

- Integration tests may use TestContainers to spin up databases or message brokers.
- Unit tests can use `AppDbContext` with an in-memory provider.
