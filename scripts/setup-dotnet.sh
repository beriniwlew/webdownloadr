#!/usr/bin/env bash
set -euo pipefail
IFS=$'\n\t'

#------------------------------------------------------------------------------
# 1) Core prerequisites (apt)
#------------------------------------------------------------------------------
sudo apt-get update
sudo apt-get install -y \
  curl wget gnupg ca-certificates lsb-release apt-transport-https

#------------------------------------------------------------------------------
# 2) .NET 9 SDK installation
#------------------------------------------------------------------------------
UBUNTU_VERSION="$(lsb_release -rs)"
if [[ "${UBUNTU_VERSION}" =~ ^(20\.04|22\.04)$ ]]; then
  echo "Registering Microsoft feed for Ubuntu ${UBUNTU_VERSION}..."
  wget -q \
    "https://packages.microsoft.com/config/ubuntu/${UBUNTU_VERSION}/packages-microsoft-prod.deb" \
    -O packages-microsoft-prod.deb
  sudo dpkg -i packages-microsoft-prod.deb
  rm packages-microsoft-prod.deb
  sudo apt-get update
  sudo apt-get install -y dotnet-sdk-9.0
else
  echo "Ubuntu ${UBUNTU_VERSION} not in Microsoft feed; using dotnet-install.sh..."
  curl -sSL https://dot.net/v1/dotnet-install.sh -o dotnet-install.sh
  chmod +x dotnet-install.sh
  ./dotnet-install.sh --channel 9.0
  rm dotnet-install.sh
fi

#------------------------------------------------------------------------------
# 3) Ensure dotnet is on PATH for *this session* (critical for CI & Codex)
#------------------------------------------------------------------------------
export DOTNET_ROOT="$HOME/.dotnet"
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
if ! grep -q '^export DOTNET_ROOT=' ~/.bashrc || \
   ! grep -q '\.dotnet/tools' ~/.bashrc; then
  cat <<'EOF_BASHRC' >> ~/.bashrc
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$DOTNET_ROOT/tools:$HOME/.local/bin:$PATH"
EOF_BASHRC
fi

echo "✅ .NET install script completed!"

#------------------------------------------------------------------------------
# 6) Optional: print PATH for debug (comment out if too noisy)
#------------------------------------------------------------------------------
# echo "PATH is now: $PATH"
