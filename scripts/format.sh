#!/usr/bin/env bash
set -euo pipefail

mode="verify"
if [[ "${1:-}" == "--fix" ]]; then
  mode="fix"
fi

# Restore packages so analyzers run correctly
dotnet restore WebDownloadr.sln

if [[ "$mode" == "fix" ]]; then
  dotnet format --fix-analyzers WebDownloadr.sln
  if ! git diff --quiet; then
    git add -A
    git commit -m "style: apply dotnet format and analyzer fixes"
  fi
else
  dotnet format --verify-no-changes --fix-analyzers WebDownloadr.sln
fi

git diff --exit-code
