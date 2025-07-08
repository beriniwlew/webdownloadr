# Contributing to WebDownloadr

Thank you for helping improve **WebDownloadr**. This repository follows the workflow documented in [AGENTS.md](AGENTS.md).

## Quick Start

1. Ensure the .NET SDK specified in `global.json` is installed.
2. Run `./scripts/install-tools.sh` to set up required tools.
3. Execute `./scripts/selfcheck.sh` and make sure it exits with `0`.
4. Commit using the format `[Layer] <summary>`.

## Pull Requests

Submit pull requests against the `main` branch. All checks in `selfcheck.sh` must succeed and CI must be green.

## Code Style & Testing

Formatting, analyzers, and test coverage are enforced via `dotnet format` and `dotnet test` with Coverlet. See `AGENTS.md` for detailed guidance.

