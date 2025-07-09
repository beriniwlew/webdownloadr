#!/usr/bin/env bash
set -euo pipefail

# Build with warnings as errors
 dotnet build --no-restore -warnaserror WebDownloadr.sln

# Run tests
 dotnet test --no-build --no-restore WebDownloadr.sln

# Verify formatting including analyzer rules
 dotnet format WebDownloadr.sln --verify-no-changes --no-restore
 dotnet format WebDownloadr.sln analyzers --verify-no-changes --no-restore

# Validate commit messages
npm install --no-save @commitlint/config-conventional
npx --yes commitlint --from HEAD~1
