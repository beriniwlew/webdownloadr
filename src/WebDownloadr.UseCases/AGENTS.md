---
inherits: ../../AGENTS.md
layer: UseCases
scope: Application commands, queries, handlers, DTOs
---

# WebDownloadr UseCases Layer Rules

This file describes guidance for `src/WebDownloadr.UseCases`.

## Layer Dependencies

- **Can reference** `WebDownloadr.Core`, MediatR, FluentValidation and other libraries.
- **Cannot reference** Infrastructure or Web projects.

## CQRS & Handlers

- Organize features under `Commands/` and `Queries/` folders.
- Commands and queries implement `IRequest<TResponse>`.
- Handlers are named `<Name>Handler`.
- Use pipeline behaviors for validation and logging.

### Validation Behavior Example

```csharp
public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) =>
        _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
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

### Command Handler Sample

```csharp
public sealed record StartDownloadCommand(string Url) : IRequest<Result<Guid>>;

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
using Ardalis.Result;

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
        CancellationToken ct)
    {
        _logger.LogInformation("Handling {Request}", typeof(TRequest).Name);
        var response = await next();
        _logger.LogInformation("Handled {Request}", typeof(TRequest).Name);
        return response;
    }
}
```

## DTO Guidelines

- Name with `<Verb><Entity>Request`/`Response`.
- Keep DTOs immutable and validation-friendly.
- Map to domain models using extension methods or Mapster.

## Testing

- Unit test each handler with mocks for repositories and services.
- Use NSubstitute for stubs and Shouldly for assertions.
- Avoid direct database calls; integration tests live in the test projects.
