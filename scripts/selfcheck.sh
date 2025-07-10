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
npx --yes markdownlint-cli2 --ignore-path .markdownlintignore "**/*.md"
npx --yes prettier --check "**/*.md" "**/*.json" --ignore-path .prettierignore

# Validate commit messages
npm install --no-save @commitlint/config-conventional
npx --yes commitlint --from HEAD~1
