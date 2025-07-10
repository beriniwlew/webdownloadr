---
extends: ../../AGENTS.md
layer: Core
---

# AGENTS.md – Core Layer

The Core layer contains the domain model and must remain free of infrastructure concerns.

## Allowed Dependencies
- .NET Standard Library only
- [Ardalis.GuardClauses](https://github.com/ardalis/GuardClauses)
- [Ardalis.Specification](https://github.com/ardalis/Specification)

## Prohibited
- References to UseCases, Infrastructure, or Web projects
- Framework-specific types (EF Core attributes, ASP.NET types)

## Patterns

### Guard-Clause Invariants
```csharp
namespace WebDownloadr.Core.ProjectAggregate;

using Ardalis.GuardClauses;

public sealed class Project
{
    public Project(string name, DateOnly startDate)
    {
        Name      = Guard.Against.NullOrEmpty(name);
        StartDate = Guard.Against.OutOfSQLDateRange(startDate);
    }

    public string  Name      { get; private set; }
    public DateOnly StartDate { get; private set; }
}
```

#### Guard-Clause Extension
```csharp
namespace WebDownloadr.Core.Guards;

using Ardalis.GuardClauses;

public static class GuardExtensions
{
    public static void Negative(this IGuardClause guard, int value, string paramName) =>
        Guard.Against.NegativeOrZero(value, paramName, "Value must be positive.");
}
```

### Raising a Domain Event
```csharp
namespace WebDownloadr.Core.ProjectAggregate.Events;

public sealed class ProjectCompletedEvent(Project project) : DomainEventBase
{
    public Project Project { get; init; } = project;
}
```
```csharp
namespace WebDownloadr.Core.ProjectAggregate;

public sealed partial class Project
{
    public void MarkComplete()
    {
        if (!IsDone)
        {
            IsDone = true;
            RegisterDomainEvent(new ProjectCompletedEvent(this));
        }
    }
}
```

### Domain Service Example
```csharp
public interface IInvoiceNumberGenerator
{
    InvoiceNumber Next();
}

public sealed class InvoiceNumberService : IInvoiceNumberGenerator
{
    private int _current;
    public InvoiceNumber Next() => new(++_current);
}
```

### Value Object Example
```csharp
public sealed record EmailAddress(string Value)
{
    public static EmailAddress Create(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        Guard.Against.InvalidFormat(value, nameof(value),
            _ => value.Contains('@'));
        return new(value.ToLowerInvariant());
    }
}
```

## Guidelines
- Define domain entities under `/src/WebDownloadr.Core/<Aggregate>/`.
- Keep aggregates persistence-ignorant and raise domain events for side effects.
- Repository interfaces belong in `Interfaces/` and should be thin abstractions.
- Specifications encapsulate query logic (`Ardalis.Specification`).
- Aim for **≥95%** line coverage in this layer.

## Anti‑Patterns
- Throwing UI-specific exceptions
- Injecting infrastructure services directly

## Troubleshooting
- **Failing invariants** – Ensure guard clauses cover all constructor parameters.
- **Missing domain events** – Verify entities call `RegisterDomainEvent` when state changes.

## Performance Considerations
- Keep aggregates small and immutable where possible.
- Avoid expensive calculations in property getters; prefer explicit methods.

## Security Guidelines
- Validate all inputs via guard clauses.
- Do not log sensitive data from domain entities.

## Monitoring & Logging
- Domain layer should not log directly; raise events consumed by outer layers.
