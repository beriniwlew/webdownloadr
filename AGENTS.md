---
title: AGENTS
project: WebDownloadr
language: "C# 12 (.NET 9)"
architecture: "Clean Architecture (Ardalis Template)"
layers:
  Core: "Domain: Entities, ValueObjects, Events, Interfaces (Ardalis.GuardClauses & Ardalis.Specification only)"
  UseCases: "Application: CQRS handlers, DTOs, validators (depends only on Core)"
  Infrastructure: "Persistence: EF Core context, external service adapters (depends on Core)"
  Web: "API: FastEndpoints HTTP host (depends on UseCases, Infrastructure, ServiceDefaults)"
  ServiceDefaults: "Cross-cutting: startup & telemetry for .NET Aspire / cloud"
  AspireHost: "Hosting: .NET Aspire entry point (preview in .NET 9)"
  Tests: "Unit, Integration, Functional, Aspire tests (mirror src structure)"
dependency_flow: "References flow inward (Web → UseCases, Infrastructure → Core; UseCases & Infrastructure never reference each other)"
ci_requirements:
  build_warnings: 0
  tests_pass: "100%"
  coverage_min: "≥ 90%"
  formatting_drift: 0
commit_convention: "Conventional Commits ([Layer] <type>: <summary>)"
agents_md_inheritance: "Global (~/.codex) → Repo root → Nested (deeper overrides parent)"
date_created: 2025-07-09T09:41:12+02:00
date_modified: 2025-07-09T11:17:25+02:00
---

# AGENTS.md
> **This file is a Binding for AI agents (e.g., [OpenAI Codex](https://platform.openai.com/docs/overview)) _and_ human contributors.**  
> Any contribution must comply with every rule herein, or the pull‑request will be rejected.  
> **Scope:** These rules apply to the entire repository. If an `AGENTS.md` file exists in a subdirectory, its instructions override or extend the global rules for files in that scope. In case of conflict, the more specific (nested) file’s guidance takes precedence.

---

## Table of Contents

- [Purpose](#purpose)
- [Nested AGENTS.md Inheritance & Layer-Specific Overrides](#nested-agentsmd-inheritance--layer-specific-overrides)
- [Solution & Project Layout](#solution--project-layout)
- [Agent Responsibilities](#agent-responsibilities)
- [DO NOT](#do-not)
- [Quality Gates](#quality-gates)
- [Runtime Environment](#runtime-environment)
- [Allowed Tools & APIs](#allowed-tools--apis)
- [Database & Migrations](#database--migrations)
- [Validation & Invariants](#validation--invariants)
- [Build, Test & Format](#build-test--format)
- [Code Formatting](#code-formatting)
- [Coding Standards](#coding-standards)
- [Testing Guidelines](#testing-guidelines)
- [Pull‑Request Guidelines](#pull-request-guidelines)
- [Environment & Secrets](#environment--secrets)
- [Architectural Notes](#architectural-notes)
- [Architecture Decision Records (ADR)](#architecture-decision-records-adr)
- [Additional Resources](#additional-resources)
- [Guidance for AI Agents](#guidance-for-ai-agents)
- [Prompt Engineering for Agents](#prompt-engineering-for-agents)
- [State and Context Awareness](#state-and-context-awareness)
- [Follow-Up: Further Enhancements](#follow-up-further-enhancements)

---

## Purpose

Codifies the operational rules for this repository so that **AI-powered agents (e.g., OpenAI Codex)** and human contributors can work safely within a **Clean Architecture** codebase based on the [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture) template while following modern .NET 9 best practices.

---

## Nested **AGENTS.md** Inheritance & Layer-Specific Overrides

> **Precedence** (lowest → highest):
> **Global** `~/.codex/AGENTS.md` → **Repo root** `/<repo>/AGENTS.md` → **Nested** `/<repo>/<folder>/AGENTS.md`
> A nested file automatically **inherits** every rule from its parent.
> If the nested file restates a rule, **the nested version wins** for that folder and its descendants.

| Layer / Folder                                                    | Scope of Nested File                          | Typical Overrides or Extensions                                                                                                                                                                     | Why Overrides Make Sense in Clean Architecture                                                                       |
| ----------------------------------------------------------------- | --------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------- |
| **Core** (`/src/<Solution>.Core`)                                 | Domain entities, value objects, domain events | *Tighten* rules: <br>• Forbid any package refs except `Ardalis.GuardClauses`, `Ardalis.Specification`.<br>• Raise line-coverage to ≥ 95 %.                                                          | Core must stay pure and fully tested; stricter quality gates prevent leakage of infrastructure concerns.             |
| **UseCases** (`/src/<Solution>.UseCases`)                         | CQRS handlers, DTOs, validators               | *Clarify* dependencies: may reference **Core** and MediatR/FluentValidation; **must not** reference **Infrastructure** or **Web**.<br>*Optionally* relax XML-doc requirement on trivial handlers.   | Ensures application logic mediates between UI and domain without coupling to infra or UI frameworks.                 |
| **Infrastructure** (`/src/<Solution>.Infrastructure`)             | EF Core context, external service adapters    | *Permit* external SDKs (e.g., EF Core, Azure).<br>*Lower* unit-test coverage if integration tests exist.<br>*Allow* broader exception handling patterns.                                            | Infrastructure is where external details live; pragmatic rules acknowledge harder unit-testing and external libs.    |
| **Web** (`/src/<Solution>.Web`)                                   | HTTP API / FastEndpoints host                 | *Enforce* that endpoints do not call **Infrastructure** directly (except in `Program.cs`).<br>*Allow* longer composition-root methods.<br>*Require* mapping DTOs—not domain entities—over the wire. | Keeps UI thin and decoupled; composition root is the single place allowed to wire infra implementations.             |
| **Tests** (`/tests/*`)                                            | Unit, Integration, Functional tests           | *Relax* style analyzers (e.g., allow underscores in test method names).<br>*Exclude* tests from coverage metrics.<br>*Require* new/changed code to ship with matching tests.                        | Test code values readability over production style; enforcing test presence is more important than stylistic purity. |
| **Migrations** (`/src/<Solution>.Infrastructure/Data/Migrations`) | EF Core-generated migration classes           | Disable analyzers & formatting drift checks.<br>Exclude from coverage.<br>“Do not hand-edit migrations—generate new ones instead.”                                                                  | Migrations are machine-generated history; linting & coverage add no value and manual edits break traceability.       |

### Authoring Guidelines for Nested Files

1. **State only deltas** – list *only* rules that differ from the repo-root file.
2. **Document exceptions** – if overriding a core architectural guard, add a brief “*Why*”.
3. **Keep under version control** – treat nested AGENTS.md edits like code; open a PR, run CI.
4. **Avoid duplication** – link back to the root file instead of copying unchanged rules.

Use these layer-specific nested files sparingly—only when a folder truly needs different rules to uphold Clean-Architecture boundaries while removing friction for contributors and AI agents.

## Solution & Project Layout

| Layer               | Folder                             | Brief description                                                                                                                                                                                                                          |
|---------------------|------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| **Core**            | `src/WebDownloadr.Core`            | Domain entities, value objects, domain events, and interfaces—**no external dependencies** except [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) & [`Ardalis.Specification`](https://github.com/ardalis/Specification). |
| **UseCases**        | `src/WebDownloadr.UseCases`        | CQRS command/query handlers, request/response DTOs, validators, and pipeline behaviors. Has a project reference only to **Core** and may use external packages but must not depend on **Infrastructure** or **Web**.                       |
| **Infrastructure**  | `src/WebDownloadr.Infrastructure`  | Contains EF Core `DbContext`, external service adapters, and persistence implementations. These align with Core interfaces and reside under the `Data/` folder.                                                                            |
| **Web**             | `src/WebDownloadr.Web`             | HTTP API using [FastEndpoints 6](https://fast-endpoints.com/docs/introduction); hosts application services. Depends on **UseCases**, **Infrastructure**, and **ServiceDefaults**.                                                          |
| **ServiceDefaults** | `src/WebDownloadr.ServiceDefaults` | Shared startup & telemetry helpers for [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) and cloud hosting.                                                                                                                  |
| **AspireHost**      | `src/WebDownloadr.AspireHost`      | Runs the Web project when using .NET Aspire (optional preview in .NET 9).                                                                                                                                                                  |
| **Tests**           | `tests/*`                          | `Unit`, `Integration`, `Functional`, and `Aspire` test projects mirroring the structure above. For every new or modified feature in `src/`, a corresponding test must be added or updated in `tests/`.                                     |

> **Dependency rule:** References must flow **inward**. For example, `Web` may reference **UseCases** and **Infrastructure**; both **UseCases** and **Infrastructure** may reference **Core** only. **UseCases** and **Infrastructure** must not reference each other. Dependency rules are reviewed manually. If architectural drift becomes an issue, consider enforcing it with tools like NetArchTest or ArchUnitNET.

| Layer          | May Reference                             |
|----------------|-------------------------------------------|
| Web            | UseCases, Infrastructure, ServiceDefaults |
| Infrastructure | Core                                      |
| UseCases       | Core                                      |
| Core           | —                                         |

---

# Clean Architecture Pattern Examples
## Pattern Reference & Advanced Examples
> **Purpose —** Provide reusable, proven code patterns that align with the **Ardalis Clean Architecture (.NET 9)** template.  
> **Scope —** AI agents & human contributors should model new code after these snippets unless a task explicitly requires a different approach.  
> **Layers —** All examples respect dependency flow (Web → UseCases → Core). Add or update tests and ADRs for any new pattern you introduce.

### 1. Validation Pipeline Behavior <small>(UseCases layer)</small>

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
````

> **DI registration**  
> `services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));`

### 2. Guard-Clause Invariants (Core layer)

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

**Custom extension example**

```csharp
namespace WebDownloadr.Core.Guards;

using Ardalis.GuardClauses;

public static class GuardExtensions
{
    public static void Negative(this IGuardClause guard, int value, string paramName) =>
        Guard.Against.NegativeOrZero(value, paramName, "Value must be positive.");
}
```

### 3. Raising a Domain Event (Core → UseCases)

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

> **Handling (UseCases layer)** – create a `ProjectCompletedHandler` implementing `INotificationHandler<ProjectCompletedEvent>` to publish emails, update search indexes, etc.

### 4. REPR DTO Pattern (Web layer)

```csharp
// Request  – validated via FluentValidation
public sealed record CreateProjectRequest(string Name, DateOnly StartDate);

// Response – thin & serializable
public sealed record CreateProjectResponse(int ProjectId);
```

_Guidelines_:

- Name with `<Verb><Entity>Request|Response>`.
    
- Keep DTOs **flat, immutable, validation-ready**.
    
- Never expose domain entities directly.
    

### 5. Query Without Repository (UseCases layer)

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

_Read operations may bypass EF repositories for performance._

### 6. Consistent Result Pattern (UseCases layer)

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

### 7. Structured Logging Behavior (UseCases layer)

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

_Register like `ValidationBehavior`; use **named placeholders** (`{Request}`) for structured logs._

### 8. Domain Service Example (Core layer)

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

_Centralize business logic spanning aggregates._

### 9. Sequence Diagram Reference

![Domain Event Sequence](https://user-images.githubusercontent.com/782127/75702680-216ce300-5c73-11ea-9187-ec656192ad3b.png)

### 10. Template Quick-Start

```bash
# Install template
dotnet new install Ardalis.CleanArchitecture.Template

# Create solution with Aspire support (.NET 9 preview)
dotnet new clean-arch -n WebDownloadr -as true
```

### 11. Local HTTPS Troubleshooting

```bash
dotnet dev-certs https --trust
```

_or import the dev cert from `~/.dotnet/corefx/cryptography/x509stores/my/`._

### 12. Opt-In MVC / Razor

```csharp
builder.Services.AddControllers();
app.MapControllers();
```

_Only add if UI requirements outgrow FastEndpoints._

> **Reminder for Contributors & AI Agents**
> 
> - Respect dependency flow (`Web → UseCases → Core`).
>     
> - Add or update tests for every new pattern.
>     
> - Create an ADR for any non-trivial architectural change.
>     
## Agent Responsibilities

1. **Commit** on the provided branch unless project maintainers instruct otherwise. When creating a new branch, prefix it with one of:
   - `feature/` – new functionality
   - `fix/` – bug fixes
   - `chore/` – maintenance or tooling
   - `docs/` – documentation updates
   Example: `feature/add-download-endpoint`
2. **Run** `./scripts/selfcheck.sh` locally. It **must** exit with `0` (fix any issues until it does).
3. **Commit** messages use `[Layer] <type>: <summary>` where `<type>` is one of `feat`, `fix`, `chore`, or `docs`.
   Examples:
   - `[UseCases] feat: add download queue processor`
   - `[Web] fix: return correct status codes`
   - `[Infrastructure] chore: update EF Core version`
   - `[Docs] docs: clarify setup instructions`
4. **Push** and open a pull request.
5. Ensure **CI is green** (all the same checks as `selfcheck.sh` must pass in CI).

### Typical Task Locations

| Scenario                                 | Destination folder / pattern                                                                                      |
|------------------------------------------|-------------------------------------------------------------------------------------------------------------------|
| Add domain entity or value object        | `src/WebDownloadr.Core/<Aggregate>/<Entity>.cs`                                                                   |
| Add repository interface / specification | `src/WebDownloadr.Core/Interfaces`                                                                                |
| Implement repository & EF mapping        | `src/WebDownloadr.Infrastructure/Data`                                                                            |
| Add/update CQRS command or query handler | `src/WebDownloadr.UseCases/Commands/<Feature>/`                                                                   |
| Write unit test                          | `tests/WebDownloadr.UnitTests/<Feature>Tests.cs`                                                                  |
| Expose REST endpoint                     | `src/WebDownloadr.Web/Modules/<Feature>/`                                                                         |
| Write integration or functional test     | `tests/WebDownloadr.IntegrationTests/<Feature>Tests.cs` or `tests/WebDownloadr.FunctionalTests/<Feature>Tests.cs` |
| Add domain event handler                 | `src/WebDownloadr.Core/<Aggregate>/Handlers/<EventHandler>.cs`                                                    |

- **Naming tip:** Prefer naming handler files as `XCommandHandler.cs` or `XQueryHandler.cs` to clearly indicate their purpose.

---

## DO NOT

- **Do not** change files in `docs/architecture-decisions/` unless the change _is_ an ADR.

- **Do not** add references from **Web** or **Infrastructure** back to **Core** (violates dependency flow).

- **Do not** commit secrets or credentials; use environment variables or user-secrets instead.

- **Do not** lower test coverage or disable analyzers.

- **Do not** add MVC Controllers or Razor Pages to the **Web** project – use FastEndpoints or ApiEndpoints instead.

- **Do not** modify generated migration files unless explicitly instructed.

- **Do not** edit or reformat code solely for whitespace or style, unless addressing a formatter/analyzer warning.

---

## Quality Gates

| Check                    | Requirement                                                                                     |
|--------------------------|-------------------------------------------------------------------------------------------------|
| Build warnings           | **0** (none)                                                                                    |
| Unit & integration tests | **100%** pass                                                                                   |
| Line coverage            | **≥ 90%** (via [Coverlet](https://github.com/coverlet-coverage/coverlet)) (not implemented yet) |
| Formatter drift          | **0** files (`dotnet format --verify-no-changes`)                                               |

The pull‑request will be blocked if any gate fails. Continuous Integration runs via GitHub Actions. The README **must** display a CI status badge for the `.github/workflows/ci.yml` workflow to ensure build visibility. Branch protection on `main` requires all these checks to pass before merging.

---

## Runtime Environment

- **.NET SDK 9.0.301** must be on the `PATH` (see `global.json`).

- The reference build environment is Ubuntu 22.04 (Docker image `mcr.microsoft.com/dotnet/sdk:9.0`).

- Run `./scripts/setup-codex.sh` to ensure the SDK and required global tools are installed. Sourcing this script sets `DOTNET_ROOT` and updates the `PATH` for the current shell, persisting them in `~/.bashrc`. It invokes `setup-dotnet.sh` and `install-tools.sh` under the hood. If new tools are added, update those scripts and document their use here or in `CONTRIBUTING.md`.

## Allowed Tools & APIs

- **`scripts/setup-codex.sh`** – Run once when setting up a development environment. Installs the .NET SDK and required global tools, exporting `DOTNET_ROOT` and updating `PATH`.
- **`scripts/selfcheck.sh`** – Mirrors the CI pipeline. Builds with warnings as errors, runs tests, and verifies formatting. Execute before pushing commits.
- **`dotnet format`** – Ensures code complies with `.editorconfig` and analyzer rules. Used in CI and by `selfcheck.sh`; run manually via `scripts/format.sh` to check formatting only.
- **`scripts/bootstrap-format.sh`** – Optional helper to normalize whitespace and analyzer fixes across the repository. Useful after cloning or when analyzer packages change.
- **`dotnet-outdated`** – Global tool (installed through `install-tools.sh`) for auditing NuGet dependencies. Run periodically to spot upgrades.
- **`ReportGenerator`** – Produces HTML code coverage reports from Coverlet output. Triggered in `selfcheck.sh` after tests complete.
- **`NetArchTest`** – Optional library for testing Clean Architecture boundaries. Add to test projects if you want automated dependency assertions.
- Before introducing any unlisted tool or API, confirm with the maintainers.

---

## Database & Migrations

### 1. Create / Update Migration

```bash
# From the Web project directory

dotnet ef migrations add <MIGRATION_NAME> \
  -c AppDbContext \
  -p ../WebDownloadr.Infrastructure/WebDownloadr.Infrastructure.csproj \
  -s WebDownloadr.Web.csproj \
  -o Data/Migrations
```

> **Rule:** All migrations live under `src/WebDownloadr.Infrastructure/Data/Migrations` and use the `AppDbContext`. Do not manually edit generated migration files.

### 2. Apply Migration (optional)

The Web project seeds the database via `EnsureCreated()`; pending migrations are not applied automatically. Run the command below whenever you need to update the schema:

```bash
dotnet ef database update -c AppDbContext \
  -p ../WebDownloadr.Infrastructure/WebDownloadr.Infrastructure.csproj \
  -s WebDownloadr.Web.csproj
```

---

## Validation & Invariants

- **Input validation** occurs in two layers:

    1. **Web endpoints** – validate incoming requests via FluentValidation or FastEndpoints’ built‑in validators.

    2. **UseCases handlers** – re-validate commands/queries via pipeline behaviors (to guard against bypassing Web validation).

- The **domain model assumes pre-validated inputs**. It uses [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) and exceptions to enforce invariants. Domain entities should throw exceptions or raise domain events when invariants are violated; they must not try to automatically correct or coerce invalid data.

- Follow the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern) for request/response DTOs (make them small, immutable records with clear validation rules).

---

## Build, Test & Format

The exact commands executed by CI are encapsulated in **`scripts/selfcheck.sh`**:

```bash
#!/usr/bin/env bash
set -euo pipefail

dotnet restore WebDownloadr.sln

dotnet build --no-restore -warnaserror

dotnet test --no-build --no-restore WebDownloadr.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults

dotnet format --verify-no-changes WebDownloadr.sln --no-restore

reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/coverage-report" -reporttypes:HtmlSummary
```

To enforce the 90% coverage threshold in CI, you can add a step that fails when the coverage summary XML shows less than 90% line coverage:

```yaml
- name: Verify Code Coverage Threshold
  run: |
    COVERAGE=$(grep -Po 'line-rate="\K[0-9.]+(?=\")' TestResults/coverage-report/coverage.xml | head -1)
    PERCENT=$(awk "BEGIN {print $COVERAGE * 100}")
    echo "Line coverage: $PERCENT%"
    awk -v p=$PERCENT 'BEGIN {exit (p<90)}'
```

Run this script locally **before pushing a branch or opening a PR**. Any non-zero exit code must abort the change. PRs with failing checks will be auto-closed by CI.

---

## Code Formatting

1. **`.editorconfig` is canonical** – the project enforces specific formatting settings:

    - `indent_style = space`

    - `*.{cs,csx,vb,vbx}` → **2-space indentation**

    - `*.{csproj,vbproj,vcxproj,proj,props,targets}` → **2-space indentation**

    - `end_of_line = crlf`

    - `charset = utf-8-bom`

    - `trim_trailing_whitespace = true`

    - `insert_final_newline = true`

2. **CI Enforcement** – Formatting is enforced via `dotnet format --verify-no-changes`. Run `./scripts/format.sh` or `dotnet format` locally before committing. Also set `git config --global core.autocrlf true` to avoid line-ending issues.

    - A one-time line-ending normalization may be required if inconsistencies exist:

        ```bash
        git add --renormalize .
        git commit -m "style: normalize line endings to match .editorconfig"
        ```

    - Formatting violations will block PRs (treat warnings from analyzers as errors).

---

## Coding Standards

- **Language & Target** – Use modern C# 12 syntax, targeting `net9.0`.

- **Nullable** (nullable reference types) and **ImplicitUsings** – enabled.

- **Analyzers** – Enabled and treated as errors: [Microsoft.CodeAnalysis.NetAnalyzers](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview) and [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers).

- **Formatting & Naming** – Governed by `.editorconfig` (enforced via `dotnet format` as above).

- Prefer **file‑scoped namespaces** and **top‑level statements** (e.g., in `Program.cs`).

- Use domain guard clauses (e.g., [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses)) to enforce invariants, and raise domain events when business rules are triggered.

- Provide XML documentation for public Core APIs and any complex methods.

- Ensure all public methods in Core have corresponding unit tests if they contain business logic.

---

## Testing Guidelines

- **Test Framework** – All test projects **must** use [xUnit](https://xunit.net/), [Shouldly](https://shouldly.readthedocs.io/en/latest/), and [NSubstitute](https://nsubstitute.github.io/index.html).
- Place tests in the matching test project: `WebDownloadr.UnitTests`, `WebDownloadr.IntegrationTests`, `WebDownloadr.FunctionalTests`, `WebDownloadr.AspireTests` (mirroring the structure of the `src/` projects).
- Maintain test isolation. Integration tests may use [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet) for external dependencies (e.g., databases), and use [HttpClientTestExtensions](https://github.com/ardalis/HttpClientTestExtensions) for concise HTTP response assertions in functional tests.
- **Coverage validation** – Run `dotnet test --collect:"XPlat Code Coverage"` for all test projects and invoke `reportgenerator -reporttypes:HtmlSummary` on the results. Line coverage must remain **>= 90%**.
- **Agent-generated tests** – When an AI agent adds or modifies tests, verify they compile and pass. Include the resulting `HtmlSummary` report (or a screenshot) in the PR description to demonstrate coverage.

---

## Pull‑Request Guidelines

- **Title** – Format as `[Layer] Short description of change` (for example, `[UseCases] Add CreateOrder command`).

- **Description** – Clearly explain the changes, list key file modifications, and include test results or screenshots if applicable.

- Reference any issue that is addressed (e.g., `Fixes #123` or `Closes #123` in the description).

- Link related issues or discussions. Use GitHub keywords to automatically close issues if appropriate.

- All quality gates must pass before review. (For documentation-only or configuration-only changes, new tests may not be needed as long as existing checks remain green.)

---

## Environment & Secrets

Never commit secrets or sensitive configuration values to the repository. For local development, use tools like [ASP.NET Core User Secrets](https://learn.microsoft.com/dotnet/core/extensions/user-secrets) or environment variables (local `.env` files are git-ignored by default). If certain environment variables or secrets are required for CI or integration tests, configure them in the CI pipeline (for example, via GitHub Actions secrets) rather than checking them into source control.

---

## Architectural Notes

- Dependencies must flow **inward** (from outer layers into Core).

- Always place new code in the correct layer according to its role.

- Any public API in **Core** should have corresponding tests and (if its behavior is not obvious) documentation comments.

- Generated files (e.g., EF Core migration classes) should remain unmodified unless a specific manual change is needed (and documented).

- If multiple bounded contexts emerge over time, consider creating an internal shared-kernel library. Until then, this solution uses the external `Ardalis.SharedKernel` package for any shared domain types.

---

## Architecture Decision Records (ADR)

Architecture Decision Records document important decisions and the reasoning behind them.

All ADR files reside under `docs/architecture-decisions/` and follow the [ardalis/CleanArchitecture ADR template](https://github.com/ardalis/CleanArchitecture/tree/main/docs/architecture-decisions).

- **When to write** – Create an ADR whenever the team is faced with a significant decision (especially if multiple approaches were considered or an old decision is being revisited).

- **Format** – Markdown file with an incrementing four-digit prefix and short descriptive slug (e.g., `0001-use-fastendpoints.md`). Include sections: **Status**, **Context**, **Decision**, **Consequences**, and **References**. List any alternatives considered and why they were not chosen.

- **Numbering** – Use the next available number in sequence, and keep the slug concise.

- **Author** – Whoever proposes the change (human or AI agent) should draft the ADR.

- **Review** – Submit the ADR as a pull request. Reviewers will check numbering and clarity before approval.

- **Immutability** – Once an ADR is merged, it becomes a historical record. Do not modify an ADR retroactively; if a decision changes, create a new ADR that supersedes the old one.

- **Tools** – You may use utilities like [adr-tools](https://github.com/npryce/adr-tools) to generate ADR templates, though this is optional.

---

## Additional Resources

- [Markdown Architecture Decision Records](https://adr.github.io/madr/)

- [Getting Started with Architecture Decision Records](https://ardalis.com/getting-started-with-architecture-decision-records/)

- [CONTRIBUTING.md]

- [CODE_OF_CONDUCT.md]

- [ardalis/CleanArchitecture Repository](https://github.com/ardalis/CleanArchitecture)

- [.NET Official Docs](https://learn.microsoft.com/dotnet/)

- [Ardalis Guard Clauses](https://github.com/ardalis/GuardClauses)

- [Ardalis Specification](https://github.com/ardalis/Specification)

- [FastEndpoints Documentation](https://fast-endpoints.com/docs/introduction)

- [.NET Aspire (Preview)](https://learn.microsoft.com/en-us/dotnet/aspire/)

- [xUnit.net – Getting Started](https://xunit.net/)

- [Shouldly Assertions](https://shouldly.readthedocs.io/en/latest/)

- [NSubstitute](https://nsubstitute.github.io/index.html)

- [Coverlet Code Coverage](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/GlobalTool.md)

- [ReportGenerator](https://danielpalme.github.io/ReportGenerator/)

- [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet)

- [NetArchTest](https://github.com/BenMorris/NetArchTest)

- [HttpClientTestExtensions](https://github.com/ardalis/HttpClientTestExtensions)

- [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated)

- [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)

- [Microsoft Code Analysis](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview)

- [Entity Framework Core Docs](https://learn.microsoft.com/ef/core/)

- [.NET 9 SDK – What’s New](https://learn.microsoft.com/aspnet/core/release-notes/aspnetcore-9.0?view=aspnetcore-9.0)

---
## Output Schemas

Agent generated responses may be consumed by other tools and therefore must
adhere to strict JSON or YAML schemas. Every defined property is required and no
additional fields are permitted.

### Example

```json
{
  "status": "success",
  "tests": {
    "total": 42,
    "passed": 42,
    "failed": 0
  }
}
```

The `status` value communicates success or failure while the `tests` object
captures overall results. Use the same structure when emitting YAML:

```yaml
status: success
tests:
  total: 42
  passed: 42
  failed: 0
```

Responses **must match these schemas exactly**—omit any extraneous properties.


## Guidance for AI Agents

- If unsure about a part of the codebase, prefer **not to change** a file unless a clear rule or failing test specifically requires it.

- If multiple valid approaches exist, choose the **simplest and most idiomatic** .NET solution.

- Always output **minimal diffs** so commits stay focused and avoid unrelated changes. Widen your changes only to fix formatting errors or analyzer warnings, or if the instructions explicitly request a broader update.

---

## Prompt Engineering for Agents

Use a structured process when generating changes.

### Step-by-Step Reasoning
1. Read the request and identify the target layer or file.
2. Outline the minimal steps needed to implement the change.
3. Execute each step, validating results along the way.
4. Ensure updates respect Clean Architecture boundaries.
5. Summarize the outcome in the PR description.

### Clarifying Questions
When tasks are ambiguous, consider asking:
- "Which project or layer should this affect?"
- "What scenario or user story drives the change?"
- "Are existing tests available to guide the update?"

### Workflow Checklist
- [ ] Run `./scripts/selfcheck.sh` and confirm all checks pass.
- [ ] Verify no secrets or credentials were introduced.
- [ ] Format code via `dotnet format --verify-no-changes`.
- [ ] Add or update tests for new behavior.
- [ ] Commit using `[Layer] <type>: <summary>`.

## State and Context Awareness

- Maintain continuity across related tasks by referencing the same **UseCase**,
  **Domain aggregate**, or other shared concept when editing multiple files.
- Keep track of decisions made earlier in the pull request so subsequent changes
  remain consistent with that context.
- Long-term state is **not persisted** between sessions unless explicitly
  instructed, so each PR should include enough details or references for future
  contributors to understand the rationale.

---

## Follow-Up: Further Enhancements

Even with the above guidelines, there are opportunities to further improve or extend the development workflow and AI integration:

- **Automate Architecture Enforcement:** Implement tools like ArchUnitNET (as referenced above) in the CI pipeline to automatically enforce the layer dependency rules. This will catch any forbidden project references or architectural drift during pull request checks.

- **Leverage Nested `AGENTS.md` Files:** If the repository grows into multiple distinct areas or modules (for example, adding a frontend or additional services), consider adding localized `AGENTS.md` files in those subdirectories. The AI agent will automatically apply those context-specific instructions (overriding the root rules) when working in that scope.

- **Include Code Examples for Patterns:** Augment this guide with small code snippets demonstrating important patterns or conventions (e.g., how to raise a domain event, use a guard clause, or format a log message). These examples can help AI agents follow established practices more closely when generating new code.

- **Keep Guidelines Up-to-Date:** Periodically revise this document to reflect evolving best practices and tooling. For instance, when .NET 10 is released or if [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/) moves out of preview, update the relevant sections. Similarly, incorporate any new analyzers or frameworks that become standard so the AI is always guided by current conventions.

- **Expand CI Quality Checks:** Consider extending the CI workflow with additional checks. For example, enforce the 90% code coverage threshold by integrating the provided script into the CI configuration (if not already done). Additionally, you could use tools like `dotnet-outdated` to detect stale dependencies or add security/static analysis scanners. Such enhancements will ensure that both human and AI contributions continue to meet the project’s high standards.

## Performance & Safety Controls

- **Default to safe limits.** When mocking external services is unavailable, restrict outbound API calls to the bare minimum, ideally none.
- **Monitor resource usage.** If tasks require excessive CPU, memory, or I/O, reduce workload or halt execution.
- **Escalate when uncertain.** When limits are exceeded or instructions remain ambiguous, pause work and request human guidance before proceeding.

---

## Fallbacks and Escalation

Agents must halt and request clarification whenever a repository rule or instruction is unclear or appears conflicting.

* **Stop** – If you cannot interpret a domain rule or architectural guideline with confidence, pause implementation.
* **Ask** – Seek guidance from project maintainers via a comment or PR description when uncertain about next steps.
* **Defer** – If a decision risks violating the codebase's principles, escalate to human review before proceeding.

Err on the side of caution and avoid speculative changes. Explicit confirmation from maintainers resolves any ambiguity.

