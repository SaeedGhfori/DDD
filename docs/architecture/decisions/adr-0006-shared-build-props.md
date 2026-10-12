# ADR-0006 — Shared build settings via `Directory.Build.props`

- **Status:** Accepted
- **Date:** unknown
- **Related:** `Directory.Build.props`, `AGENTS.md` §E

## Context

Every project needs the same target framework and compiler settings; repeating
them invites inconsistency.

## Decision

Define common build properties once in `Directory.Build.props`
(`TargetFramework`, `Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`,
`NoWarn`).

## Alternatives considered

- Repeat settings per project — rejected: duplication and drift.

## Consequences and trade-offs

- Consistent build across all projects.
- A change here affects the whole solution (intended).

## Related documents or code

- `Directory.Build.props`, `global.json`
