#!/usr/bin/env bash
set -euo pipefail

cd "$(dirname "$0")/.."

# Install .NET 9 SDK via dotnet/backports if not already present
if ! command -v dotnet >/dev/null || ! [[ $(dotnet --version) =~ ^9\. ]]; then
  apt-get update
  apt-get install -y software-properties-common
  add-apt-repository -y ppa:dotnet/backports
  apt-get update
  apt-get install -y dotnet-sdk-9.0
fi

export DOTNET_ROOT="/usr/lib/dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$HOME/.local/bin:$PATH"

if ! grep -q '^export DOTNET_ROOT=' ~/.bashrc; then
  cat <<'EOF' >> ~/.bashrc
export DOTNET_ROOT="/usr/lib/dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$HOME/.local/bin:$PATH"
EOF
fi

./scripts/install-tools.sh

echo "✅ Codex environment ready."
