# AGENTS.md — UseCases Layer Overrides

> **Scope**: `/src/<SolutionName>.UseCases` and sub‑folders only.
> **Inheritance rule**: This file *extends* the repository‑level **AGENTS.md**. Follow every global rule unless an item below **adds** to or **overrides** it.

---

## 1 Layer Boundaries

* **UseCases** is the **Application layer**. It may reference **Core** and NuGet packages, but **must not** reference **Web**, **Infrastructure**, or any concrete external technology (e.g., EF Core, ASP.NET types).
* Cross‑cutting concerns live in **MediatR pipeline behaviours**.

---

## 2 Folder & File Layout

```
/UseCases/<Feature>/
  • VerbNoun.cs            // Command (IRequest<TResult>)
  • VerbNounHandler.cs     // Sealed handler
  • VerbNounValidator.cs   // FluentValidation rules
  • NounQuery.cs           // Optional query (read‑side)
```

*For a ****single‑request**** feature, use the flat structure shown above.*

*For a ****multi‑request**** feature (e.g., **`Projects`** with **`Create`** & **`ListShallow`**), nest each request in its own sub‑folder:*

```
/UseCases/<Feature>/<Action>/
  • <Action>.cs              // Command or Query DTO
  • <Action>Handler.cs       // IRequestHandler implementation
  • <Action>Validator.cs     // Optional AbstractValidator
```

*Avoid adding an extra “Commands” or “Queries” layer; the ****action folder**** names already convey intent.*

---

## 3 Coding & Validation Conventions

* C# 12, `net9.0`, nullable enabled, implicit usings on (inherits root `.editorconfig`).
* Commands === writes; Queries === reads (CQRS).
* All commands/queries are dispatched via **MediatR** (`IRequest<T>`/`IRequest<Unit>`).
* Add an `AbstractValidator<TRequest>` per request; rely on the shared `ValidationBehavior` to execute them.

---

## 4 Data‑Access Guidance

| Scenario                 | Allowed approach                                                                | Forbidden                         |
| ------------------------ | ------------------------------------------------------------------------------- | --------------------------------- |
| **Create/Update/Delete** | `IRepository<TEntity>` + `IUnitOfWork`                                          | Direct `DbContext` access         |
| **Read**                 | `IRepository<TEntity>` with `ISpecification<T>` **or** read‑service abstraction | Inline SQL or EF LINQ in handlers |

*Handlers must map domain entities to DTOs before returning.*

---

## 5 Testing Checklist (UseCases)

For **every** new command/query:

1. Unit‑test the **handler** with mocks/stubs for repositories.
2. Unit‑test any **validator** rules.
3. Integration tests (optional) go under `tests/IntegrationTests`.

---

## 6 Agent Post‑Generation Steps

1. `dotnet build src/<SolutionName>.UseCases -c Release` – zero warnings.
2. `dotnet test tests/UnitTests --filter Category=UseCases` – all green.
3. `dotnet format --verify-no-changes` – style clean.

(See root **AGENTS.md** §5 for full CI gates.)

---

## 7 Worked Example — Todos Feature

Below is a minimal, end‑to‑end slice that follows every rule above. Copy‑paste and adjust names for new features.

```
/UseCases/Todos/
  • CreateTodo.cs
  • CreateTodoHandler.cs
  • CreateTodoValidator.cs
```

### 7.1 Command — CreateTodo.cs

```csharp
public sealed record CreateTodo(string Title) : IRequest<int>;
```

### 7.2 Handler — CreateTodoHandler.cs

```csharp
public sealed class CreateTodoHandler : IRequestHandler<CreateTodo, int>
{
    private readonly IRepository<TodoItem> _repo;
    private readonly IUnitOfWork _uow;

    public CreateTodoHandler(IRepository<TodoItem> repo, IUnitOfWork uow)
        => (_repo, _uow) = (repo, uow);

    public async Task<int> Handle(CreateTodo request, CancellationToken ct)
    {
        var todo = new TodoItem(request.Title);
        await _repo.AddAsync(todo, ct);
        await _uow.SaveChangesAsync(ct);
        return todo.Id;
    }
}
```

*Notes:* Mutations go through `IRepository`/`IUnitOfWork`; no direct DbContext access.

### 7.3 Validator — CreateTodoValidator.cs

```csharp
public sealed class CreateTodoValidator : AbstractValidator<CreateTodo>
{
    public CreateTodoValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200);
    }
}
```

### 7.4 Unit Test (xUnit) — CreateTodoHandlerTests.cs

```csharp
public class CreateTodoHandlerTests
{
    [Fact]
    public async Task Handle_GivenValidCommand_ShouldAddAndReturnId()
    {
        // Arrange
        var repo = new Mock<IRepository<TodoItem>>();
        var uow  = new Mock<IUnitOfWork>();
        var sut  = new CreateTodoHandler(repo.Object, uow.Object);
        var cmd  = new CreateTodo("Write AGENTS.md examples");

        // Act
        var id = await sut.Handle(cmd, CancellationToken.None);

        // Assert
        repo.Verify(r => r.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()));
        uow.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()));
    }
}
```

This example compiles under C# 12, passes the validator automatically via the shared `ValidationBehavior`, and can be tested without any external dependencies.

---

## 8 Additional Worked Example — Projects Feature

A more sophisticated slice taken from the **SampleToDo** project demonstrates both a **command** (write) and a **query** (read) plus the use of a **query‑service abstraction**.

```
/UseCases/Projects/
  • Create/CreateProjectCommand.cs
  • Create/CreateProjectHandler.cs
  • ListShallow/ListProjectsShallowQuery.cs
  • ListShallow/ListProjectsShallowHandler.cs
  • ListShallow/IListProjectsShallowQueryService.cs   // abstraction implemented in Infrastructure
```

### 8.1 CreateProjectCommand.cs (Write)

```csharp
// Creates a new Project aggregate and returns its Id
public record CreateProjectCommand(string Name) : ICommand<int>;
```

### 8.2 CreateProjectHandler.cs

```csharp
public sealed class CreateProjectHandler(
    IRepository<Project> repository) : ICommandHandler<CreateProjectCommand, int>
{
    public async Task<int> Handle(CreateProjectCommand cmd, CancellationToken ct)
    {
        var project = new Project(ProjectName.From(cmd.Name));
        var created  = await repository.AddAsync(project, ct);
        return created.Id;
    }
}
```

*Highlights*: Uses **Value Objects** (`ProjectName`) from the domain and persists via `IRepository<Project>` — no EF Core types.\\

### 8.3 ListProjectsShallowQuery.cs (Read)

```csharp
// Optional paging parameters
public record ListProjectsShallowQuery(int? Skip, int? Take)
    : IQuery<Result<IReadOnlyList<ProjectDTO>>>;
```

### 8.4 ListProjectsShallowHandler.cs

```csharp
public sealed class ListProjectsShallowHandler(
    IListProjectsShallowQueryService query)
        : IQueryHandler<ListProjectsShallowQuery,
                        Result<IReadOnlyList<ProjectDTO>>>
{
    public async Task<Result<IReadOnlyList<ProjectDTO>>> Handle(
        ListProjectsShallowQuery request, CancellationToken ct)
    {
        var projects = await query.ListAsync(request.Skip, request.Take, ct);
        return Result.Success(projects);
    }
}
```

*Highlights*: Demonstrates a **read‑service abstraction** (`IListProjectsShallowQueryService`) instead of repository/specification to optimise list retrieval.

---

## 9 DTO Guidelines

### 9.1 What qualifies as a DTO?

> A **Data‑Transfer Object** is a *shape‑only* type used for moving data across boundaries. It *must not* contain domain logic or depend on outer layers.

| Kind                      | Purpose                                                           | Typical Location                                              |
| ------------------------- | ----------------------------------------------------------------- | ------------------------------------------------------------- |
| **Request DTO**           | *Commands / Queries* (already records implementing `IRequest<T>`) | Same file as the handler or action sub‑folder.                |
| **Response DTO**          | Data returned from handlers to the Web layer                      | `<Feature>/Dtos/` or alongside the query/command when unique. |
| **List / Projection DTO** | Lightweight projection used by queries for grids/lists            | `<Feature>/Dtos/` or under the specific query folder.         |

### 9.2 Design Rules

1. **Immutable** — prefer C# `record` types with `init;` setters.
2. **Flat structure** — avoid nesting aggregates; use IDs + simple properties.
3. **Serialization‑friendly** — only expose primitives, `DateTime`, `Guid`, etc.; no domain value objects.
4. **No behaviour** — DTOs carry data only; invariants live in domain entities/VOs.
5. **Naming** — suffix with `Dto` (or `DTO`) and keep feature context, e.g., `ProjectDto`, `TodoItemDto`.

### 9.3 Mapping Strategies

* **Commands** map *from* request DTO → domain entity inside the handler.
* **Queries** map domain entity → response DTO *before* returning (never return entities).
* Allowed mappers:

    * Manual mapping (clear and explicit for simple cases).
    * **Mapster** or **AutoMapper** configured in UseCases (interfaces only) with profiles defined inside `/UseCases/Mapping/`.
* Keep mapper extension methods in `static` classes under `Mapping` to avoid cluttering handlers.

### 9.4 Example — ProjectDto

```csharp
namespace NimblePros.SampleToDo.UseCases.Projects.Dtos;

public sealed record ProjectDto
{
    public int    Id          { get; init; }
    public string Name        { get; init; } = string.Empty;
    public int    ItemsCount  { get; init; }
    public DateTime CreatedOn { get; init; }
}

public static class ProjectMappings
{
    public static ProjectDto ToDto(this Project project) => new()
    {
        Id         = project.Id,
        Name       = project.Name.Value,
        ItemsCount = project.Items.Count,
        CreatedOn  = project.CreatedOn
    };
}
```

## 10 Query‑Service Pattern

> **When to prefer a Query Service?** Use it when a read query:
>
> * Needs joins or projections that don’t map cleanly to a repository/specification.
> * Reads from multiple aggregates (breaking aggregate boundaries is acceptable on read side).
> * Must use tools like Dapper or raw SQL for performance.
>
> The **interface lives in UseCases** so handlers depend on an abstraction; the **implementation lives in Infrastructure** where EF Core / SQL lives.

### 10.1 Interface Example — IListProjectsShallowQueryService.cs

```csharp
namespace NimblePros.SampleToDo.UseCases.Projects.ListShallow;

public interface IListProjectsShallowQueryService
{
    /// <summary>
    /// Returns a lightweight projection of projects for list/grid UIs.
    /// </summary>
    Task<IReadOnlyList<ProjectDto>> ListAsync(
        int? skip,
        int? take,
        CancellationToken ct = default);
}
```

*Rules*: return DTOs, be asynchronous, include `CancellationToken`.

### 10.2 Infrastructure Implementation Sketch

```csharp
internal sealed class ListProjectsShallowQueryService : IListProjectsShallowQueryService
{
    private readonly DbContext _db;

    public ListProjectsShallowQueryService(DbContext db) => _db = db;

    public async Task<IReadOnlyList<ProjectDto>> ListAsync(
        int? skip, int? take, CancellationToken ct)
    {
        var query = _db.Set<Project>()
                       .AsNoTracking()
                       .Select(p => new ProjectDto
                       {
                           Id         = p.Id,
                           Name       = p.Name.Value,
                           ItemsCount = p.Items.Count,
                           CreatedOn  = p.CreatedOn
                       })
                       .OrderBy(p => p.Name);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return await query.ToListAsync(ct);
    }
}
```

*Notes*: Implementation is **internal** to Infrastructure; it uses EF Core but only depends on the DTO defined in UseCases.

### 10.3 Registration

```csharp
services.AddScoped<IListProjectsShallowQueryService, ListProjectsShallowQueryService>();
```

Add this in **Infrastructure**’s DI setup so UseCases can inject the abstraction.

### 10.4 Unit Testing the Handler

Because the handler depends on the interface, unit tests can supply a **mock service** without hitting the database.

```csharp
var mock = new Mock<IListProjectsShallowQueryService>();
mock.Setup(m => m.ListAsync(null, null, It.IsAny<CancellationToken>()))
    .ReturnsAsync(Fixture.CreateMany<ProjectDto>(3).ToList());
```

---

*End of UseCases overrides*
