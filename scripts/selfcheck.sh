#!/usr/bin/env bash
set -euo pipefail

# Build with warnings as errors
 dotnet build --no-restore -warnaserror WebDownloadr.sln

# Run tests
 dotnet test --no-build --no-restore WebDownloadr.sln

# Verify formatting including analyzer rules
 dotnet format WebDownloadr.sln --verify-no-changes --no-restore
 dotnet format WebDownloadr.sln analyzers --verify-no-changes --no-restore
