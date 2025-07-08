#!/usr/bin/env bash
set -euo pipefail

mode="verify"
if [[ "${1:-}" == "--fix" ]]; then
  mode="fix"
fi

# Restore packages so analyzers run correctly
dotnet restore WebDownloadr.sln

if [[ "$mode" == "fix" ]]; then
  dotnet format WebDownloadr.sln
  dotnet format analyzers WebDownloadr.sln
  if ! git diff --quiet; then
    git add -A
    git commit -m "style: apply dotnet format and analyzer fixes"
  fi
else
  dotnet format WebDownloadr.sln --verify-no-changes
  dotnet format analyzers WebDownloadr.sln --verify-no-changes
fi

git diff --exit-code
