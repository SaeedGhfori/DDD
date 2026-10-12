# ADR-0009 — Warnings as errors, nullable + implicit usings

- **Status:** Accepted
- **Date:** unknown
- **Related:** `Directory.Build.props`, `AGENTS.md` §E

## Context

A higher quality baseline reduces latent defects.

## Decision

Enable `TreatWarningsAsErrors`, `Nullable`, and `ImplicitUsings` solution-wide via
`Directory.Build.props`. The build must remain warning-free.

## Alternatives considered

- Default (warnings only) — rejected: allows warnings to accumulate.

## Consequences and trade-offs

- Stricter, cleaner code.
- Style/analyzer warnings can block the build until fixed.

## Related documents or code

- `Directory.Build.props`
