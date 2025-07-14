# Maintainers

This document lists the current maintainers of the WebDownloadr project and provides guidelines for contributors and AI agents.

## Core Maintainers

### Project Lead
- **Primary Contact**: Berin Iwlew
- **GitHub**: [@beriniwlew](https://github.com/beriniwlew)
- **Responsibilities**: Architecture decisions, major feature reviews, release management


## Maintainer Guidelines

### Review Responsibilities
- **Core Maintainers**: Review all architectural changes, breaking changes, and major features
- **Technical Maintainers**: Review changes in their respective areas
- **All Maintainers**: Review documentation updates, dependency updates, and security patches

### Response Times
- **Critical Issues**: 24 hours
- **Feature Requests**: 1 week
- **Documentation**: 1 week
- **AI Agent Escalations**: 48 hours

### Escalation Process
1. **AI Agent Escalations**: Create GitHub Issue with `ai-agent-escalation` label
2. **Architecture Decisions**: Follow ADR process in `docs/architecture-decisions/`
3. **Security Issues**: Create private security issue or contact maintainers directly
4. **Breaking Changes**: Require approval from at least 2 core maintainers

## Contact Information

### For Contributors
- **General Questions**: Create GitHub Issue with appropriate label
- **Code Reviews**: Request review from relevant technical maintainer
- **Architecture Questions**: Use ADR process or tag core maintainers

### For AI Agents
- **Escalations**: Create issue with `ai-agent-escalation` label
- **Uncertain Rules**: Tag maintainers in issue description
- **Architecture Decisions**: Follow ADR process


## Maintainer Onboarding

### New Maintainer Checklist
- [ ] Read and understand AGENTS.md
- [ ] Review architecture decisions in `docs/architecture-decisions/`
- [ ] Set up development environment using scripts in `/scripts`
- [ ] Run full test suite and understand CI/CD pipeline
- [ ] Review recent PRs to understand review standards
- [ ] Join maintainer communication channels

### Maintainer Offboarding
- [ ] Transfer ownership of assigned areas
- [ ] Update contact information in this file
- [ ] Archive or transfer personal repositories
- [ ] Update CI/CD access and secrets

## Decision Making

### Consensus Process
- **Minor Changes**: Single maintainer approval
- **Feature Additions**: Two maintainer approvals
- **Breaking Changes**: All core maintainer approvals
- **Architecture Changes**: ADR process + core maintainer approval

### Conflict Resolution
- **Technical Disputes**: Architecture decision record (ADR)
- **Process Disputes**: Core maintainer discussion
- **Escalation**: Project lead makes final decision

---

> **Note**: This file should be updated whenever maintainer information changes. All maintainers should have access to update this file. 
