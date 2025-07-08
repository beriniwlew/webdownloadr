# AGENTS.md

> **This file is binding for both humans and AI agents.** Any contribution must comply with every rule herein, or the pull‑request will be rejected.

---

## Purpose

Codifies the operational rules for this repository so that Codex‑style agents (and human contributors) can work safely within a **Clean Architecture** codebase based on the [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture) template while following modern .NET 9 best practices.

---

## Solution & Project Layout

| Layer               | Folder                             | Brief description                                                                                                                                                                                                                          |
| ------------------- | ---------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **Core**            | `src/WebDownloadr.Core`            | Domain entities, value objects, domain events, and interfaces—**no external dependencies** except [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) & [`Ardalis.Specification`](https://github.com/ardalis/Specification). |
| **UseCases**        | `src/WebDownloadr.UseCases`        | CQRS command/query handlers, validators, and pipeline behaviors. Depends on **Core** only.                                                                                                                                                 |
| **Infrastructure**  | `src/WebDownloadr.Infrastructure`  | [EF Core](https://learn.microsoft.com/ef/core/) `DbContext`, external service adapters, and persistence. Implements Core interfaces; depends on **Core** & **UseCases**.                                                                   |
| **Web**             | `src/WebDownloadr.Web`             | HTTP API using [FastEndpoints v2](https://fast-endpoints.com/docs/introduction); hosts application services. Depends on **UseCases**, **Infrastructure**, and **ServiceDefaults**.                                                         |
| **ServiceDefaults** | `src/WebDownloadr.ServiceDefaults` | Shared startup & telemetry helpers for [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/overview) and cloud hosting.                                                                                                                |
| **AspireHost**      | `src/WebDownloadr.AspireHost`      | Runs the Web project when using .NET Aspire.                                                                                                                                                                                               |
| **Tests**           | `tests/*`                          | `Unit`, `Integration`, `Functional`, and `Aspire` test projects mirroring the structure above.                                                                                                                                             |

> **Dependency rule** – References must flow **inward** (Web → Infrastructure → UseCases → Core). The build will fail if an outer layer references an inner layer.

---

## Agent Responsibilities

1. **Create** a branch named `feature/<short‑slug>` (if your environment allows).
2. **Run** `./scripts/selfcheck.sh` locally. It **must** exit with `0`.
3. **Commit** with the message format `[Layer] <summary>`.
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

---

## DO NOT

* Change files in `docs/architecture-decisions/` unless the change **is** an ADR.

* Add references from **Web** or **Infrastructure** *back* to **Core**.

* Commit secrets or credentials; use environment variables or user‑secrets.

* Lower test coverage or disable analyzers.

* Add MVC Controllers or Razor Pages to the **Web** project – use FastEndpoints or ApiEndpoints instead.

* Modify generated migration files unless explicitly instructed.

---

## Quality Gates

| Check                    | Requirement                                                                                                    |
| ------------------------ | -------------------------------------------------------------------------------------------------------------- |
| Build warnings           | **0**                                                                                                          |
| Unit & integration tests | **100 % pass**                                                                                                 |
| Line coverage            | **≥ 90 %** ([Coverlet](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/GlobalTool.md)) |
| Formatter drift          | **0 files** (`dotnet format --verify-no-changes`)                                                              |

The pull‑request will be blocked if any gate fails.

---

## Runtime Environment

* **.NET SDK 9.0.301** must be on `PATH` (see `global.json`).
* Ubuntu 22.04 image `mcr.microsoft.com/dotnet/sdk:9.0` is the reference container.
* Install extra tooling via `./scripts/install-tools.sh` (e.g., `dotnet-outdated`, `reportgenerator`).

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

> **Rule:** All migrations live under `src/WebDownloadr.Infrastructure/Data/Migrations` and use the `AppDbContext`.

### 2. Apply Migration (optional)

The Web project’s startup automatically executes any **pending** migrations when it boots in *Development* or *Docker* environments. If you want to verify the schema before running the host (e.g., in CI or local testing), you may apply it manually:

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
* The **domain model assumes pre‑validated inputs** and therefore uses guard clauses and exceptions to enforce invariants.
* Follow the [REPR pattern](https://deviq.com/design-patterns/repr-design-pattern) for request/response DTOs.

---

## Build, Test & Format

The exact commands executed by CI are encapsulated in **`scripts/selfcheck.sh`**:

```bash
#!/usr/bin/env bash
set -euo pipefail

dotnet restore WebDownloadr.sln

dotnet build --no-restore -warnaserror WebDownloadr.sln

dotnet test --no-build --no-restore WebDownloadr.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults

dotnet format --verify-no-changes WebDownloadr.sln --no-restore

reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/coverage-report" -reporttypes:HtmlSummary
```

Run this script locally **before every commit**. Any non‑zero exit code must abort the change.

---

## Coding Standards

* **Language** – C# 12 targeting `net9.0`.
* **Nullable & ImplicitUsings** – enabled.
* **Analyzers** – [`Microsoft.CodeAnalysis.NetAnalyzers`](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview), [`StyleCop.Analyzers`](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) (warnings treated as errors).
* **Formatting & naming** – Enforced via `.editorconfig`; run `dotnet format`.
* **File‑scoped namespaces**, **top‑level statements** in `Program.cs`.
* Use [`Ardalis.GuardClauses`](https://github.com/ardalis/GuardClauses) and raise **Domain Events** for invariants.
* XML docs required for public Core APIs and complex methods.

---

## Testing Guidelines

* Test framework: [xUnit](https://xunit.net/docs/getting-started/net6) with [Shouldly](https://shouldly.readthedocs.io/en/latest/) + [NSubstitute](https://nsubstitute.github.io/help/).
* Place tests in the matching project: `UnitTests`, `IntegrationTests`, `FunctionalTests`, `AspireTests`.
* Maintain test isolation.
  Integration tests may use [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet) for external dependencies.

---

## Pull‑Request Guidelines

* **Title** – `[Layer] Short descriptive title` (e.g., `[UseCases] Add CreateOrder command`).
* **Description** – Explain the change, list affected files, include test results.
* Link related issues; close them via keywords if appropriate.
* All quality gates must pass.

---

## Environment & Secrets

Never commit secrets or sensitive config. Local `.env` files are git‑ignored by default. Use [ASP.NET Core User Secrets](https://learn.microsoft.com/dotnet/core/extensions/user-secrets) for development.

---

## Architectural Notes

* Dependencies must flow **inward**.
* Place new code in the correct layer.
* Public APIs in **Core** require corresponding tests & docs.
* Generated files (e.g., EF Core migrations) should not be edited manually.

---

## Additional Resources

* [Architecture Decision Records](docs/architecture-decisions)
* [CONTRIBUTING.md](CONTRIBUTING.md)
* [CODE\_OF\_CONDUCT.md](CODE_OF_CONDUCT.md)
* [ardalis/CleanArchitecture](https://github.com/ardalis/CleanArchitecture)
* [.NET Docs](https://learn.microsoft.com/dotnet/)
* [Ardalis Guard Clauses](https://github.com/ardalis/GuardClauses)
* [Ardalis Specification](https://github.com/ardalis/Specification)
* [FastEndpoints Documentation](https://fast-endpoints.com/docs/introduction)
* [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/overview)
* [xUnit.net Getting Started](https://xunit.net/docs/getting-started/net6)
* [Shouldly Assertions](https://shouldly.readthedocs.io/en/latest/)
* [NSubstitute](https://nsubstitute.github.io/help/)
* [Coverlet Coverage Tool](https://github.com/coverlet-coverage/coverlet/blob/master/Documentation/GlobalTool.md)
* [ReportGenerator](https://danielpalme.github.io/ReportGenerator/)
* [TestContainers for .NET](https://github.com/testcontainers/testcontainers-dotnet)
* [NetArchTest](https://github.com/BenMorris/NetArchTest)
* [dotnet-outdated](https://github.com/dotnet-outdated/dotnet-outdated)
* [StyleCop Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers)
* [Microsoft Code Analysis](https://learn.microsoft.com/dotnet/fundamentals/code-analysis/overview)
* [Entity Framework Core Docs](https://learn.microsoft.com/ef/core/)
* [.NET 9 SDK – What’s New](https://learn.microsoft.com/dotnet/core/whats-new/dotnet-9)
