# AGENTS.md — Core Layer Guidelines (Validated against NimblePros.SampleToDo.Core)

> **Scope**: /src/<SolutionName>.Core and all sub‑folders — the Domain layer.
> **Inheritance**: Extends root‑level AGENTS.md. Every rule here adds to or overrides global guidance.

---

## 1  Layer Purpose & Boundaries

| ✅ Allowed                                                                                                                    | ❌ Forbidden                                                                                                         |
| ---------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------- |
| Aggregates, entities, value objects, domain events, enums, specifications, domain‑level interfaces | Any infrastructure tech (EF Core, Dapper, Serilog, etc.), Web/UI types, UseCases references, concrete external APIs |

Helper NuGets permitted: `Ardalis.GuardClauses`, `Ardalis.Specification`, `Vogen`, `NetEscapades.EnumGenerators`.

---

## 2  Folder & File Layout

```text
/Core/
  ├─ ProjectAggregate/           # one folder per aggregate root
  │     • Project.cs            # aggregate root entity
  │     • ToDoItem.cs           # child entity
  │     • ProjectName.cs        # value object lives alongside entities
  │     • Events/
  │          └─ NewItemAddedEvent.cs
  │     • Specifications/
  │          └─ ProjectsByStatusSpec.cs
  ├─ Interfaces/                # cross‑aggregate abstractions (e.g., IDateTime)
  ├─ Services/                  # domain services operating across aggregates
  │     • OverdueCalculator.cs  # example domain service
  └─ SharedKernel/              # reusable base types (EntityBase, DomainEventBase)
```

*One public type per file; file name == type name.*

---

## 3  Aggregates & Entities

### 3.1  Key Rules

1. **Strong identity** — use value objects or Vogen‑generated IDs (`ProjectId`).
2. **Encapsulated collections** — expose `IReadOnlyList<T>`; mutate internally.
3. **Behaviour‑first** — verbs over setters; invariants enforced with guard clauses.
4. **Domain events** — raise events inside behaviour.

### 3.2  `Project` Aggregate Root (abridged)

```csharp
public class Project : EntityBase, IAggregateRoot
{
    private readonly List<ToDoItem> _items = new();
    public ProjectName Name { get; private set; }

    // Derived property — no backing field
    public ProjectStatus Status =>
        _items.All(i => i.IsDone) ? ProjectStatus.Complete : ProjectStatus.InProgress;

    public Project(ProjectName name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public ToDoItem AddItem(string title)
    {
        Guard.Against.NullOrWhiteSpace(title);
        var item = new ToDoItem(title);
        _items.Add(item);
        RegisterDomainEvent(new NewItemAddedEvent(this, item));
        return item;
    }

    public Project Archive()
    {
        if (Status == ProjectStatus.Archived) return this;
        RegisterDomainEvent(new ProjectArchivedEvent(Id));
        // …additional logic (set status, audit, etc.)
        return this;
    }
}
```

---

## 4  Value Objects

### 4.1  Guidelines

- Prefer **Vogen** for single‑value VOs / IDs.
- Alternative: immutable record/class with value‑equality.
- Validate during creation (`From()` or Vogen `Validate`).

### 4.2  `ProjectName` (Vogen example)

```csharp
using Vogen;
using Ardalis.GuardClauses;

[ValueObject(typeof(string))]
public partial class ProjectName
{
    private static Validation Validate => static value =>
    {
        Guard.Against.NullOrWhiteSpace(value);
        Guard.Against.LengthGreaterThan(value, 200);
    };
}
```

### 4.3  `Email` (non‑Vogen VO)

```csharp
public sealed record Email
{
    public string Value { get; }
    private Email(string value) => Value = value;

    public static Email From(string value)
    {
        Guard.Against.NullOrWhiteSpace(value);
        Guard.Against.InvalidInput(value, nameof(value), v => v.Contains('@'));
        return new Email(value.Trim());
    }

    public override string ToString() => Value;
}
```

---

## 5  Domain Events

### 5.1  `NewItemAddedEvent`

```csharp
public sealed record NewItemAddedEvent(Project Project, ToDoItem Item) : DomainEventBase;
```

### 5.2  `ProjectArchivedEvent`

```csharp
public sealed record ProjectArchivedEvent(int ProjectId) : DomainEventBase;
```

*Events live in **``**; carry only primitives or aggregate refs.*

---

## 6  Services

Domain **services** encapsulate **business rules that don’t naturally belong inside a single entity or value object**. They come in two flavours:

| Kind                 | Purpose                                                             | Where it lives                                             | Example                                           |
| -------------------- | ------------------------------------------------------------------- | ---------------------------------------------------------- | ------------------------------------------------- |
| **Domain Service**   | Pure computation across aggregates; no external dependencies.       | `/Core/Services/` or inside an aggregate folder if scoped. | `OverdueCalculator` below                         |
| **Domain Interface** | Abstraction over external concern; concrete impl in Infrastructure. | `/Core/Interfaces/`                                        | `IDateTime`, `IEmailSender`, `ICurrencyConverter` |

### 6.1  Domain Service Example — `OverdueCalculator`

```csharp
namespace MySolution.Core.Services;

public sealed class OverdueCalculator
{
    public bool IsOverdue(ToDoItem item, DateTime asOf)
        => !item.IsDone && item.DueOn.HasValue && item.DueOn.Value < asOf;
}
```

*Stateless, deterministic, unit‑testable without mocks.*

### 6.2  Domain Interface Example — `IDateTime`

```csharp
namespace MySolution.Core.Interfaces;

public interface IDateTime
{
    DateTime UtcNow { get; }
}
```

*Infrastructure supplies an implementation (e.g., **`SystemDateTime`**) registered in DI; Core code depends only on the interface for time‑based logic.*

### 6.3  When to introduce a Service

- A calculation queries **multiple aggregates** (violating aggregate boundary inside an entity).
- A rule requires **external data** (time, currency rates, email), in which case expose it via an **interface**.
- Shared behaviour reused by several aggregates (e.g., pricing, tax, metrics).

---

## 7  Specifications

### 7.1  `ProjectsByOwnerSpec`

```csharp
using Ardalis.Specification;

public sealed class ProjectsByOwnerSpec(int ownerId) : Specification<Project>
{
    public ProjectsByOwnerSpec(int ownerId)
    {
        Query.Where(p => p.OwnerId == ownerId)
             .Include(p => p.Items)
             .OrderByDescending(p => p.CreatedOn);
    }
}
```

*Place specs inside **``**; reuse across repos and query services.*

---

## 8  Testing Checklist (Core)

-

### 8.1  Spec Unit Test Example (pure LINQ)

```csharp
public class ProjectsByOwnerSpecTests
{
    [Fact]
    public void FiltersByOwnerAndOrdersDesc()
    {
        var mine   = new Project(ProjectName.From("Mine")) { OwnerId = 42 };
        var theirs = new Project(ProjectName.From("Yours")) { OwnerId = 7  };
        var src    = new[] { theirs, mine }.AsQueryable();

        var spec   = new ProjectsByOwnerSpec(42);
        var result = SpecificationEvaluator.Default.GetQuery(src, spec).ToList();

        Assert.Single(result);
        Assert.Equal(mine, result[0]);
    }
}
```

---

## 9  Agent Post‑Generation Steps

1. `dotnet build src/<SolutionName>.Core -c Release` — zero warnings.
2. `dotnet test tests/UnitTests --filter Category=Core` — all green.
3. `dotnet format --verify-no-changes` — style passes.

---

## 10  Checklist for Agents (tick before committing)

-

---

> *End of Core guidelines*

