# Contributing to WebDownloadr

Thank you for helping improve **WebDownloadr**. This repository follows the workflow documented in [AGENTS.md](AGENTS.md).

## Quick Start

1. Run `./scripts/setup-codex.sh` to install the .NET SDK and required global tools.
2. Install [Node.js](https://nodejs.org/) **v20** or later and ensure `npm` is available on your `PATH`.
3. Execute `./scripts/selfcheck.sh` (build, tests, formatting, `markdownlint-cli2`, and `prettier --check`) and make sure it exits with `0`.
4. Commit using the format `type(scope): <summary>` following [Conventional Commits](https://www.conventionalcommits.org/).

## Pull Requests

Submit pull requests against the `main` branch. All checks in `selfcheck.sh` must succeed and CI must be green.

## Code Style & Testing

Formatting, analyzers, and test coverage are enforced via `dotnet format` and `dotnet test` with Coverlet. See `AGENTS.md` for detailed
guidance.
