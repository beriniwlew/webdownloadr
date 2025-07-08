# AGENTS.md

> **This file is binding for both humans and AI agents.** Any contribution must comply with every rule herein, or the pull‑request will be rejected.

---

## Purpose

Codifies the operational rules for this repository so that **AI-powered agents and human contributors** can work safely within a **Clean Architecture** codebase based on the [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture) template while following modern .NET 9 best practices.

---

## Solution & Project Layout

| Layer               | Folder                             | Brief description                                                                                                                                                                                                                          |
| ------------------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Core**            | `src/WebDownloadr.Core`            | Domain entities, value objects, domain events, and interfaces—**no external dependencies** except [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) & [`Ardalis.Specification`](https://github.com/ardalis/Specification). |
| **UseCases**        | `src/WebDownloadr.UseCases`        | CQRS command/query handlers, request/response DTOs, validators, and pipeline behaviors. Has a project reference only to **Core** and may use external packages but must not depend on **Infrastructure** or **Web**.                                                                                                                                                 |
| **Infrastructure**  | `src/WebDownloadr.Infrastructure`  | Contains EF Core `DbContext`, external service adapters, and persistence implementations. These align with Core interfaces and reside under the `Data/` folder. |
| **Web**             | `src/WebDownloadr.Web`             | HTTP API using [FastEndpoints 6](https://fast-endpoints.com/docs/introduction); hosts application services. Depends on **UseCases**, **Infrastructure**, and **ServiceDefaults**.                                                         |
| **ServiceDefaults** | `src/WebDownloadr.ServiceDefaults` | Shared startup & telemetry helpers for [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/) and cloud hosting.                                                                                                                |
| **AspireHost**      | `src/WebDownloadr.AspireHost`      | Runs the Web project when using .NET Aspire.                                                                                                                                                                                               |
> **Note:** Usage of .NET Aspire is optional and remains preview in .NET 9.
| **Tests**           | `tests/*`                          | `Unit`, `Integration`, `Functional`, and `Aspire` test projects mirroring the structure above. For every new or modified feature in `src/`, a corresponding test must be added or updated in `tests/`.                                     |

> **Dependency rule** – References must flow **inward**: `Web` may reference **UseCases** and **Infrastructure**; both **UseCases** and **Infrastructure** may reference **Core** only. **UseCases** and **Infrastructure** must not reference each other. Dependency rules are reviewed manually. If architectural drift becomes an issue, consider enforcing with tools like NetArchTest or ArchUnitNET.

| Layer          | May Reference                             |
| -------------- | ----------------------------------------- |
| Web            | UseCases, Infrastructure, ServiceDefaults |
| Infrastructure | Core                                      |
| UseCases       | Core                                      |
| Core           | —                                         |

---

## Agent Responsibilities

1. **Create** a branch named `feature/<slug>` for new features. Use `fix/<slug>`, `chore/<slug>`, or `docs/<slug>` for other updates.
2. **Run** `./scripts/selfcheck.sh` locally. It **must** exit with `0`.
3. **Commit** using `[Layer] <type>: <summary>` format following [Conventional Commits](https://www.conventionalcommits.org/). Validate messages with commitlint.
4. **Push** and open a pull request.
5. Ensure **CI is green** (same steps as `selfcheck.sh`).

### Typical Tasks

| Scenario                                 | Destination folder / pattern                                                                                      |
| ---------------------------------------- | ----------------------------------------------------------------------------------------------------------------- |
| Add domain entity or value object        | `src/WebDownloadr.Core/<Aggregate>/<Entity>.cs`                                                                   |
| Add repository interface / specification | `src/WebDownloadr.Core/Interfaces`                                                                                |
| Implement repository & EF mapping        | `src/WebDownloadr.Infrastructure/Data`                                                                            |
| Add/update CQRS command or query handler | `src/WebDownloadr.UseCases/Commands/<Feature>/`                                                                   |
| Write unit test                          | `tests/WebDownloadr.UnitTests/<Feature>Tests.cs`                                                                  |
| Expose REST endpoint                     | `src/WebDownloadr.Web/Modules/<Feature>/`                                                                         |
| Write integration or functional test     | `tests/WebDownloadr.IntegrationTests/<Feature>Tests.cs` or `tests/WebDownloadr.FunctionalTests/<Feature>Tests.cs` |
| Add domain event handler                 | `src/WebDownloadr.Core/<Aggregate>/Handlers/<EventHandler>.cs`                                                     |

* **Naming tip:** prefer `XCommandHandler.cs` and `XQueryHandler.cs` for handler files.
---

## DO NOT

* Change files in `docs/architecture-decisions/` unless the change **is** an ADR.
* Add references from **Web** or **Infrastructure** *back* to **Core**.
* Commit secrets or credentials; use environment variables or user‑secrets.
* Lower test coverage or disable analyzers.
* Add MVC Controllers or Razor Pages to the **Web** project – use FastEndpoints or ApiEndpoints instead.
* Modify generated migration files unless explicitly instructed.
* Edit or reformat code files solely for whitespace or style unless addressing a formatter/analyzer warning.

---

## Quality Gates

| Check                    | Requirement                                                                                                    |
| ------------------------ | -------------------------------------------------------------------------------------------------------------- |
| Build warnings           | **0**                                                                                                          |
| Unit & integration tests | **100 % pass**                                                                                                 |
| Line coverage            | **≥ 90 %** ([Coverlet](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/GlobalTool.md)) |
| Formatter drift          | **0 files** (`dotnet format --verify-no-changes`)                                                              |

The pull‑request will be blocked if any gate fails. CI runs via [GitHub Actions](.github/workflows/ci.yml). The README **must** display a CI status badge for `.github/workflows/ci.yml` to ensure build visibility.
Branch protection on `main` requires these checks to pass before merging.

---

## Runtime Environment

* **.NET SDK 9.0.301** must be on `PATH` (see `global.json`).
* Ubuntu 22.04 image `mcr.microsoft.com/dotnet/sdk:9.0` is the reference container.
* Install extra tooling via `./scripts/install-tools.sh` (e.g., `dotnet-outdated`, `reportgenerator`). If new tools are required, update this script and document their use in this file or in `CONTRIBUTING.md`.

---

## Database & Migrations

When you introduce or modify persistent entities you **must generate a new EF Core migration** so the schema stays in sync with the model.

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

* Input validation occurs in two layers:

  1. **Web endpoints** – use FluentValidation or FastEndpoints’ built‑in validators.
  2. **UseCases handlers** – re‑validate commands/queries via pipeline behaviors.
* The **domain model assumes pre‑validated inputs** and therefore uses guard clauses and exceptions to enforce invariants. Domain entities should throw exceptions or raise domain events when invariants are violated; they must not attempt to coerce invalid data.
* Follow the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern) for request/response DTOs.

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
To enforce the 90% coverage threshold in CI, add a step that fails when the summary XML shows less than 90% line coverage.

```yaml
- name: Verify Code Coverage Threshold
  run: |
    COVERAGE=$(grep -Po 'line-rate="\\K[0-9.]+(?=\")' TestResults/coverage-report/coverage.xml | head -1)
    PERCENT=$(awk "BEGIN {print $COVERAGE * 100}")
    echo "Line coverage: $PERCENT%"
    awk -v p=$PERCENT 'BEGIN {exit (p<90)}'
```
Run this script locally **before pushing a branch or opening a PR**. Any non‑zero exit code must abort the change. PRs with failing checks will be auto-closed.

---

## Coding Standards

* **Language** – C# 12 targeting `net9.0`.
* **Nullable & ImplicitUsings** – enabled.
* **Analyzers** – [`Microsoft.CodeAnalysis.NetAnalyzers`](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview), [`StyleCop.Analyzers`](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) (warnings treated as errors).
* **Formatting & naming** – Enforced via `.editorconfig`; run `dotnet format`.
* **File‑scoped namespaces**, **top‑level statements** in `Program.cs`.
* Use [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) and raise **Domain Events** for invariants.
* XML docs required for public Core APIs and complex methods.
* Public methods in Core must include XML doc comments. If executable, they should have matching unit tests.

---

## Testing Guidelines

* Test framework: [xUnit](https://xunit.net/) with [Shouldly](https://shouldly.readthedocs.io/en/latest/) + [NSubstitute](https://nsubstitute.github.io/index.html).
* Place tests in the matching project: `UnitTests`, `IntegrationTests`, `FunctionalTests`, `AspireTests`.
* Maintain test isolation. Integration tests may use [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet) for external dependencies, and [HttpClientTestExtensions](https://github.com/ardalis/HttpClientTestExtensions) for concise HTTP assertions.

---

## Pull‑Request Guidelines

* **Title** – `[Layer] Short descriptive title` (e.g., `[UseCases] Add CreateOrder command`).
* **Description** – Explain the change, list affected files, include test results.
* If your PR addresses an issue, reference it in the description using `Fixes #<issue>` or `Closes #<issue>`.
* Link related issues; close them via keywords if appropriate.
* All quality gates must pass. Documentation-only or config-only changes may skip new tests if existing checks remain green.

---

## Environment & Secrets

Never commit secrets or sensitive config. Local `.env` files are git‑ignored by default. Use [ASP.NET Core User Secrets](https://learn.microsoft.com/dotnet/core/extensions/user-secrets) for development. If .env files are needed for CI or testing, ensure they are created by the workflow and not checked into the repo.

---

## Architectural Notes

* Dependencies must flow **inward**.
* Place new code in the correct layer.
* Public APIs in **Core** require corresponding tests & docs.
* Generated files (e.g., EF Core migrations) must remain unedited unless explicitly required and documented.
* If multiple bounded contexts emerge, consider creating an internal `SharedKernel` package. Until then, the solution uses `Ardalis.SharedKernel`.

---

## Architecture Decision Records (ADR)

Architecture Decision Records capture key design choices and the options we considered.
They reside under `docs/architecture-decisions/` and mirror the
[ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture/tree/main/docs/architecture-decisions) format.

- **When to write** – create one whenever the team debates multiple approaches or revisits a prior decision.
- **Format** – Markdown file with an incrementing four-digit prefix and short slug,
  e.g., `0001-use-fastendpoints.md`. Include `Status`, `Context`, `Decision`,
  `Consequences`, and `References` sections. Clearly list alternatives considered and why they were rejected.
- **Numbering** – use the next available number in sequence and keep the slug concise.
- **Author** – whoever proposes the change (human or AI) drafts the ADR.
- **Review** – submit via pull request. Reviewers verify numbering and clarity.
- **Superseding** – once merged, ADRs are immutable. Create a new record that supersedes previous ones.
- **Tools** – optional helpers like [adr-tools](https://github.com/npryce/adr-tools) may generate templates.

---

## Additional Resources

* [Architecture Decision Records](docs/architecture-decisions)
* [Getting Started with Architecture Decision Records](https://ardalis.com/getting-started-with-architecture-decision-records/)
* [CONTRIBUTING.md](CONTRIBUTING.md)
* [CODE\_OF\_CONDUCT.md](CODE_OF_CONDUCT.md)
* [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
* [.NET Docs](https://learn.microsoft.com/dotnet/)
* [Ardalis Guard Clauses](https://github.com/ardalis/GuardClauses)
* [Ardalis Specification](https://github.com/ardalis/Specification)
* [FastEndpoints Documentation](https://fast-endpoints.com/docs/introduction)
* [.NET Aspire](https://learn.microsoft.com/en-us/dotnet/aspire/)
* [xUnit.net Getting Started](https://xunit.net/)
* [Shouldly Assertions](https://shouldly.readthedocs.io/en/latest/)
* [NSubstitute](https://nsubstitute.github.io/index.html)
* [Coverlet Coverage Tool](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/GlobalTool.md)
* [ReportGenerator](https://danielpalme.github.io/ReportGenerator/)
* [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet)
* [NetArchTest](https://github.com/BenMorris/NetArchTest)
* [HttpClientTestExtensions](https://github.com/ardalis/HttpClientTestExtensions)
* [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated)
* [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
* [Microsoft Code Analysis](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview)
* [Entity Framework Core Docs](https://learn.microsoft.com/ef/core/)
* [.NET 9 SDK – What’s New](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9)

---

## Guidance for AI Agents

* If unsure, prefer **not to change** a file unless a clear rule or test requires it.
* If multiple valid approaches exist, prefer the **simplest and most idiomatic** .NET solution.
* Always output **minimal diffs**—avoid broad formatting or style changes outside the scope of your PR.
