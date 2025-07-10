---
extends: ../../AGENTS.md
layer: UseCases
---

# AGENTS.md – UseCases Layer

Handlers in this layer orchestrate the domain model through commands and queries. Only the Core layer may be referenced.

## Allowed Dependencies

- WebDownloadr.Core
- MediatR
- FluentValidation
- Ardalis.Result
- AutoMapper (profiles defined here)

## Prohibited

- Direct references to Infrastructure or Web projects
- Accessing DbContext or external services directly

## Patterns

### Validation Pipeline Behavior

```csharp
namespace WebDownloadr.UseCases.Shared.Behaviors;

using FluentValidation;
using MediatR;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 ct)
    {
        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

### CQRS Handler Example

```csharp
namespace WebDownloadr.UseCases.Downloads;

public sealed record StartDownloadCommand(string Url)
    : IRequest<Result<Guid>>;

public sealed class StartDownloadHandler
    : IRequestHandler<StartDownloadCommand, Result<Guid>>
{
    private readonly IRepository<DownloadRequest> _repo;
    private readonly ILogger<StartDownloadHandler> _logger;

    public StartDownloadHandler(IRepository<DownloadRequest> repo,
                                ILogger<StartDownloadHandler> logger)
    {
        _repo   = repo;
        _logger = logger;
    }

    public async Task<Result<Guid>> Handle(
        StartDownloadCommand request, CancellationToken ct)
    {
        Guard.Against.NullOrWhiteSpace(request.Url);

        var download = new DownloadRequest(request.Url);
        await _repo.AddAsync(download, ct);

        _logger.LogInformation(
            "Scheduled download {DownloadId} for {Url}",
            download.Id, download.TargetUrl);

        return Result.Success(download.Id);
    }
}
```

### Query Without Repository

```csharp
public sealed class GetProjectsQuery : IRequest<IEnumerable<ProjectDto>>;

public sealed class GetProjectsHandler
    : IRequestHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IDbConnection _db;
    public GetProjectsHandler(IDbConnection db) => _db = db;

    public async Task<IEnumerable<ProjectDto>> Handle(GetProjectsQuery q, CancellationToken ct) =>
        await _db.QueryAsync<ProjectDto>(
            "SELECT Id, Name FROM Projects ORDER BY Name");
}
```

### Consistent Result Pattern

```csharp
public async Task<Result<int>> Handle(CreateProjectCommand cmd, CancellationToken ct)
{
    if (await _repo.ExistsAsync(cmd.Name))
        return Result.Invalid(new ValidationError("name", "Duplicate"));

    var id = await _repo.AddAsync(new Project(cmd.Name, cmd.StartDate));
    return Result.Success(id);
}
```

### Structured Logging Behavior

```csharp
public sealed class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger) =>
        _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken                 ct)
    {
        _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled  {Request}", typeof(TRequest).Name);
        return response;
    }
}
```

### AutoMapper Profile Example

```csharp
public sealed class ProjectMappingProfile : Profile
{
    public ProjectMappingProfile()
    {
        CreateMap<Project, ProjectDto>();
    }
}
```

## Guidelines

- Commands and queries live under `Commands/` and `Queries/` folders.
- DTOs follow the REPR naming convention and are immutable.
- Register pipeline behaviors in DI: `services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));` etc.
- Use AutoMapper profiles in this project to map between domain entities and DTOs.
- Provide unit tests for each handler using xUnit and Shouldly.

## Anti‑Patterns

- Injecting DbContext directly
- Mixing validation and business logic in controllers

## Troubleshooting

- **Validation exceptions** – Ensure validators are registered in DI and pipeline behaviors are configured.
- **Handler not invoked** – Check that MediatR is registered and the request type matches the handler.

## Performance Considerations

- Prefer asynchronous repository methods and avoid synchronous blocking calls.
- Keep DTOs lightweight; project queries to DTOs instead of loading full entities when possible.

## Security Guidelines

- Never trust client input; validate again in handlers.
- Avoid exposing domain exceptions directly to callers; wrap them in `Result` types.

## Monitoring & Logging

- Use the provided `LoggingBehavior` to capture request/response information.
- Include correlation identifiers in logs to trace individual operations.
