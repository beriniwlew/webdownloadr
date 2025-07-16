#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

# Apply .NET formatting fixes
 dotnet format WebDownloadr.sln

# Format shell scripts in-place
shfmt -i 2 -w $(git ls-files '*.sh')

# Format Markdown, JSON, and YAML files using Prettier
npx --yes prettier --write --ignore-path .prettierignore "**/*.{md,json,ya?ml}"
