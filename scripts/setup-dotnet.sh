#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

#------------------------------------------------------------------------------
# 1) Core prerequisites (apt)
#------------------------------------------------------------------------------
apt-get update
apt-get install -y \
  curl wget gnupg ca-certificates lsb-release apt-transport-https software-properties-common

#------------------------------------------------------------------------------
# 2) .NET 9 SDK installation
#------------------------------------------------------------------------------
UBUNTU_VERSION="$(lsb_release -rs)"
echo "Registering Microsoft backports feed for Ubuntu ${UBUNTU_VERSION}..."
add-apt-repository -y ppa:dotnet/backports
apt-get update
apt-get install -y dotnet-sdk-9.0

#------------------------------------------------------------------------------
# 3) Ensure dotnet is on PATH for *this session* (critical for CI & Codex)
#------------------------------------------------------------------------------
export DOTNET_ROOT="/usr/lib/dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$HOME/.local/bin:$PATH"

#------------------------------------------------------------------------------
# 4) Test and report the .NET version; fail fast if not available
#------------------------------------------------------------------------------
if ! command -v dotnet >/dev/null; then
  echo "❌ dotnet still not found in PATH after install! Try opening a new shell or source your ~/.bashrc"
  exit 1
fi
echo "✅ .NET version: $(dotnet --version)"

#------------------------------------------------------------------------------
# 5) Persist env-vars for future shells (good for local use)
#------------------------------------------------------------------------------
if ! grep -q '^export DOTNET_ROOT=' ~/.bashrc || ! grep -q '/usr/lib/dotnet' ~/.bashrc; then
  cat <<'EOF_BASHRC' >> ~/.bashrc
export DOTNET_ROOT="/usr/lib/dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$HOME/.local/bin:$PATH"
EOF_BASHRC
fi

echo "✅ .NET install script completed!"

#------------------------------------------------------------------------------
# 6) Optional: print PATH for debug (comment out if too noisy)
#------------------------------------------------------------------------------
# echo "PATH is now: $PATH"
