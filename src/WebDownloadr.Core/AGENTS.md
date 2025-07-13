# AGENTS.md — Core Layer Guidelines (Validated against NimblePros.SampleToDo.Core)

> **Scope**: `/src/<SolutionName>.Core` and sub‑folders.\
> **Inheritance**: Extends repo‑root **AGENTS.md**; rules here add to / override the global ones.\
> **Reference sample**: *NimblePros.SampleToDo.Core* (local copy of the Clean Architecture sample).

---

## 1  Layer Purpose & Boundaries

- **Core** is the **Domain layer**. It owns *pure* business rules: **aggregates, entities, value objects, domain events, enums, specifications**, and **abstractions** (interfaces).
- **Zero outward dependencies** – Core must not reference UseCases, Web, Infrastructure, EF Core, MediatR, FastEndpoints, etc. Allowed NuGets: *Ardalis.GuardClauses*, *Ardalis.Specification*, *Vogen*, *NetEscapades.EnumGenerators*.
- Public APIs expose **behaviour**; state mutation stays private or protected.

---

## 2  Folder & File Layout (mirrors sample)

```
/Core/
  ├─ <Aggregate>NameAggregate/           # one folder per aggregate root
  │     • <Aggregate>.cs                 # aggregate root entity
  │     • <SubEntity>.cs                 # supporting entities
  │     • ValueObjects/                  # optional VO sub‑folder
  │     • Events/                        # domain events
  │     • Specifications/                # aggregate‑specific specs
  ├─ SharedKernel/                       # cross‑aggregate base types
  │     • EntityBase.cs
  │     • EntityBase<TId>.cs             # generic variant
  │     • DomainEventBase.cs
  └─ Interfaces/                         # cross‑aggregate interfaces (optional)
```

*File names match type names; one top‑level type per file.*

---

## 3  Coding Conventions

| Topic               | Rule                                                                                         |
| ------------------- | -------------------------------------------------------------------------------------------- |
| **Language / TFM**  | C# 12 on `net9.0`; nullable enabled.                                                         |
| **Base types**      | Derive entities/aggregates from `EntityBase` or `EntityBase<TId>` found in **SharedKernel**. |
| **Behaviour first** | Expose verbs (`AddItem`, `MarkComplete`) publicly; keep setters private.                     |
| **Constructors**    | Always enforce invariants with *Ardalis.GuardClauses*.                                       |
| **Domain events**   | Raise events inside behaviour and call `RegisterDomainEvent(...)`.                           |

---

## 4  Entities & Aggregates

### 4.1  Key Rules

1. **Strong identity** – Prefer strongly‑typed IDs (Vogen or value‑object) when feasible.
2. **Encapsulated collections** – Expose `IEnumerable<T>` or `IReadOnlyList<T>`; mutate via methods.
3. **Status via behaviour** – Derived properties (e.g., `Status`) should compute from child state.
4. **Equality** – Entities compare by identity; value objects compare by value.


### 4.2  Core Layer Sample Snippets

The following self‑contained examples compile inside a blank **Core** project that already contains `EntityBase`, `DomainEventBase`, and the Ardalis helper packages.

---

## 1. Value Object with Vogen — `Email.cs`

```csharp
using Vogen;
using Ardalis.GuardClauses;

[ValueObject(typeof(string))]
public partial class Email
{
    private static Validation Validate => static (value) =>
    {
        Guard.Against.NullOrWhiteSpace(value, nameof(value));
        Guard.Against.InvalidInput(value, nameof(value), v => v.Contains('@'));
    };
}
```

---

## 2. Domain Event — `ProjectArchivedEvent.cs`

```csharp
public sealed record ProjectArchivedEvent(int ProjectId) : DomainEventBase;
```

**Raising inside an aggregate**

```csharp
public Project Archive()
{
    if (Status == ProjectStatus.Archived) return this;

    Status = ProjectStatus.Archived;
    RegisterDomainEvent(new ProjectArchivedEvent(Id));
    return this;
}
```

---

## 3. Specification — `ProjectsByOwnerSpec.cs`

```csharp
using Ardalis.Specification;

public sealed class ProjectsByOwnerSpec : Specification<Project>
{
    public ProjectsByOwnerSpec(int ownerUserId)
    {
        Query.Where(p => p.OwnerId == ownerUserId)
             .Include(p => p.Items)
             .OrderByDescending(p => p.CreatedOn);
    }
}
```

---

## 4. Strongly‑Typed ID — `ProjectId.cs`

```csharp
public readonly record struct ProjectId(int Value)
{
    public override string ToString() => Value.ToString();
    public static implicit operator int(ProjectId id) => id.Value;
    public static implicit operator ProjectId(int value) => new(value);
}
```

---

## 5. Unit Test for Specification — `ProjectsByOwnerSpecTests.cs`

```csharp
public class ProjectsByOwnerSpecTests
{
    [Fact]
    public void FiltersProjectsByOwnerAndOrdersDesc()
    {
        // Arrange
        var owned = new Project(ProjectName.From("Mine")) { OwnerId = 42, CreatedOn = DateTime.UtcNow };
        var notOwned = new Project(ProjectName.From("Yours")) { OwnerId = 7, CreatedOn = DateTime.UtcNow.AddDays(-1) };
        var source = new[] { notOwned, owned }.AsQueryable();

        var spec = new ProjectsByOwnerSpec(42);

        // Act
        var result = SpecificationEvaluator.Default.GetQuery(source, spec).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(owned, result[0]);
    }
}
```

---

## 6. Aggregate with Encapsulated Collection — `Project.cs` (condensed)

```csharp
public class Project : EntityBase, IAggregateRoot
{
    private readonly List<ToDoItem> _items = [];
    public IReadOnlyList<ToDoItem> Items => _items;

    public Project AddItem(string title)
    {
        Guard.Against.NullOrWhiteSpace(title);
        _items.Add(new ToDoItem(title));
        return this;
    }
}
```

---

*End of snippets*

*Adapted from the sample project.*

---

## 5  Value Objects

- Prefer **Vogen**‑generated structs for lightweight, performant IDs and simple VOs (e.g., `ProjectName`).
- Alternative: classic immutable record or class inheriting from a `ValueObject` helper.
- Validation lives in a static factory (`From(string)`), constructor, or Vogen `Validate()` delegate.

---

## 6  Domain Events

- Base class: `DomainEventBase` (in **SharedKernel**).
- Events named `<SomethingHappened>Event` and placed under `<Aggregate>Aggregate/Events/`.
- Pass only primitives/IDs or aggregate references – never EF proxies.
- Example: `NewItemAddedEvent` is raised in `Project.AddItem()`.

---

## 7  Specifications

- Use **Ardalis.Specification** for rich queries.
- Folder location: aggregate’s own `/Specifications` folder, or `/Core/Specifications` if shared.
- Naming convention: `<Aggregate><Filter>Description>Spec` (e.g., `ProjectsByStatusSpec`).

---

## 8  Testing Checklist (Core)

-

---

## 9  Agent Post‑Generation Steps

1. `dotnet build src/<SolutionName>.Core -c Release` — zero warnings.
2. `dotnet test tests/UnitTests --filter Category=Core` — all green.
3. `dotnet format --verify-no-changes` — style passes.

---

## 10  Checklist for Agents (tick before committing)

-

---

*End of Core guidelines*

