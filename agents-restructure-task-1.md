# Agents Restructure Task 1

The **WebDownloadr** repo already includes an extensive `AGENTS.md` (at the root) as well as layer-specific guides under
`src/WebDownloadr.Core`, `UseCases`, and `Infrastructure`. To make it even more helpful, consider adding a brief **Project Overview**
section at the top of `AGENTS.md` that summarizes the purpose (“a clean‐architecture .NET 9 web page downloader” or similar) and the tech
stack (C# 12, .NET 9, FastEndpoints, EF Core, xUnit, etc.). This helps orient both human developers and AI agents. For example, many
AGENTS.md files list languages, frameworks, and key tools up
front[openai.com](https://openai.com/index/introducing-codex/#:~:text=Codex%20can%20be%20guided%20by,testing%20setups%2C%20and%20clear%20documentation)[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=Common%20Commands%20and%20Scripts).
(Our existing YAML front matter already notes “C# 12 (.NET 9)” and “Clean
Architecture”[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L2-L10), but a
one‐paragraph summary in prose can make this clearer to readers and agents alike.)

## Essential Commands

Standard guidance is to include an **Essential Commands** section with code blocks for build/test/lint steps. For example, an AGENTS.md
might list commands like:

```bash
dotnet restore
dotnet build
dotnet test
dotnet format --verify-no-changes
npm run markdownlint
npm run prettier --check
```

Our repo already uses a `scripts/selfcheck.sh` to run these (see **Build, Test & Format**
section)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L810-L819), but
surfacing the key commands in a concise list helps agents. For instance, Tembo’s docs show a “Common Commands” list (e.g. `npm run build`,
`npm run test`, `npm run lint`)[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=Common%20Commands%20and%20Scripts). We
should similarly **outline** our setup: e.g. installing dependencies, running the Web project, running all tests, etc. This could mirror the
README’s “Getting Started” steps (e.g. `dotnet run --project src/WebDownloadr.Web/...`) plus the CI steps from
`selfcheck.sh`[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L814-L822).

## Development & Workflow Guidelines

Add a **Development Workflow** or **Conventions** section in `AGENTS.md`. This should spell out how to prepare a change and PR. We already
mention Conventional Commits and branch prefixes in
YAML[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L22-L30), but it’s good to
repeat in text (e.g. “Use `type(scope): summary` commit messages, reference issues as Fixes #N”). We also should call out running
`./scripts/selfcheck.sh` (or the equivalent CI pipeline) before pushing, as our Quick Reference
does[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L38-L41)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L70-L78).
A bullet list like “1) Code, 2) Self-check (build/test/format) locally, 3) Commit with `feat/usecases: ...`, 4) Open a PR with description
referencing ADRs or issues” would align with the Prefect example (which instructs agents to always include title/description in
PR)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L60-L68). Also mention **PR
conventions**: e.g. “Always include a clear title/description in imperative form, reference issues with `Fixes #N`, and fill in the PR
checklist”[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L38-L41)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L70-L78).

## Code Quality & Formatting

Our existing `AGENTS.md` already details editorconfig, analyzers, linters,
etc[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L852-L861)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L867-L876).
Ensure there’s a summary of these under something like “Code Formatting” or “Linting”. We should explicitly tell agents to run
`dotnet format` and our `scripts/format.sh`, as well as `markdownlint-cli2` and `prettier` on
docs[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L867-L876). For example, the
Trunk docs suggest adding a “Formatting and Linting” section so Codex knows to run those tools after
edits[docs.trunk.io](https://docs.trunk.io/code-quality/setup-and-installation/openai-codex-support#:~:text=). We could add an “AGENTS
Instructions” snippet (similar to Trunk’s example) that says e.g. “Run `dotnet format`, `dotnet format --verify-no-changes`,
`npx markdownlint-cli2`, and `npx prettier` to enforce
formatting”[docs.trunk.io](https://docs.trunk.io/code-quality/setup-and-installation/openai-codex-support#:~:text=). This makes our
intentions explicit to any AI/CI.

Likewise, it’s worth highlighting our **analyzers and styling rules**. We note in AGENTS.md that all analyzers (Microsoft and StyleCop) are
treated as errors[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L900-L908). We
should ensure we list any special style decisions (e.g. “file-scoped namespaces, nullable enabled”) so that agents generate code
accordingly. For readability, bullet-point lists of “Code Style” rules (indentation, line endings, naming, etc.) can help; Tembo’s “Code
Style and Conventions” example shows this
format[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=Code%20Style%20and%20Conventions).

## Testing Guidelines

Add a clear **Testing** section. Our root AGENTS.md already mandates ≥90% coverage and refers to
coverlet/reportgenerator[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L18-L21)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L830-L838).
Emphasize _how_ to run tests (e.g. `dotnet test --collect:"XPlat Code Coverage"` as in `selfcheck.sh`) and our threshold policy (fail CI if
<90%). Include the statement that new features must include matching unit tests (as our Tests section
hints[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L106-L112)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.UseCases/AGENTS.md#L148-L153)).
We should also note the test framework (xUnit), mocking libraries (NSubstitute, Shouldly) etc. For example: “Write xUnit tests for all
business logic; use NSubstitute and Shouldly for mocks/assertions; aim for ≥90% coverage; include integration or functional tests for any
new
feature”[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L106-L112)[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=%23%20Testing%20guidelines%20,Include%20error%20handling%20test%20cases).
Agents will then know both _that_ to test and _how_.

## Nested `AGENTS.md` Files

We already follow the layered approach: there are nested AGENTS.md under Core, UseCases, and Infrastructure that “inherit” the root
file[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L2-L11)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.UseCases/AGENTS.md#L2-L10).
This is great and in line with best practices. One improvement: **add a nested AGENTS.md in the Web project** (e.g.
`src/WebDownloadr.Web/AGENTS.md`) and perhaps in `Tests/`, to document any UI/API-specific or test conventions. The templates suggest using
nested files to _tighten_ or _relax_ rules per layer. For example, a Web-specific AGENTS.md could say “Endpoints should never call
Infrastructure directly (use handlers), always map domain to DTOs for APIs, use FastEndpoints’ validation”. A Tests AGENTS.md could note
that test code can use underscores in names or exclude itself from coverage stats. By doing this, we mirror our Core/UseCases examples
(which set strict boundaries and naming
conventions[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L16-L25)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.UseCases/AGENTS.md#L11-L15)).

## Pull Request & Commit Conventions

Explicitly state our **commit message and PR format** rules. We already require Conventional Commits and have a commitlint config
template[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L14-L20). Remind
contributors (and codex) to use, e.g. `type(scope): Summary`. A bullet like “Use `feat/usecases: ...`, `fix/core: ...` etc., and ensure
commit messages and branch names match the `core/usecases/...`
scopes[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L14-L20)” would be
helpful. This aligns with how other AGENTS.md examples list commit/PR practices. We can cite that Prefect’s AGENTS.md instructs agents to
use imperative titles and close issues (e.g. “Always include a clear title, description, and use imperative
mood”)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L60-L68). Also mention any
PR description expectations (e.g. summarizing changes, linking ADRs).

## CI/CD and Automation

Incorporate guidance about automating these rules. For example, teams often use tools like **Ruler** or Codex CLI in CI to enforce AGENTS.md
compliance. We can suggest adding a CI step to run `ruler apply --no-gitignore` and fail if AGENTS.md (or nested instructions)
drift[github.com](https://github.com/intellectronica/ruler#:~:text=Q%3A%20Can%20I%20run%20Ruler,the%20GitHub%20Actions%20example%20above).
This ensures the agent guidance stays up-to-date. Likewise, we might run Codex CLI in a “full-auto” mode on PRs to auto-apply fixes (as
Codex docs
describe)[openai.com](https://openai.com/index/introducing-codex/#:~:text=Codex%20can%20be%20guided%20by,testing%20setups%2C%20and%20clear%20documentation).
Even if not fully automated yet, it’s worth noting that our `scripts/selfcheck.sh` could be run via GitHub Actions before merges, and that
`AGENTS.md` itself should be version-controlled and updated via PRs.

## Context & Examples

Finally, include references to **example content**. Our `AGENTS.md` already contains code-pattern snippets (e.g. entity with guard clauses)
to show idiomatic
styles[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/AGENTS.md#L924-L933)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L40-L49)
– this is excellent. We should ensure any new sections similarly use examples or links. For instance, linking to Clean Architecture
(Ardalis) docs or showing a model handler snippet can help agents copy patterns. We can cite Tembo’s example that Clean Architecture itself
is an explicit
guideline[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=,components%20focused%20on%20single%20responsibilities). Also,
providing an example of a conventional commit in AGENTS.md (like “feat(usecases): implement download queue”) can reinforce practice.

**In summary**, the repo has a solid foundation (including an AGENTS.md with many rules) but could be improved by reorganizing and adding
the common AGENTS.md sections described above. In particular, adding a succinct project overview/tech stack, an “Essential Commands”
snippet, clearer testing/coverage guidelines, and explicit PR/commit rules will make the guidance more discoverable. Ensuring each layer
(including Web and Tests) has its own rule file will tighten architecture
boundaries[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.Core/AGENTS.md#L16-L25)[GitHub](https://github.com/beriniwlew/webdownloadr/blob/ab8efe333f9e567a518cc02d56d7431c2698eb1d/src/WebDownloadr.UseCases/AGENTS.md#L11-L15).
Finally, integrating tools like Ruler in CI (and citing examples from Codex or Tembo docs) will help keep the guidance
enforced[github.com](https://github.com/intellectronica/ruler#:~:text=Q%3A%20Can%20I%20run%20Ruler,the%20GitHub%20Actions%20example%20above)[docs.trunk.io](https://docs.trunk.io/code-quality/setup-and-installation/openai-codex-support#:~:text=).

**Sources:** We followed best-practice templates from official AGENTS.md examples and tooling docs (e.g. Prefect/Torus AGENTS.md patterns,
Tembo’s rule-files documentation, OpenAI Codex blog) to draft these
suggestions[openai.com](https://openai.com/index/introducing-codex/#:~:text=Codex%20can%20be%20guided%20by,testing%20setups%2C%20and%20clear%20documentation)[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=Common%20Commands%20and%20Scripts)[docs.tembo.io](https://docs.tembo.io/features/rule-files#:~:text=,components%20focused%20on%20single%20responsibilities)[docs.trunk.io](https://docs.trunk.io/code-quality/setup-and-installation/openai-codex-support#:~:text=)[github.com](https://github.com/intellectronica/ruler#:~:text=Q%3A%20Can%20I%20run%20Ruler,the%20GitHub%20Actions%20example%20above).

Introducing Codex | OpenAI

<https://openai.com/index/introducing-codex/>

Rule Files - Tembo Docs

<https://docs.tembo.io/features/rule-files>
