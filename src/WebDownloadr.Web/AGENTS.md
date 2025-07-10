---
inherits: ../../AGENTS.md
layer: Web
scope: API endpoints, middleware, authentication
---

# WebDownloadr Web Layer Rules

This file applies to `src/WebDownloadr.Web`.

## Layer Dependencies

- **Can reference** UseCases, Infrastructure and ServiceDefaults.
- Endpoints must interact with Infrastructure via interfaces or UseCase handlers.
- Direct access to `AppDbContext` is allowed only in `Program.cs` for dependency injection wiring.

## Endpoint Conventions

- Use [FastEndpoints](https://fast-endpoints.com/docs/introduction) for HTTP APIs.
- Name endpoint classes `<Verb><Entity>Endpoint` and place them under `Modules/<Feature>/`.
- Validate requests with FluentValidation and map to commands/queries in the UseCases layer.

### REPR DTO Pattern

```csharp
public sealed record CreateProjectRequest(string Name, DateOnly StartDate);

public sealed record CreateProjectResponse(int ProjectId);

```

- Keep request/response models flat and serializable.
- Do not expose domain entities directly.

## Program.cs Guidance

- Configure global exception handling and Serilog request logging.
- Register Infrastructure and UseCases via `InfrastructureServiceExtensions` and `UseCaseServiceExtensions`.

### Opt-In MVC / Razor

```csharp
builder.Services.AddControllers();
app.MapControllers();
```

_Only add controllers if FastEndpoints becomes insufficient._

## Authentication & Authorization

- Configure authentication schemes in this project.
- Apply `[Authorize]` on protected endpoints.

## Testing

- Functional tests should hit endpoints using `WebApplicationFactory` and HttpClient.
- Use snapshot testing for JSON when appropriate.
