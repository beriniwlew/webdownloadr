#!/usr/bin/env bash
set -euo pipefail

commit=false
if [[ "${1:-}" == "--commit" ]]; then
  commit=true
fi

# Ensure git respects .editorconfig line endings
git config core.autocrlf true

# Restore packages before formatting
dotnet restore WebDownloadr.sln

# Normalize all tracked files
git add --renormalize .

# Format the solution
dotnet format WebDownloadr.sln
dotnet format analyzers WebDownloadr.sln

# Show a summary of changes
echo "\nChanged files:"

git status --short

git diff --stat

if $commit; then
  if ! git diff --cached --quiet || ! git diff --quiet; then
    git add -A
    git commit -m "style: normalize line endings and encoding to match .editorconfig"
  else
    echo "No changes to commit."
  fi
fi

if ! git diff --quiet || ! git diff --cached --quiet; then
  echo "\n❌ Uncommitted changes remain."
  exit 1
fi

echo "\n✔ Repository formatted and normalized."
