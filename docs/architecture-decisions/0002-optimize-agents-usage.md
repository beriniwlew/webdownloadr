# 0002 – Optimize AGENTS Usage Across Folders

**Status**: Proposed

## Context

This repository relies on `AGENTS.md` files to define contribution rules for humans and AI. The [root file](../../AGENTS.md) explains that these files can live in any subdirectory and that the most specific one overrides higher levels. Lines 86–103 outline known overrides such as `docs/architecture-decisions/` and remind contributors to always check for a local `AGENTS.md`.

Despite this guidance, some areas still lack clear inheritance patterns, leading to inconsistent rules for contributors and AI agents. Standardizing how these files are inherited will reduce confusion and ensure everyone follows the intended guidelines.

## Decision

Adopt a standardized approach to `AGENTS.md` inheritance. Every major folder must either contain its own `AGENTS.md` or explicitly inherit from its nearest parent. Documentation will be updated to clarify this hierarchy so contributors and AI agents know which file governs their work.

## Consequences

- **Improved Guidance**: Clearer rules in each folder help new contributors and automation follow repository standards.
- **Easier Maintenance**: Standard inheritance reduces duplication and simplifies updates across the project.
- **Consistent Agent Behavior**: AI tools can reliably determine applicable rules, minimizing errors due to mismatched instructions.

## Alternatives Considered

- Continue using only the root `AGENTS.md` and a few overrides, risking inconsistent guidance.
- Centralize everything in one file, which would be harder to maintain and less flexible.

## References

- Root `AGENTS.md` lines 86–103 show the current inheritance guidance.

