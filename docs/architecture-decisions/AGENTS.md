# AGENTS.md – Architecture Decision Records (ADR)

> **Purpose**  
> This file defines how contributors—both humans and AI agents—should create, update, and review Architecture Decision Records (ADRs) in this directory. It ensures consistency, traceability, and clarity for all architectural decisions in the project.

---

## Table of Contents

1. [Scope](#1-scope)
2. [ADR Structure & Conventions](#2-adr-structure--conventions)
3. [Contribution & Review Workflow](#3-contribution--review-workflow)
4. [AI Agent Rules 🤖](#4-ai-agent-rules-)
5. [Examples](#5-examples)
6. [Maintenance & Improvement](#6-maintenance--improvement)
7. [Common Pitfalls & Best Practices](#7-common-pitfalls--best-practices)
8. [Glossary](#8-glossary)

---

## 1. Scope

- This AGENTS.md applies **only** to the `docs/architecture-decisions` directory, which contains all ADRs for this repository.

---

## 2. ADR Structure & Conventions

- **Format:**
  - All ADRs must follow [Michael Nygard’s template](https://github.com/joelparkerhenderson/architecture-decision-record/tree/main/locales/en/templates/decision-record-template-by-michael-nygard) (status, context, decision, consequences, etc).
  - Use Markdown files, named sequentially: `0001-title.md`, `0002-another-decision.md`, etc.
- **File Naming:**
  - Use all lowercase, words separated by hyphens.
  - Prefix with a 4-digit ADR number (e.g., `0003-use-fastendpoints.md`).
- **Status:**
  - Each ADR must have a `Status:` field (`Proposed`, `Accepted`, `Superseded`, `Deprecated`, `Rejected`).

---

## 3. Contribution & Review Workflow

- **Proposing an ADR:**
  - Add a new Markdown file as described above.
  - Reference related issues or discussions in the ADR.
  - Submit as a Pull Request (PR) targeting the main branch.
- **Review & Acceptance:**
  - All ADRs must be reviewed by at least one core maintainer before acceptance.
  - Status should be updated to `Accepted` or `Rejected` as appropriate during review.
- **Superseding or Updating ADRs:**
  - When an ADR is replaced, clearly mark it as `Superseded` and reference the new ADR number.

---

## 4. AI Agent Rules 🤖

- **Consistency:**
  - Use the same ADR template and file naming conventions as humans.
- **Traceability:**
  - Reference the context or discussions that justify the decision.
- **Self-Validation:**
  - AI agents must check that all required ADR sections are present and complete.
- **Escalation:**
  - If requirements are unclear or context is missing, propose an ADR draft and tag a maintainer for review.
- **Improvement Recommendations:**
  - AI agents should recommend updates to this AGENTS.md if better practices or automation can be identified (see [Section 6](#6-maintenance--improvement)).
- **Output Format:**
  - All ADRs and communications must be in valid Markdown, ready for review and commit.
- **Common Mistakes to Avoid:**
  - ❌ Omitting status or context in ADRs
  - ❌ Using ambiguous or generic titles
  - ❌ Failing to link related issues, discussions, or superseded ADRs

---

## 5. Examples

**Filename:**  
`0004-adopt-aspire-for-service-orchestration.md`

**Header Example:**

```markdown
# 0004: Adopt .NET Aspire for Service Orchestration

Status: Accepted  
Date: 2025-07-11  
Supersedes: N/A  
Superseded-by: N/A

## Context
...

## Decision
...

## Consequences
...
```

---

## 6. Maintenance & Improvement

- Update this AGENTS.md whenever ADR standards or workflows change.
- AI agents are encouraged to suggest improvements to this file as the project evolves. Propose such changes as PRs or draft issues for human review.

---

## 7. Common Pitfalls & Best Practices

**Pitfalls:**
- Incomplete ADR sections (missing status, context, or consequences)
- Inconsistent file naming or numbering
- Lack of references to relevant issues or ADRs
- Vague or non-actionable decisions

**Best Practices:**
- Use precise, descriptive titles
- Keep one decision per ADR
- Clearly state trade-offs and alternatives
- Regularly review and supersede outdated ADRs

---

## 8. Glossary

- **ADR:** Architecture Decision Record
- **Status:** The current state of an ADR (e.g., Proposed, Accepted)
- **Supersedes/Superseded-by:** Links between ADRs when one replaces another
- **Escalation:** The process by which an AI agent requests human intervention or review

---

> **Reminder:** AGENTS.md in this directory takes precedence over the root AGENTS.md for ADR-related contributions.
