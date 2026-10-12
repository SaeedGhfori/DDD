# AGENTS.md — Working instructions for AI coding agents

This is the primary entry point for any AI coding agent working in this
repository. Read it first, then read only the linked documents relevant to your
task. Keep changes minimal and evidence-based.

---

## A. Project Identity

- **Project name:** `DDD`
- **Business purpose:** Mini Procurement — employees create purchase requests,
  a department manager approves / rejects / returns them for correction.
- **Solution file:** `DDD.slnx` (there is **no** classic `DDD.sln`).
- **Target framework:** `net10.0`; **SDK pinned:** `10.0.400` (`global.json`,
  `rollForward: latestMinor`).
- **Stack (verified):** .NET 10, ASP.NET Core Minimal APIs, YARP reverse proxy,
  EF Core + Npgsql (referenced by the Persistence projects only, not yet used),
  xUnit + FluentAssertions for tests.
- **Structure:** two independent microservices (`Procurement`, `Notification`),
  an API gateway (`DDD.ApiGateway`), and a contracts project (`DDD.Contracts`).
- **Current implementation status:** see `docs/architecture/current-state.md`.
  In short: `Procurement` has a Domain model + one end-to-end use case; most
  other capabilities are scaffolding only.

Always distinguish **implemented** from **planned**. Do not describe planned
features as if they exist.

## B. Mandatory Agent Workflow

1. Inspect the relevant files and `git status` **before** changing anything.
2. Read this file, then **only** the linked documents that apply to the task
   (see §J); do not reread all docs, historical reports, or unrelated source.
3. Inspect existing architecture and business decisions before proposing designs
   (`docs/architecture/system-overview.md`, `docs/architecture/decisions/`).
4. Identify the actual project and layer affected.
5. Make the smallest justified change (see §G).
6. Preserve unrelated user changes; never discard user work or run destructive
   Git commands without explicit authorization.
7. Do not duplicate existing functionality or documentation.
8. Add or update tests for behaviour changes.
9. Run relevant validation commands when possible (see §F).
10. Report actual results and remaining limitations honestly (see §I).

Never claim to have inspected, edited, built, tested, or verified something you
did not actually do.

## C. Architecture Rules

Intended structure: `Procurement` and `Notification` microservices, each with
Clean-Architecture-style layers, plus `DDD.ApiGateway` and `DDD.Contracts`.

Intended dependency direction within a service:

```
Presentation → Application → Domain
Infrastructure ─┐
Persistence ────┴→ (Domain / Application abstractions)
```

**Verify actual project references** (`.csproj`) instead of trusting this
diagram. The rules below are enforced today by
`tests/Procurement/Procurement.ArchitectureTests`.

Prohibited:

- Domain depending on ASP.NET Core, EF Core, PostgreSQL providers, or YARP.
- Domain-to-Domain references across microservices.
- Business rules implemented only in HTTP endpoints (the "Smart UI"
  anti-pattern).
- Treating Presentation DTOs automatically as Domain models.
- Direct cross-service database access.
- Sharing Domain entities across microservices.
- Abstractions/interfaces created without a real requirement.
- Large, unrelated refactors.
- Introducing patterns only to look sophisticated (Domain Events, Specification,
  Factory, State pattern, etc.) without a justified need.

## D. Business Rule Protection

Business rules are canonical in `docs/domain/business-rules.md`; vocabulary is
canonical in `docs/domain/ubiquitous-language.md`.

If a requested change conflicts with a documented rule:

1. Identify the conflict.
2. Explain its impact.
3. Check whether the rule is **Confirmed**, an **Assumption**, or an **Open
   Question**.
4. Do not silently change the rule.
5. Ask for clarification when a material business decision is unresolved.

## E. Technology and Dependency Policy

- Target framework and SDK are fixed by `Directory.Build.props` and `global.json`.
- **Central Package Management is enabled** (`Directory.Packages.props`). Add
  versions there; `PackageReference` entries in `.csproj` must have **no**
  `Version` attribute.
- Do not introduce a new library when platform capabilities suffice.
- Do not upgrade unrelated dependencies as part of ordinary work.
- Verify compatibility before adding or changing dependencies.
- `Nullable` and `ImplicitUsings` are enabled and `TreatWarningsAsErrors` is on;
  keep the build warning-free.
- Tests use xUnit + FluentAssertions. Follow existing naming: descriptive method
  names, `sealed` test classes, arrange/act/assert.

## F. Testing and Validation

Standard validation from the repository root (actual solution file):

```powershell
dotnet restore DDD.slnx
dotnet build   DDD.slnx -c Debug
dotnet test    DDD.slnx
```

- Run targeted tests while developing; run solution-wide validation when the
  scope and risk of the change warrant it.
- Do **not** rerun unchanged, already-successful checks without a reason; rerun
  the affected checks after any change that could invalidate their results.
- Do not skip required checks merely to save time or tokens.
- Report warnings, errors, skipped tests, and environmental blockers honestly.
- **Never present an unexecuted command as successful.**
- Distinguish test categories:
  - Unit tests — `Procurement.Domain.Tests`, `Procurement.Application.Tests`
  - Integration tests — `Procurement.IntegrationTests`
  - Architecture tests — `Procurement.ArchitectureTests`
- Do not weaken or delete tests merely to get a green build.

## G. Scope and Implementation Discipline

Implement incrementally. Do **not** implement every technology on the roadmap.
RabbitMQ, MassTransit, Outbox/Inbox, Saga, cross-service communication,
authentication, persistence, observability and similar capabilities must be
introduced only when the active task requires them **and** an applicable ADR
exists. A technology being "planned" does not mean it is implemented or should
be added now.

- For small, well-defined changes: inspect, implement and validate directly.
- For large or cross-cutting changes: write a concise plan before editing.
- Keep the task within its explicit scope.
- Record unrelated findings as follow-up work instead of implementing them.
- Ask for clarification when an unresolved business decision materially affects
  correctness.
- Avoid unrelated refactoring, formatting churn, dependency upgrades and
  speculative abstractions.

## H. Documentation Maintenance

Update the canonical document when a meaningful change affects it:

| Change | Document |
| --- | --- |
| Business rule | `docs/domain/business-rules.md` |
| Domain vocabulary | `docs/domain/ubiquitous-language.md` |
| Architectural decision | `docs/architecture/decisions/` (new ADR) |
| Capability added/removed | `docs/architecture/current-state.md` |
| Validation or audit | corresponding report under `docs/architecture/` |

- Maintain **one canonical source** for each important fact; link to it instead
  of duplicating content.
- Update documentation only when a material fact, decision, behaviour or project
  status changes — not after trivial changes.
- Keep this file (`AGENTS.md`) concise and focused on stable instructions; keep
  volatile implementation status in `docs/architecture/current-state.md`.
- Never create a new summary document for information already maintained
  canonically elsewhere.
- Never claim that a tool automatically loads a document unless that is verified
  for the actual environment.

## I. Final Response Format

Conclude each implementation task with:

1. Summary of changes.
2. Files changed.
3. Relevant design decisions.
4. Tests and commands actually executed.
5. Actual build and test results.
6. Known limitations.
7. Documentation updated.
8. Remaining work / unresolved questions.

Keep the report concise: summarize changed files, key decisions, actual
validation results and remaining risks. Do **not** repeat entire source files or
long documentation. Include detailed logs only when needed to diagnose a failure.

---

## J. Context and Output Efficiency

Context loading:

- Treat this file as the main entry point; read only the linked documents that
  are relevant to the current task.
- Prefer targeted searches and relevant file sections over dumping whole files.
- Reuse information already verified earlier in the same session while it stays
  valid; recheck facts when source changes, conflicts or uncertainty make prior
  knowledge unreliable.
- Do not create a new summary document for information already maintained
  canonically elsewhere.

Output efficiency:

- Keep progress messages and final reports concise (see §I).
- Report outcomes, not transcripts.

## K. Quality Guardrails

Efficiency must never override:

- Business-rule correctness (§D).
- Clean Architecture boundaries (§C).
- Security.
- Meaningful test coverage (§F).
- Preservation of user work (§B).
- Honest reporting (§I).
- Required review of the relevant existing code (§B).

---

## Document map

- `docs/architecture/system-overview.md` — context, components, boundaries.
- `docs/architecture/current-state.md` — what exists today vs planned.
- `docs/architecture/decisions/` — ADRs (canonical decisions).
- `docs/domain/business-rules.md` — canonical business rules.
- `docs/domain/ubiquitous-language.md` — canonical vocabulary.
- `docs/architecture/domain-model-v0.2.md` — domain model detail.
- `docs/architecture/chat-01-final-review.md`, `docs/architecture/chat-01-audit-report.md` — verification reports.
- `README.md` — human entry point.

> Note: this file is named `AGENTS.md` because that is the shared convention the
> current environment uses to load repository guidance. Do not create duplicate,
> conflicting instruction files.
