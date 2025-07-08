#!/usr/bin/env bash
set -euo pipefail

if [[ "${1:-}" == "--fix" ]]; then
  dotnet format WebDownloadr.sln
  if ! git diff --quiet; then
    git add -A
    git commit -m "style: apply dotnet format"
  fi
else
  dotnet format --verify-no-changes WebDownloadr.sln
fi

git diff --exit-code
