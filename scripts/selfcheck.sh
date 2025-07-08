#!/usr/bin/env bash
set -euo pipefail

# Restore packages
 dotnet restore WebDownloadr.sln

# Build with warnings as errors
dotnet build --no-restore -warnaserror WebDownloadr.sln

# Run tests with coverage
 dotnet test --no-build --no-restore WebDownloadr.sln --collect:"XPlat Code Coverage" --results-directory ./TestResults

# Verify formatting
 dotnet format --verify-no-changes WebDownloadr.sln --no-restore

# Generate coverage report
 reportgenerator "-reports:TestResults/**/coverage.cobertura.xml" "-targetdir:TestResults/coverage-report" -reporttypes:HtmlSummary

