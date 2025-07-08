#!/usr/bin/env bash
set -euo pipefail

# Install global dotnet tools required for development

dotnet tool update -g dotnet-outdated-tool || dotnet tool install -g dotnet-outdated-tool

# ReportGenerator's official package ID is dotnet-reportgenerator-globaltool
dotnet tool update -g dotnet-reportgenerator-globaltool || dotnet tool install -g dotnet-reportgenerator-globaltool

