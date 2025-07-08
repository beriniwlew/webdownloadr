#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

dotnet restore WebDownloadr.sln

# ---- VERIFY, no fixes ----
dotnet format whitespace WebDownloadr.sln --verify-no-changes

dotnet format analyzers WebDownloadr.sln --verify-no-changes

echo "✅ Source _matches_ .editorconfig and analyzers."
