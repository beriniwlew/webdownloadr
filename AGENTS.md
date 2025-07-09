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
dependency_flow: "References flow inward. Core → none; UseCases → Core; Infrastructure → Core; Web → UseCases (+ Infrastructure only in Program/Startup for DI)."
ci_requirements:
  build_warnings: 0
  tests_pass: "100%"
  coverage_min: "≥ 90%"
  formatting_drift: 0
commit_convention: "Conventional Commits ([Layer] <type>: <summary>)"
branch_prefixes:
  - feature/
  - fix/
  - chore/
  - docs/
agents_md_inheritance: "Global (~/.codex) → Repo root → Nested (deeper overrides parent)"
date_created: 2025-07-09T09:41:12+02:00
date_modified: 2025-07-09T13:50:00+02:00
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
- [Output Schemas](#output-schemas)
- [Prompt Engineering for AI Agents](#prompt-engineering-for-ai-agents)
- [Runtime Environment](#runtime-environment)
- [Allowed Tools & APIs](#allowed-tools--apis)
- [Database & Migrations](#database--migrations)
- [Validation & Invariants](#validation--invariants)
- [Build, Test & Format](#build-test--format)
- [Code Formatting](#code-formatting)
- [Coding Standards](#coding-standards)
- [Code Patterns (Ready-to-Copy Examples)](#code-patterns-ready-to-copy-examples)
- [Testing Guidelines](#testing-guidelines)
- [Pull‑Request Guidelines](#pull-request-guidelines)
- [Environment & Secrets](#environment--secrets)
- [Architectural Notes](#architectural-notes)
- [Architecture Decision Records (ADR)](#architecture-decision-records-adr)
- [Example Repositories & Further Reading](#example-repositories--further-reading)
- [Guidance for AI Agents](#guidance-for-ai-agents)
- [Prompt Engineering for Agents](#prompt-engineering-for-agents)
- [State and Context Awareness](#state-and-context-awareness)
- [Follow-Up: Further Enhancements](#follow-up-further-enhancements)
- [Performance & Safety Controls](#performance--safety-controls)
- [Fallbacks & Escalation](#fallbacks--escalation)

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
|-------------------------------------------------------------------|-----------------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------|
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
| **Web**             | `src/WebDownloadr.Web`             | HTTP API using [FastEndpoints 6](https://fast-endpoints.com/docs/introduction); hosts application services. Depends on **UseCases**, **Infrastructure**, and **ServiceDefaults**. Except for dependency-injection wiring in `Program.cs`, Web code must call Infrastructure **only via interfaces or UseCase handlers**. |
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

1. **Commit** on the provided branch unless project maintainers instruct otherwise. New branches must start with one of these prefixes:
   - `feature/` – new functionality
   - `fix/` – bug fixes
   - `chore/` – maintenance or tooling
   - `docs/` – documentation updates
   Example: `feature/add-download-endpoint`
2. **Run** `./scripts/selfcheck.sh` locally. It **must** exit with `0` (fix any issues until it does).
3. **Commit** messages must follow the format `[Layer] <type>: <summary>`.
   Allowed `<type>` values:
   - `feat`  – new features
   - `fix`   – bug fixes
   - `chore` – maintenance
   - `docs`  – documentation
   Example commits:
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

- Do **NOT** add references **from Core to any other project**.
- Do **NOT** add references **from UseCases to Infrastructure or Web**.

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

### Architecture Enforcement (CI)
- The `Project.ArchitectureTests` project verifies layer rules using **NetArchTest.Rules**.
- CI step `dotnet test ./tests/Project.ArchitectureTests` must pass.
- Rules:
  * Core **must not** depend on any other project.
  * UseCases **must not** depend on Infrastructure or Web.
  * Infrastructure **must not** depend on Web.
  * Web may reference Infrastructure **only** in the `Program` (composition-root) namespace.
- If a new project is added, extend `DependencyRulesTests` to include it before merging.
- Also check for circular dependencies (e.g., `Types().That().ResideInProject("Core").ShouldNotHaveCircularDependencies()`).

The pull‑request will be blocked if any gate fails. Continuous Integration runs via GitHub Actions. The README **must** display a CI status badge for the `.github/workflows/ci.yml` workflow to ensure build visibility. Branch protection on `main` requires all these checks to pass before merging.

---

## Performance & Safety Controls

The guidelines below protect CI budgets, API quotas, and human reviewers from runaway tasks. **All contributors—human *and* AI—must honour these limits.**

| Area                        | Hard Limit                                  | Agent Rule                                                                                    |
| --------------------------- | ------------------------------------------- | --------------------------------------------------------------------------------------------- |
| **External HTTP/CLI calls** | **Max 3** per workflow run                  | Batch fetches when possible. If a 4th call is required, **stop and request human approval**.  |
| **Call timeout**            | **30 s** per request                        | Abort and escalate if an endpoint exceeds the timeout twice.                                  |
| **Download size**           | **10 MB** per individual fetch              | Skip large assets; log a TODO for manual retrieval.                                           |
| **Runtime budget**          | **10 min** total wall-clock per CI job      | Ensure long-running scripts (e.g., `dotnet test`) stream progress; abort if the limit is hit. |
| **Memory footprint**        | **512 MB** for any spawned process          | Use streaming / chunked parsing; avoid loading entire datasets in RAM.                        |
| **Token usage (AI calls)**  | **≤ 30 k tokens** per PR iteration          | Summarise context before invoking LLMs; chunk analysis.                                       |
| **Retry logic**             | Max **2** retries with exponential back-off | Log failures; do **not** loop indefinitely.                                                   |
| **Cost ceiling**            | Free-tier / zero-cost services only         | If a paid resource is required, escalate to a human owner.                                    |
| **Secrets & keys**          | Must come from CI secrets manager           | Abort if a secret is missing; never prompt for interactive entry.                             |

#### Escalate to Humans When…

1. A required API or service is **unreachable** after 2 retries.
2. You encounter an **ambiguous domain model change** (e.g., conflicting migration history).
3. Test failures persist after a single automated fix attempt.
4. Implementation would break any “Hard Limit” above.

> **AI agents:** Emit a clear JSON status block—`{"status":"escalate","reason":"<short text>"}`—when invoking an escalation. Humans will review and unblock you.

#### Logging & Metrics

* Use **structured logging** (`ILogger` placeholders) to record:
  `CallCount`, `ElapsedMs`, `BytesDownloaded`, `Retries`, `TokenCost`.
* CI surfaces resource-usage summaries in job artifacts; PRs failing limits are auto-closed.

---

## Output Schemas

> **Why?** Explicit JSON / YAML contracts let CI pipelines and custom tooling parse AI-generated artifacts automatically—no guesswork, no brittle regex.

### 1. Task Status Report (JSON)

When an AI agent finishes a multi-step task (e.g., code change + tests), it **must output** a final JSON block that matches this schema:

```jsonc
{
  "status": "success | failure",      // REQUIRED: overall result
  "message": "Short human summary",   // REQUIRED: 1-2 sentence outcome
  "filesChanged": [                   // OPTIONAL: paths relative to repo root
    "src/WebDownloadr.Core/NewEntity.cs",
    "tests/WebDownloadr.UnitTests/NewEntityTests.cs"
  ],
  "testsPassed": 123,                 // OPTIONAL: integer
  "testsFailed": 0,                   // OPTIONAL: integer
  "coverage": 92.4                    // OPTIONAL: line-coverage percent
}
```

*Agents*: **Output only the JSON**—no prose above or below the fenced block.

### 2. Test Execution Summary (YAML)

Long CI logs are noisy. For quick dashboards, agents executing `dotnet test` should emit a YAML summary:

```yaml
status: success          # success | failure
total_tests: 120
passed: 120
failed: 0
skipped: 0
coverage: 92.4           # percent (float)
report_path: TestResults/coverage-report/index.html
```

### 3. Code Generation List (JSON)

If the agent generates multiple new files (scaffolding, ADR drafts), list them in this minimal schema:

```json
{
  "generatedFiles": [
    { "path": "docs/architecture-decisions/0008-new-choice.md", "type": "adr" },
    { "path": "src/WebDownloadr.UseCases/Commands/Foo/CreateFooCommand.cs", "type": "code" }
  ]
}
```

### Validation Rules

1. **No extra keys** — unknown properties break the contract.
2. **Exact casing** — field names are case-sensitive.
3. **Single block** — agents should not stream partial objects; emit once when done.
4. **JSON mode preferred** — unless YAML is explicitly requested (e.g., for test summary).

> CI will fail any PR whose agent output is invalid JSON/YAML or missing required fields.

---

*Following these schemas ensures humans, AI agents, and automated checks all speak the same, machine-parsable language when reporting results.*

---

## Prompt Engineering for AI Agents

> These rules help AI contributors deliver high-quality, low-friction output.
> Humans may ignore—AI **must** comply.

### 1. Reasoning Checklist  
Before returning any result, an AI agent **MUST** internally step through:

1. **Understand the ask**  
   *Paraphrase the task in your head; identify target layer & file.*
2. **Locate context**  
   *Open relevant code / docs (start with AGENTS.md + README).*
3. **Ask clarifying questions** (if still <80 % certain).  
4. **Plan step-by-step**  
   *List sub-steps or functions you’ll touch; ensure no layer violation.*  
5. **Generate minimal diff**  
   *Change only what the task requires.*  
6. **Self-check** (`./scripts/selfcheck.sh`)  
   *Green checks locally before proposing PR.*  
7. **Explain** (in PR description)  
   *Summarise reasoning in ≤ 5 sentences; cite ADRs or rules followed.*

### 2. Clarifying-Question Guide  
Ask **exactly one** concise question when:

| Situation | Example Question |
|-----------|------------------|
| Ambiguous requirement | “Which HTTP status codes should this endpoint return on validation failure?” |
| Missing domain rule | “Is negative quantity ever valid in `OrderLine`?” |
| Conflicting source | “Core forbids EF types, yet `Product` entity has `[Key]`; should I remove?” |

If no answer after two attempts ⇒ **escalate to human reviewer**.

### 3. Example Prompts  

| Scenario | Prompt |
|----------|--------|
| **Bug fix, known layer** | *“Fix the null-ref in `ContributorListQueryHandler.cs` (UseCases layer) without adding new deps. Ensure unit tests pass.”* |
| **Add ADR** | *“Draft an ADR proposing PostgreSQL instead of SQLite. Follow ADR template; Status: Proposed.”* |
| **Refactor with diff** | *“Refactor `src/WebDownloadr.Core/Project.cs` to raise `ProjectCompletedEvent` when all tasks done. Show minimal diff only.”* |

### 4. Forbidden Prompt Styles  
* ❌ Open-ended “rewrite everything” requests  
* ❌ Mass formatting or lint-only changes  
* ❌ Skipping self-check

> Agents deviating from this guidance will have their PR auto-closed by CI.

---

## Runtime Environment

- **.NET SDK 9.0.301** must be on the `PATH` (see `global.json`).

- The reference build environment is Ubuntu 22.04 (Docker image `mcr.microsoft.com/dotnet/sdk:9.0`).

- Run `./scripts/setup-codex.sh` to ensure the SDK and required global tools are installed. Sourcing this script sets `DOTNET_ROOT` and updates the `PATH` for the current shell, persisting them in `~/.bashrc`. It invokes `setup-dotnet.sh` and `install-tools.sh` under the hood. If new tools are added, update those scripts and document their use here or in `CONTRIBUTING.md`.


## Allowed Tools & APIs

| Tool / Package                                                  | Layer(s) Where Used            | Purpose & Typical Invocation                                                                                                               | Notes / Links                                                                          |
| --------------------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------------------------------------------------------------------------ | -------------------------------------------------------------------------------------- |
| **dotnet CLI** (`dotnet build`, `dotnet test`, `dotnet format`) | All                            | Core build, test, and formatter commands (see `scripts/selfcheck.sh`).                                                                     | Version pinned by `global.json` (❱ **9.0.301**).                                       |
| **dotnet-ef**                                                   | *Infrastructure*, *Migrations* | Add / apply Entity Framework Core migrations.<br>`bash dotnet ef migrations add Foo --project ...`                                         | Use only through `AppDbContext`; migrations live in `src/.../Data/Migrations/`.        |
| **reportgenerator** (`dotnet-reportgenerator-globaltool`)       | CI & local                     | Convert Coverlet `.cobertura.xml` into HTML summary.                                                                                       | Installed via `install-tools.sh`; output written to `TestResults/coverage-report/`.    |
| **ArchUnitNET CLI** (`archunitnet-cli`)                         | CI & local                     | Enforce layer dependency rules. Run in `Verify Architecture` step or locally:<br>`bash dotnet archunitnet-cli --solution WebDownloadr.sln` | Fails CI if Web → Core, Infrastructure ↔ UseCases, etc.                                |
| **dotnet-outdated**                                             | Any                            | Detect NuGet packages with newer versions:<br>`bash dotnet outdated --include-transitive`                                                  | PRs may include a dependency-bump commit but **must not** auto-upgrade without review. |
| **TestContainers for .NET** (`testcontainers-dotnet`)           | *IntegrationTests*             | Spin up throw-away Docker services for DB or message broker tests. Used via code—not CLI.                                                  | Agents **should not** call Docker directly; rely on testcontainers API in tests.       |
| **adr-tools / dotnet-adr**                                      | Docs                           | Scaffold new `docs/architecture-decisions/00NN-*.md` files and update index.                                                               | Required when an ADR is part of the task.                                              |
| **FastEndpoints CLI** (`fastendpoints`) – *optional*            | Web                            | Generate Endpoint skeletons:<br>`bash fastendpoints new Customer/GetById`                                                                  | Use only if adding new API endpoints; follow folder conventions.                       |
| **GitHub CLI** (`gh`)                                           | Local automation               | Create PRs, manage secrets:<br>`bash gh pr create`                                                                                         | Optional convenience; repo access tokens must be in env vars or GitHub CLI keychain.   |
| **OpenAI / Azure OpenAI APIs**                                  | *Agents / Scripts*             | Allowed only within AI-assist tooling or spikes. **Do not** embed keys in repo—use secrets.                                                | New agent scripts invoking LLMs require an ADR + security review.                      |

### Rules of Engagement

1. **Use only tools in this table**. Proposing a new CLI or service ⇒ open an ADR.
2. Invoke CLIs via existing shell scripts (`./scripts/*.sh`) where possible.
   AI agents: call the script instead of duplicating logic.
3. Global tools are installed by `install-tools.sh` and cached in CI.
   *Never* `sudo apt-get` a global package inside workflow YAML without discussion.
4. External APIs **must** read credentials from environment variables or GitHub Actions Secrets.
   Leaking keys is a hard-fail commit.

> **AI reminder:** If your task needs a tool not listed here, pause and create an ADR proposing its adoption.

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

## Code Patterns (Ready-to-Copy Examples)

> These snippets illustrate **idiomatic Clean Architecture** techniques for each layer.
> AI agents should mimic these styles when generating new code.

---

### 1  Core – Domain Event + Guard Clause

```csharp
namespace WebDownloadr.Core.DownloadAggregate;

public sealed class DownloadRequest : EntityBase, IAggregateRoot
{
    public Uri TargetUrl { get; private set; }

    // EF Core constructor
    private DownloadRequest() { }

    public DownloadRequest(string url)
    {
        // Fail-fast input validation
        TargetUrl = Guard.Against
            .NullOrWhiteSpace(url, nameof(url))
            .Pipe(u => new Uri(u, UriKind.Absolute));

        // Emit domain event for downstream handlers
        RegisterDomainEvent(new DownloadRequestedEvent(this));
    }

    public void MarkSucceeded()
        => RegisterDomainEvent(new DownloadSucceededEvent(this));
}
```

*Key points*

* No external dependencies except **Ardalis.GuardClauses**.
* Entity raises events, it **does not** call infrastructure services directly.
* Domain events are registered, not dispatched; the dispatcher lives in Infrastructure/Web.

---

### 2  UseCases – CQRS Handler with Validation & Structured Log

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

        // Structured logging – name placeholders, no string-interpolation
        _logger.LogInformation(
            "Scheduled download {DownloadId} for {Url}",
            download.Id, download.TargetUrl);

        return Result.Success(download.Id);
    }
}
```

*Key points*

* Uses **Guard.Against** inside handler to re-validate.
* Calls domain via repository abstraction – never talks to DbContext directly.
* Logs with **named placeholders** so Serilog / Seq etc. capture key-value pairs.

---

### 3  Infrastructure – Repository Implementation + Structured Log

```csharp
namespace WebDownloadr.Infrastructure.Data;

public sealed class EfRepository<T> : IRepository<T>
    where T : class, IAggregateRoot
{
    private readonly AppDbContext _db;
    private readonly ILogger<EfRepository<T>> _logger;

    public EfRepository(AppDbContext db,
                        ILogger<EfRepository<T>> logger)
    {
        _db     = db;
        _logger = logger;
    }

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        _logger.LogDebug(
            "Persisting {EntityType} (Id ={EntityId})",
            typeof(T).Name,
            (entity as EntityBase)?.Id);

        _db.Set<T>().Add(entity);
        await _db.SaveChangesAsync(ct);
    }
}
```

*Key points*

* Implements Core’s abstraction; **Infrastructure depends on Core**, not vice versa.
* Structured log at `Debug` level—safe for high-volume operations.
* No business rules here—only persistence logic.

---

**How to use these examples**

* AI agents: **clone** patterns (guard clauses, event registration, structured `ILogger` calls) instead of inventing new styles.
* Human reviewers: compare PR code against these snippets; discrepancies may signal architectural drift.

---

## Testing Guidelines

- **Test Framework** – All test projects **must** use [xUnit](https://xunit.net/), [Shouldly](https://shouldly.readthedocs.io/en/latest/), and [NSubstitute](https://nsubstitute.github.io/index.html).
- Place tests in the matching test project: `WebDownloadr.UnitTests`, `WebDownloadr.IntegrationTests`, `WebDownloadr.FunctionalTests`, `WebDownloadr.AspireTests` (mirroring the structure of the `src/` projects).
- Maintain test isolation. Integration tests may use [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet) for external dependencies (e.g., databases), and use [HttpClientTestExtensions](https://github.com/ardalis/HttpClientTestExtensions) for concise HTTP response assertions in functional tests.
- **Coverage validation** – Run `dotnet test --collect:"XPlat Code Coverage"` for all test projects and invoke `reportgenerator -reporttypes:HtmlSummary` on the results. Line coverage must remain **>= 90%**.
- **Agent-generated tests** – When an AI agent adds or modifies tests, verify they compile and pass. Include the resulting `HtmlSummary` report (or a screenshot) in the PR description to demonstrate coverage.

- **Analyzer relaxations for test code**

  - The test projects suppress **StyleCop rule CA1707** (identifiers should not contain underscores) so that test method names can use a descriptive pattern such as  
    `MethodUnderTest_ShouldReturnExpectedResult_WhenCondition`.  
  - Additional StyleCop or Roslyn rules may be disabled in test assemblies **only** when they improve test readability and do **not** affect production code.

- **Snapshot-test exception to minimal-diff rule**

  - Files under `tests/**/Snapshots/` (e.g., approval or snapshot assets) are treated as *generated artifacts*; the **“always output minimal diffs”** rule is waived for these files.  
    When a functional change legitimately updates a snapshot, commit the full new file—even if it is large—to preserve the canonical expected output.

---

## Pull‑Request Guidelines

- **Title** – Format as `[Layer] Short description of change` (for example, `[UseCases] Add CreateOrder command`).

- **Description** – Clearly explain the changes, list key file modifications, and include test results or screenshots if applicable.

- Reference any issue that is addressed (e.g., `Fixes #123` or `Closes #123` in the description).

- Link related issues or discussions. Use GitHub keywords to automatically close issues if appropriate.

- All quality gates must pass before review. (For documentation-only or configuration-only changes, new tests may not be needed as long as existing checks remain green.)

---

## Environment & Secrets

Never commit secrets, credentials, or API keys to the repository under any circumstance. Keep configuration outside of source control by following these patterns:

1. **ASP.NET Core User Secrets** – In development, run `dotnet user-secrets init` inside the Web project and store values with `dotnet user-secrets set`. These secrets live in `secrets.json` under your user profile and are never checked in.
2. **`.env` files** – Optionally place environment variables in a local `.env` file at the solution root. `.env` files are ignored by Git. Load them in `Program.cs` using `DotNetEnv` or a similar helper.
3. **GitHub Actions secrets** – Add tokens or passwords required for CI to the repository's Secrets settings and reference them as `${{ secrets.NAME }}` in workflow YAML. Do not print these values to the logs.

If environment variables or secrets are needed for integration tests, configure them through User Secrets or GitHub Actions secrets—never in code or configuration committed to the repository. If a credential is accidentally committed, contact the maintainers immediately so the history can be scrubbed.

---

## Architectural Notes

- Dependencies must flow **inward** (from outer layers into Core).

- Always place new code in the correct layer according to its role.

- Any public API in **Core** should have corresponding tests and (if its behavior is not obvious) documentation comments.

- Generated files (e.g., EF Core migration classes) should remain unmodified unless a specific manual change is needed (and documented).

- If multiple bounded contexts emerge over time, consider creating an internal shared-kernel library. Until then, this solution uses the external `Ardalis.SharedKernel` package for any shared domain types.

---

## Architecture Decision Records (ADR)

> **Why ADRs?**  
> ADRs capture _significant_ architectural choices—what was decided, why it was chosen, and the impact—so future contributors (human **or AI**) never have to re-debate settled questions.

---

### 1. When to Write an ADR

| Write a **new ADR** when…                                                                                                                                                                                                                                                         | Don’t bother when…                                                                                                                                                  |
|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| • Choosing / replacing a framework, library, persistence or messaging strategy.• Changing a cross-cutting pattern (e.g., switch from Mediator to Minimal APIs).• Revisiting or overturning a previous ADR.• Any decision that sparked extended debate or could puzzle a newcomer. | • The change follows an established team convention.• It is purely cosmetic or refactoring with no design impact.• A minor tweak to configuration or build scripts. |

> **AI agents:** If your task introduces a design decision that meets the left-hand criteria, you **must** draft an ADR in the pull request.

---

### 2. Location & Naming

```
docs/architecture-decisions/
└── 0007-use-minimal-apis.md   # four-digit, zero-padded index + short slug
```

- Use the **next available number** in sequence.
    
- Keep the slug concise, lowercase, hyphen-separated.
    
- Once merged, **never rename or delete** an ADR. Historical integrity matters.

---

### 3. Template (Markdown)

```markdown
# 0007 – Use Minimal APIs for Lightweight Endpoints

**Status**: Proposed | Accepted | Rejected | Superseded (by ADR-00XX)

## Context
Explain the problem or forces driving this decision.

## Decision
State the chosen option in one sentence.

## Consequences
*Positive* and *negative* outcomes, trade-offs, follow-up tasks.

## Alternatives Considered
- Option A – why not
- Option B – why not

## References
Links to discussions, spikes, benchmarks, external docs.
```

> Place this template in `docs/architecture-decisions/0000-template.md` for quick reuse.

---

### 4. Status Vocabulary

| Status         | Meaning                                                      |
|----------------|--------------------------------------------------------------|
| **Proposed**   | Draft under discussion.                                      |
| **Accepted**   | Approved; implementation may proceed.                        |
| **Rejected**   | Considered but declined (capture _why_).                     |
| **Superseded** | Replaced by a newer ADR—add “Superseded by ADR-00XX” at top. |
| **Deprecated** | Still in effect but slated for removal.                      |

_Only update an ADR’s **Status** or add a supersession notice—**do not** rewrite history._

---

### 5. Workflow

1. **Draft** – Author (human or AI) creates `docs/architecture-decisions/00NN-my-decision.md` with _Status: Proposed_.
    
2. **Pull Request** – Open a PR titled `[ADR] 00NN My Decision`.
    
3. **Review** – Team discusses, edits, and either **Accepts** or **Rejects**.
    
4. **Merge** – Merge the PR; the ADR becomes read-only (except status tag).
    
5. **Reference** – Future commits and PRs that implement or rely on this decision should cite the ADR ID.

---

### 6. Immutability & Supersession

- **Never** edit accepted ADR content.
    
- To change course, write a new ADR that:
    
    - References the old one in **Context** (“Supersedes ADR-0007”).
        
    - Sets the old ADR’s status to **Superseded** (add a one-line note at top).
        
    - Explains the _new_ decision and why circumstances changed.

---

### 7. Tooling (Optional but Encouraged)

| Tool                           | Purpose                                        |
|--------------------------------|------------------------------------------------|
| **adr-tools** / **dotnet-adr** | CLI to initialise, number, and link ADRs.      |
| **Log4Brains**                 | Generates a searchable ADR site from markdown. |
| **VS Code ADR extension**      | Snippets & status commands inside the editor.  |

---

### 8. AI-Specific Reminders

- Use the template verbatim; fill in every section.
    
- Ensure file name and status are correct.
    
- Scan `docs/architecture-decisions/` to pick the next ID—avoid collisions.
    
- Add the new ADR file to the same PR as the code change that depends on it.



## Guidance for AI Agents

- If unsure about a part of the codebase, prefer **not to change** a file unless a clear rule or failing test specifically requires it.

- If multiple valid approaches exist, choose the **simplest and most idiomatic** .NET solution.

- Always output **minimal diffs** so commits stay focused and avoid unrelated changes. Widen your changes only to fix formatting errors or analyzer warnings, or if the instructions explicitly request a broader update.

### State & Context Awareness

AI agents often complete **multi-step tasks** (e.g., draft an ADR → implement code → update tests). To avoid drifting context or leaking data, follow these rules:

1. **Carry identifiers forward.**
   If your first step creates a *UseCase* named `DownloadPageCommand`, reference that exact name (or ADR ID, issue number, etc.) in all subsequent steps and commit messages.

2. **Short-lived in-memory context only.**
   Persist information **only** in the branch you’re working on (code, ADRs, tests). Do **not** write to external stores, long-term caches, or hidden files.

3. **No secret retention.**
   Environment variables, API keys, or user-secrets may be read during the run but **must never** be logged, committed, or stored in agent memory between runs.

4. **Re-load on each invocation.**
   Assume the agent starts “fresh” every time. Re-read modified files (e.g., the ADR you just wrote) before generating follow-up changes rather than relying on previous prompt history.

5. **Ask if context is unclear.**
   If the required identifier (ADR ID, entity name, etc.) is missing or ambiguous, **pause and request clarification** instead of guessing.

These guidelines keep multi-step contributions consistent, reproducible, and free of accidental data leaks.

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

## Fallbacks & Escalation

AI agents should handle routine contributions autonomously, **but must not get stuck in endless clarification loops**.  
Follow this safety protocol whenever requirements are ambiguous:

1. **First Clarification** – Ask a concise, targeted question in the PR or chat.  
2. **Second Clarification** – If the reply is still unclear, rephrase once and reference the relevant spec or code line numbers.  
3. **Escalate to Human Reviewer** –  
   * If **after two clarification attempts** the task remains ambiguous **or** the answer conflicts with repository rules,  
   * **Stop** further automation.  
   * Tag the assigned human reviewer (e.g., `@maintainers`) and add the label `needs-human-input`.  
   * Post a short summary of what is unclear and link any related files or ADRs.

> **Never** commit speculative changes when the requirement is still unresolved.  
> The goal is to prevent silent failures and ensure architectural integrity while minimising wasted agent cycles.


## Example Repositories & Further Reading

> Curated references for AGENTS.md patterns, AI-agent frameworks, and Clean-Architecture-friendly tooling.  
> Each entry is tagged **Production-Ready** 🟢 or **Experimental** 🧪 so you can gauge stability.

### 🟢 Production-Ready

- **agentsmd.net** – Community site with 14 + structured AGENTS.md templates (includes a .NET Blazor + Clean-Architecture sample).  
- **gakeez/agents_md_collection** – GitHub repo bundling those templates; each file has YAML metadata for easy parsing.  
- **OpenAI Codex CLI** – Official terminal-first agent; reads AGENTS.md to guide coding, testing, and refactoring.  
- **LangChain** – Framework for orchestrating LLM agents (Python / JS); offers memory, tool-use, and JSON-schema enforcement modules.  
- **Microsoft Semantic Kernel** – C#/.NET SDK for building AI agents with plugins, planners, and vector memory—enterprise-ready.  
- **Qodo PR-Agent** – GitHub Action that reviews pull requests with GPT-4; demonstrates production AI/DevOps integration.

### 🧪 Experimental & Exploratory

- **Auto-GPT** – Fully autonomous loop agent that plans and executes tasks end-to-end. Powerful but resource-heavy.  
- **BabyAGI** – 200-line prototype that cycles task creation, prioritisation, execution—great for concept exploration.  
- **GPT-Engineer** – Generates entire codebases from natural-language specs; rapidly evolving, expect manual polish.  
- **Clean-Architecture PR Reviewer (demo)** – Community PoC of a .NET Clean-Architecture agent reviewing GitHub PRs.

### 🧠 Further Reading

- **Introducing Codex – OpenAI Blog** – Explains how AGENTS.md boosts alignment and safety in autonomous coding.  
- **Martin Fowler: Autonomous Agents in Codebases** – Case study of a Codex agent using AGENTS.md & README as its compass.  
- **LangChain Docs** – Deep dives on memory, tool plugins, and schema-constrained output for robust agent design.  
- **Awesome AI Agents (GitHub)** – Living index of frameworks, libraries, and research papers in the autonomous-agent ecosystem.

> **Contribute:** Found a valuable pattern or tool? Open a PR to append it here—future humans & AI will thank you!

