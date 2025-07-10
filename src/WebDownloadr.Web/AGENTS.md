---
extends: ../../AGENTS.md
layer: Web
---

# AGENTS.md – Web Layer

The Web project hosts the API using FastEndpoints and composes all other layers. It may reference UseCases and Infrastructure, but infrastructure types should be injected via DI.

## Allowed Dependencies
- WebDownloadr.Core
- WebDownloadr.UseCases
- WebDownloadr.Infrastructure (only in Program/Startup)
- FastEndpoints
- ASP.NET Core middleware and authentication packages
- Serilog or Microsoft.Extensions.Logging

## Prohibited
- Calling Infrastructure classes directly from endpoints (except in Program.cs)
- Exposing domain entities over the wire

## Patterns

### REPR DTO Pattern

```csharp
// Request  – validated via FluentValidation
public sealed record CreateProjectRequest(string Name, DateOnly StartDate);

// Response – thin & serializable
public sealed record CreateProjectResponse(int ProjectId);
```

### Middleware and Authentication
- Register global exception handling middleware in `Program.cs`.
- Use JWT bearer authentication. Configure policies in the `Configurations/` folder.

### FastEndpoints Example

```csharp
public sealed class CreateProjectEndpoint : Endpoint<CreateProjectRequest, CreateProjectResponse>
{
    private readonly ISender _mediator;
    public override void Configure()
        => Post("/projects").AllowAnonymous();

    public CreateProjectEndpoint(ISender mediator) => _mediator = mediator;

    public override async Task HandleAsync(CreateProjectRequest req, CancellationToken ct)
        => await SendAsync(await _mediator.Send(new CreateProjectCommand(req.Name, req.StartDate), ct));
}
```

## Guidelines
- Endpoints live under `Modules/<Feature>` and should delegate to use case handlers.
- Map DTOs to commands/queries using AutoMapper profiles defined in UseCases.
- Keep `Program.cs` as the composition root for DI wiring.
- Document endpoints via Swagger / FastEndpoints summary attributes.

## Anti‑Patterns
- Business logic inside endpoints
- Returning raw exceptions to clients

## Troubleshooting
- **404 errors** – Ensure route patterns match the endpoint `Configure` method.
- **DI failures** – Verify all dependencies are registered in `Program.cs`.

## Performance Considerations
- Keep endpoint logic thin and asynchronous.
- Use response caching or compression where appropriate.

## Security Guidelines
- Enforce authentication and authorization policies on endpoints.
- Validate request models and sanitize all user input.

## Monitoring & Logging
- Log HTTP requests and responses using Serilog or the built-in logging framework.
- Propagate correlation IDs so downstream services can be traced.
