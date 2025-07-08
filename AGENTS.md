# AGENTS.md

## Overview

This repository implements a Clean Architecture approach. Projects are organized by layer and each layer has a dedicated folder under `src` or `tests`.

- **WebDownloadr.Core** – Domain model and business logic. No dependencies on other projects.
- **WebDownloadr.UseCases** – Application logic (commands/queries) built on top of the Core. Depends on Core.
- **WebDownloadr.Infrastructure** – Implementations for persistence, external services and other infrastructure concerns. Depends on Core and UseCases.
- **WebDownloadr.Web** – HTTP presentation layer using FastEndpoints. Depends on UseCases, Infrastructure and ServiceDefaults.
- **WebDownloadr.ServiceDefaults** – Shared configuration for service discovery, health checks and OpenTelemetry. Referenced by service projects.
- **WebDownloadr.AspireHost** – Hosts the web project when using .NET Aspire.
- **Tests** – `WebDownloadr.UnitTests`, `WebDownloadr.IntegrationTests`, `WebDownloadr.FunctionalTests` and `WebDownloadr.AspireTests`.

Documentation for architectural decisions lives in `docs/architecture-decisions`.

## Coding Standards

- **Language** – C# 12 targeting .NET 9 (`net9.0`).
- **Formatting and Naming** – Enforced via the repository `.editorconfig`. Run `dotnet format` before committing.
- **Naming conventions** – PascalCase for public members and classes, camelCase for locals and parameters.
- **XML documentation** – Required for public APIs and complex methods.
- **Treat warnings as errors** – Defined in `Directory.Build.props`.

## Build, Test, and Format

The CI workflow in `.github/workflows/ci.yml` runs these commands. Run them locally before submitting a pull request:

```bash
# Restore dependencies
 dotnet restore WebDownloadr.sln

# Build all projects
 dotnet build --no-restore WebDownloadr.sln

# Run tests
 dotnet test --no-build --no-restore WebDownloadr.sln

# Ensure formatting
 dotnet format
```

You may combine them:

```bash
dotnet restore WebDownloadr.sln && \
  dotnet build --no-restore WebDownloadr.sln && \
  dotnet test --no-build --no-restore WebDownloadr.sln && \
  dotnet format
```

## Testing Guidelines

- Tests use **xUnit** with **Shouldly** for assertions and **NSubstitute** for mocking.
- Add tests in the matching test project (`UnitTests`, `IntegrationTests`, `FunctionalTests`, or `AspireTests`).
- Keep tests isolated and avoid using external services unless explicitly writing integration tests.

## Pull Request Guidelines

- **Title format** – `[Layer] Short descriptive title` (example: `[UseCases] Add CreateOrder command handler`).
- **Description** – Summarize changes, list affected projects/files, and provide testing instructions and results.
- All tests must pass and code must be formatted.
- Reference related issues or tickets when applicable.

## Environment and Secrets

- Never commit secrets or sensitive data. Use environment variables or user secrets for local development.
- `.env` files can be used locally but should remain untracked.

## Architectural Notes

- Clean Architecture dependencies must flow inward. Outer layers (`Web`, `Infrastructure`) should not contain business rules.
- Place new code in the appropriate project following the structure above.
- Do not modify generated files or migrations unless explicitly instructed.

## Additional Resources

- [Architecture Decision Records](docs/architecture-decisions)
- [CONTRIBUTING.md](CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](CODE_OF_CONDUCT.md)
- [Clean Architecture repository](https://github.com/ardalis/CleanArchitecture)

