#!/usr/bin/env bash
set -euo pipefail

USAGE="Usage: $0 [--skip-restore] [--skip-build] [--skip-test] [--skip-arch] [--skip-format] [--skip-docs] [--skip-commitlint]"

SKIP_RESTORE=false
SKIP_BUILD=false
SKIP_TEST=false
SKIP_ARCH=false
SKIP_FORMAT=false
SKIP_DOCS=false
SKIP_COMMITLINT=false

while [[ $# -gt 0 ]]; do
  case "$1" in
    --skip-restore) SKIP_RESTORE=true ;;
    --skip-build) SKIP_BUILD=true ;;
    --skip-test) SKIP_TEST=true ;;
    --skip-arch) SKIP_ARCH=true ;;
    --skip-format) SKIP_FORMAT=true ;;
    --skip-docs) SKIP_DOCS=true ;;
    --skip-commitlint) SKIP_COMMITLINT=true ;;
    -h|--help) echo "$USAGE"; exit 0 ;;
    *) echo "Unknown option: $1"; echo "$USAGE"; exit 1 ;;
  esac
  shift
done

PROJECT=WebDownloadr.sln

if ! $SKIP_RESTORE; then
  dotnet restore "$PROJECT"
fi

if ! $SKIP_BUILD; then
  dotnet build --no-restore -warnaserror "$PROJECT"
fi

if ! $SKIP_TEST; then
  dotnet test --no-build --no-restore "$PROJECT"
fi

if ! $SKIP_ARCH; then
  ./scripts/archtest.sh
fi

if ! $SKIP_FORMAT; then
  dotnet format "$PROJECT" --verify-no-changes --no-restore
  dotnet format "$PROJECT" analyzers --verify-no-changes --no-restore
fi

if ! $SKIP_DOCS; then
  npx --yes markdownlint-cli2 "**/*.md" "#node_modules"   # ignore via '#' glob
  npx --yes prettier --check --ignore-path .prettierignore "**/*.{md,json}"
fi

if ! $SKIP_COMMITLINT; then
  npm install --no-save @commitlint/config-conventional
  npx --yes commitlint --from HEAD~1
fi
