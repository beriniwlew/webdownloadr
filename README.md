# WebDownloadr

![CI](https://github.com/beriniwlew/webdownloadr/actions/workflows/ci.yml/badge.svg?branch=main)

WebDownloadr demonstrates how to apply Clean Architecture to a simple web page downloader built with .NET 9. The solution is organized into
separate projects that isolate the domain model, application services, infrastructure and API.

This repository is optimized for **AI‑assisted development**. The [AGENTS.md](AGENTS.md) guidelines enable Codex and other AI agents to contribute safely by
enforcing layer boundaries, formatting rules and documentation standards. Together these practices help maintain a clean architecture even as the
project grows.

## Projects

- **WebDownloadr.Core** – domain model containing the [`WebPage` aggregate](src/WebDownloadr.Core/WebPageAggregate/README.md).
- **WebDownloadr.UseCases** – application layer describing operations such as requesting downloads (see
  [README](src/WebDownloadr.UseCases/README.md)).
- **WebDownloadr.Infrastructure** – implementations of external dependencies like HTTP clients.
- **WebDownloadr.Web** – minimal API exposing endpoints (see [README](src/WebDownloadr.Web/README.md)).
- **Tests** – unit, integration and functional test projects.

## Architecture Decisions

This project uses [Architecture Decision Records (ADRs)](docs/architecture-decisions/README.md) to document significant architectural
choices. Key decisions include:

- **[0001: Adopt Clean Architecture](docs/architecture-decisions/0001-adopt-clean-architecture.md)** – Foundation for the layered
  architecture
- **[0002: Optimize AGENTS Usage](docs/architecture-decisions/0002-optimize-agents-usage.md)** – Standardized contribution guidelines
- **[0003: Web Pages Functionality](docs/architecture-decisions/0003-web-pages-functionality.md)** – Domain-driven implementation approach
- **[0004: .NET DI Adoption](docs/architecture-decisions/0004-dotnet-di-adoption.md)** – Technology choice for dependency injection

For new architectural decisions, see the [ADR guidelines](docs/architecture-decisions/AGENTS.md).

## Getting Started

Restore packages and run the Web project:

```bash
dotnet run --project src/WebDownloadr.Web/WebDownloadr.Web.csproj
```

Browse to `/swagger` for API documentation while the app is running.

## Environment Setup

Run `./scripts/setup-codex.sh` to install the .NET SDK and required global tools before building the solution.

## Documentation

Additional documentation lives in the `docs` folder and within each project. Start with
[`src/WebDownloadr.Core/README.md`](src/WebDownloadr.Core/README.md) to learn about the domain model.

## Formatting

Formatting is enforced in CI. All text files use LF endings. Configure Git to convert CRLF on commit:

```bash
git config --global core.autocrlf input
```

For the initial bootstrap, maintainers should normalize all files and run the autoformat script before committing:

```bash
git add --renormalize .
git commit -m "style: normalize line endings to match .editorconfig"
./scripts/autoformat.sh
```

The repository's `.gitattributes` enforces LF for text files and marks common binaries.

After this commit, run `./scripts/format.sh` (or `dotnet format`) locally to ensure no style fixes are needed.

Prettier and `markdownlint-cli2` enforce documentation style. They run automatically via pre-commit but you can verify manually with
`npx prettier --check .` and `npx markdownlint-cli2`.

## Pre-commit

Node.js **v20** or later is required for the documentation hooks. Install [pre-commit](https://pre-commit.com/) and set up the git hook:

```bash
pip install pre-commit
pre-commit install
```

Running `pre-commit` will execute the hooks defined in `.pre-commit-config.yaml`.

Before pushing changes, run `./scripts/selfcheck.sh` to ensure build, tests, and formatting pass locally.

## License

This project is licensed under the [MIT](LICENSE) license.
