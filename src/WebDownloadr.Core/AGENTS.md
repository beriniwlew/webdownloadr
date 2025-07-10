---
inherits: ../../AGENTS.md
layer: Core
scope: Domain entities, value objects, domain services, repository interfaces
---

# WebDownloadr Core Layer Rules

## Summary

Defines the **domain model** and related abstractions. The Core project contains entities, value objects, domain events, and repository
interfaces. It must remain infrastructure-agnostic and thoroughly tested.

This file supplements the repository root [AGENTS.md](../../AGENTS.md) for code under `src/WebDownloadr.Core`.

## Layer Dependencies

- **Can reference** only .NET primitives and the packages `Ardalis.GuardClauses` and `Ardalis.Specification`. These provide guard clause
  helpers and the specification pattern without pulling in infrastructure dependencies.
- **Cannot reference** UseCases, Infrastructure, or Web projects so that the domain model remains portable and free from UI or data-access
  concerns.

## Domain Patterns

### Domain-Driven Design Notes

- Treat each **aggregate** as a transactional boundary to ensure invariants hold whenever a unit is persisted.
- Use **value objects** to model concepts with rules so that invalid states are unrepresentable.
- Expose **behavior** on entities rather than manipulating their state externally; this keeps invariants centralized.

### Entities and Value Objects

- Place domain types under an `*Aggregate` folder (e.g., `WebPageAggregate`). This keeps related entities and value objects grouped
  together.
- Constructors validate all inputs using guard clauses so invalid objects are never created.
- Keep persistent constructor overloads `private` or `protected` for EF Core to allow materialization without exposing them publicly.
- Use value objects for compound types and implement `IEquatable<T>` to ensure equality is based on value semantics.
- Prefer immutable properties; modify state via methods that enforce invariants and trigger domain events.

```csharp
public sealed class Project
{
    public Project(string name, DateOnly startDate)
    {
        Name      = Guard.Against.NullOrEmpty(name);
        StartDate = Guard.Against.OutOfSQLDateRange(startDate);
    }

    public string  Name { get; private set; }
    public DateOnly StartDate { get; private set; }
}
```

#### Custom Guard Example

```csharp
public static class GuardExtensions
{
    public static void Negative(this IGuardClause guard, int value, string paramName) =>
        Guard.Against.NegativeOrZero(value, paramName, "Value must be positive.");
}
```

Guard extensions centralize domain validation logic. Use them to express rules succinctly and fail fast; they should throw exceptions rather
than silently correcting bad input.

### Domain Events

- Events are records deriving from `DomainEventBase` to provide a common base for dispatching.
- Register events within aggregate methods rather than dispatching directly so that persistence can coordinate when they fire.
- Events should describe completed business actions (e.g., `ProjectCompleted`) to keep them meaningful and side-effect free.
- Handlers live in the UseCases layer or Infrastructure; Core only raises them to remain decoupled from implementation concerns.

```csharp
public sealed class ProjectCompletedEvent(Project project) : DomainEventBase
{
    public Project Project { get; init; } = project;
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

Domain services encapsulate business logic that spans multiple aggregates or requires complex calculations. Keep them stateless and inject
implementations via interfaces so that unit tests can substitute fakes.

## Repository Interfaces

- Define abstractions such as `IRepository<T>` here to decouple the domain from the data-access technology.
- Persistence logic lives in Infrastructure implementations so Core can be tested without a database.
- Methods should be asynchronous and cancellation-token friendly to support scalability and graceful shutdown.
- Avoid leaking ORM types (e.g., `DbSet`) through the interface to prevent infrastructure-specific dependencies from creeping in.

## Testing

- Aim for **≥95%** line coverage in this project to catch regressions early.
- Unit tests must cover all public business logic.
- Use xUnit, Shouldly, and NSubstitute for a consistent test stack.
- Fakes for time or random number generation should live in the test projects to keep the Core project free from test-only code.
