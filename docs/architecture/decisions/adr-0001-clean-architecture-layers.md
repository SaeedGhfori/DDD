# ADR-0001 — Clean Architecture with layered services

- **Status:** Accepted
- **Date:** unknown
- **Related:** `AGENTS.md` §C, `docs/architecture/system-overview.md`

## Context

The solution must keep business rules independent from frameworks and
infrastructure, and remain testable. Without structure, business logic tends to
leak into endpoints and infrastructure.

## Decision

Each microservice uses Clean-Architecture-style layers: `Domain`,
`Application`, `Infrastructure`, `Persistence`, `Presentation`. Dependency
direction is `Presentation → Application → Domain`; `Infrastructure` and
`Persistence` depend on `Domain`/`Application`, never the reverse. `Domain` has
no project references.

## Alternatives considered

- Layered-by-technology (Controllers/Services/Repositories in one project) —
  rejected: encourages framework coupling and a weak domain.
- Single project per service — rejected: no enforceable boundary.

## Consequences and trade-offs

- Domain is framework-free and unit-testable.
- Dependency direction is enforceable by architecture tests.
- More projects and ceremony per service.

## Related documents or code

- `tests/Procurement/Procurement.ArchitectureTests/`
- `docs/architecture/system-overview.md`
