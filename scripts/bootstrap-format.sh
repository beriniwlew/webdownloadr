#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

commit=false
if [[ "${1:-}" == "--commit" ]]; then
  commit=true
fi

dotnet restore WebDownloadr.sln

git config core.autocrlf true

git add --renormalize .

echo "🛠  Fixing whitespace / EOL…"
dotnet format whitespace WebDownloadr.sln

echo "🛠  Fixing analyzer & style rules…"
dotnet format analyzers WebDownloadr.sln

git add -A

if ! git diff --cached --quiet; then
  if $commit; then
    git commit -m "chore: normalize code formatting (CRLF, indentation, analyzers)"
    echo "✅ Normalization commit created."
  else
    echo "✅ Changes staged but not committed."
  fi
else
  echo "✅ Already clean — no commit needed."
fi

