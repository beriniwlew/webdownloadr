#!/usr/bin/env bash
set -euo pipefail

dotnet test ./tests/WebDownloadr.ArchTests --no-build --verbosity quiet
