#!/usr/bin/env bash
set -euo pipefail

# Build with warnings as errors
dotnet build --no-restore -warnaserror WebDownloadr.sln

# Run tests
dotnet test --no-build --no-restore WebDownloadr.sln

# --- Verify architecture rules
./scripts/archtest.sh

# Verify formatting including analyzer rules
dotnet format WebDownloadr.sln --verify-no-changes --no-restore
dotnet format WebDownloadr.sln analyzers --verify-no-changes --no-restore

# Lint and format Markdown documentation
npx --yes markdownlint-cli2 "**/*.md" "#node_modules"   # ignore via '#' glob
npx --yes prettier --check --ignore-path .prettierignore "**/*.{md,json}"

# Validate commit messages
npm install --no-save @commitlint/config-conventional
npx --yes commitlint --from HEAD~1
