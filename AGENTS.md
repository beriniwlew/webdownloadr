---
title: 'AGENTS.md'
project: 'WebDownloadr'
language: 'C# 12 (.NET 9)'
architecture: 'Clean Architecture (Ardalis Template)'
commit_convention: 'Conventional Commits (<type>(<scope>): <summary>)'
branch_prefixes:
  - feature/
  - fix/
  - chore/
  - docs/
agents_md_inheritance: 'Global (~/.codex) → Repo root → Nested (deeper overrides parent)'
ci_requirements:
  build_warnings: 0
  tests_pass: '100%'
  coverage_min: '≥ 90%'
  formatting_drift: 0
---

# AGENTS.md

> This document guides AI agents and human contributors. See [AGENTS_orig.md](AGENTS_orig.md) for detailed historical rules.

## Project Overview

WebDownloadr is a .NET 9 sample application demonstrating a Clean Architecture approach for downloading pages. It exposes
FastEndpoints-based APIs to enqueue jobs and fetch pages asynchronously, managing a persistent download queue. The solution strictly
separates concerns into layers (Core, UseCases, Infrastructure, Web), enforcing entity boundaries and inward dependency flow. At a high
level, the Core project holds the domain model (WebPage entities) and UseCases contain CQRS handlers. Infrastructure implements persistence
and HTTP services, while the Web project hosts the API.

## Tech Stack

- **Language / Runtime** C# 12 on **.NET 9.0 Preview** (pinned via `global.json`)
- **API Layer** [FastEndpoints v6.2] – ultra-thin minimal-API wrapper
- **CQRS / Mediator** MediatR 13 (+ FastEndpoints integration)
- **Persistence** Entity Framework Core 9.0.6 (SQL Server provider)
- **Domain Validation** Ardalis.GuardClauses, FluentValidation
- **Logging / Telemetry** Serilog (+ OpenTelemetry exporters)
- **Testing** xUnit 2.9.3, NSubstitute 5.3, Shouldly 4.3, Microsoft.AspNetCore.Mvc.Testing 9.0.6
- **Code Quality** Roslyn analyzers (Microsoft + StyleCop), `dotnet-format`, markdownlint-cli2, Prettier
- **Coverage** coverlet.collector + ReportGenerator (≥ 90 % gate)
- **Scripting / Tooling** Bash scripts in `scripts/`, Docker Compose for optional local SQL Server

---

## Essential Commands

Below are the commands Codex (and humans) should run in typical workflows.

```bash
# 🔧 Restore packages & build solution (Release configuration)
dotnet restore WebDownloadr.sln
dotnet build   WebDownloadr.sln --configuration Release

# ⚙️  Format & lint all code and docs
./scripts/format.sh       # dotnet-format + markdownlint + prettier

# ✅ Run all unit / integration tests with coverage
dotnet test WebDownloadr.sln \
  --collect:"XPlat Code Coverage" \
  --configuration Release

# 📊 Generate coverage report (HTML)
reportgenerator -reports:**/coverage.cobertura.xml -targetdir:coverage

# 🚀 Run the API locally (hot-reload disabled for deterministic output)
dotnet run --project src/WebDownloadr.Web/WebDownloadr.Web.csproj

# 🩺 One-shot self-check (build → test → format → coverage ≥ 90 %)
./scripts/selfcheck.sh
```

## Development Workflow

1. **Create a feature branch** – name it `feat/<layer>/<short-desc>` or `fix/<layer>/<short-desc>`.
2. **Implement the change** following Clean-Architecture layer rules.
3. **Run local quality gate**:

   ```bash
   ./scripts/selfcheck.sh  # restore → build → test → format → coverage ≥90 %
   ```

   – If any step fails, fix before committing.

4. **Commit** using Conventional Commits (`type(scope): summary`).
5. **Push & open PR** – title `[Layer] <summary>` and fill in the PR template checklist.
6. **Ensure CI passes** (same self-check); address feedback before merge.

Optional helpers:

- `./scripts/archtest.sh` – run architecture tests
- `dotnet ef migrations add <Name>` – create EF Core migration when persistence changes
- `scripts/format.sh` – auto-apply `dotnet format`, Prettier, markdownlint

## Code Quality & Formatting

- All Roslyn analyzers and StyleCop rules run at severity **error**; the build must compile clean.
- Source files use file-scoped namespaces and `#nullable enable`.
- Types and methods follow PascalCase while locals and parameters use camelCase.
- Prefer explicit access modifiers; `var` is allowed when the type is obvious.
- Architecture boundaries are verified by running `./scripts/archtest.sh`.
- Run the formatters and linters before committing:

  ```bash
  dotnet format --verify-no-changes
  npx markdownlint-cli2 "**/*.md" "#node_modules"
  npx prettier --check .
  ```

- Commit messages are validated with `commitlint`.
- CI fails if global line coverage < 90 % — do not suppress tests.

## Testing Guidelines

- Write xUnit tests using NSubstitute and Shouldly.
- New or modified code must include matching tests.
- Execute `dotnet test --no-build --no-restore` to run tests and collect coverage (minimum 90%).
- Integration tests may use in-memory or ephemeral databases as needed.

## Nested `AGENTS.md` Files

Layer-specific rule files reside under `src/*/AGENTS.md` and override this root file for their respective folders.

## Pull Request & Commit Conventions

- Commit messages follow Conventional Commits; example: `feat/web: add health endpoint`.
- PR titles are imperative and describe what the change does.
- Include a summary of key changes and links to ADRs or issues.
- Ensure `./scripts/selfcheck.sh` succeeds both locally and in CI.

## CI/CD and Automation

GitHub Actions run `scripts/selfcheck.sh` on every PR. Coverage must stay above 90% and formatting must match. Tools like Ruler or Codex CLI
may be integrated to enforce these instructions automatically.

## Context & Examples

Detailed patterns, examples and architectural notes are preserved in [AGENTS_orig.md](AGENTS_orig.md) and nested AGENTS files. Consult them
for code snippets illustrating domain events, pipeline behaviors and other conventions.
