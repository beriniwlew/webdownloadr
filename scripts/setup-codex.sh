#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

# Source setup-dotnet so PATH updates propagate
source ./scripts/setup-dotnet.sh
./scripts/install-tools.sh

echo "✅ Codex environment ready."
