#!/bin/bash

# ADR Validation Script
# Checks that all ADR files follow the required format

set -e

ADR_DIR="docs/architecture-decisions"
ERRORS=0

echo "🔍 Validating ADR files in $ADR_DIR..."

# Check each ADR file
for file in "$ADR_DIR"/[0-9][0-9][0-9][0-9]-*.md; do
    if [[ -f "$file" ]]; then
        echo "Checking $file..."
        
        # Check for required sections
        if ! grep -q "^Status:" "$file"; then
            echo "❌ ERROR: $file missing Status field"
            ((ERRORS++))
        fi
        
        if ! grep -q "^Date:" "$file"; then
            echo "❌ ERROR: $file missing Date field"
            ((ERRORS++))
        fi
        
        if ! grep -q "^## Context" "$file"; then
            echo "❌ ERROR: $file missing Context section"
            ((ERRORS++))
        fi
        
        if ! grep -q "^## Decision" "$file"; then
            echo "❌ ERROR: $file missing Decision section"
            ((ERRORS++))
        fi
        
        if ! grep -q "^## Consequences" "$file"; then
            echo "❌ ERROR: $file missing Consequences section"
            ((ERRORS++))
        fi
        
        # Check for proper numbering in filename
        filename=$(basename "$file")
        if ! [[ "$filename" =~ ^[0-9]{4}- ]]; then
            echo "❌ ERROR: $file has incorrect numbering format"
            ((ERRORS++))
        fi
    fi
done

# Check for files that don't follow naming convention
for file in "$ADR_DIR"/*.md; do
    if [[ -f "$file" ]]; then
        filename=$(basename "$file")
        if [[ "$filename" != "AGENTS.md" && "$filename" != "README.md" && "$filename" != "0000-template.md" ]]; then
            if ! [[ "$filename" =~ ^[0-9]{4}- ]]; then
                echo "❌ ERROR: $file doesn't follow ADR naming convention"
                ((ERRORS++))
            fi
        fi
    fi
done

if [[ $ERRORS -eq 0 ]]; then
    echo "✅ All ADR files are valid!"
    exit 0
else
    echo "❌ Found $ERRORS validation errors"
    exit 1
fi 