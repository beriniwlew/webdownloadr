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

---

## Essential Commands

```bash
# Install SDK and tools
./scripts/setup-codex.sh

# Restore dependencies
dotnet restore WebDownloadr.sln

# Build with warnings as errors
dotnet build --no-restore WebDownloadr.sln -warnaserror

# Run tests with coverage
dotnet test --no-build --no-restore WebDownloadr.sln

# Verify formatting and analyzers
dotnet format WebDownloadr.sln --verify-no-changes --no-restore
dotnet format WebDownloadr.sln analyzers --verify-no-changes --no-restore

# Lint markdown and JSON docs
npx markdownlint-cli2 "**/*.md" "#node_modules"
npx prettier --check --ignore-path .prettierignore "**/*.{md,json}"

# One-stop check
./scripts/selfcheck.sh
# Skip individual steps with flags like `--skip-test` or `--skip-format` if needed
```

## Development Workflow

1. Create a branch using one of the prefixes above.
2. Implement your change respecting Clean Architecture boundaries.
3. Run `./scripts/selfcheck.sh` locally; fix any issues.
4. Commit with `type(scope): summary` and push.
5. Open a pull request with an imperative title and descriptive body referencing issues (e.g. `Fixes #42`).

## Code Quality & Formatting

- Analyzer warnings are treated as errors.
- `dotnet format` and `dotnet format analyzers` must report no changes.
- `markdownlint-cli2` and `prettier --check` enforce documentation style.
- Follow `.editorconfig` settings such as file-scoped namespaces and LF endings.
- Architecture rules are checked via `scripts/archtest.sh`.

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
