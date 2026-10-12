# ADR-0005 — Central Package Management

- **Status:** Accepted
- **Date:** unknown
- **Related:** `Directory.Packages.props`, `AGENTS.md` §E

## Context

Multiple projects consume shared packages; per-project versions drift and cause
inconsistent builds.

## Decision

Enable Central Package Management. All package versions are declared once in
`Directory.Packages.props`; `PackageReference` entries in `.csproj` must have no
`Version` attribute.

## Alternatives considered

- Per-project versions — rejected: drift and duplication.

## Consequences and trade-offs

- Single source of truth for versions.
- Contributors must add versions centrally.

## Related documents or code

- `Directory.Packages.props`
