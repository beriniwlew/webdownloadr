#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

./scripts/setup-dotnet.sh
./scripts/install-tools.sh

echo "✅ Codex environment ready."
