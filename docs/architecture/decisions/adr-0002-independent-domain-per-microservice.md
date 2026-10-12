# ADR-0002 — One independent domain per microservice

- **Status:** Accepted
- **Date:** unknown
- **Related:** `AGENTS.md` §C

## Context

Sharing domain entities across services couples their release cycles and
reintroduces a distributed monolith.

## Decision

Each microservice owns its own domain. There is **no shared domain project** and
**no Domain-to-Domain reference** between `Procurement` and `Notification`.

## Alternatives considered

- Shared `DDD.Domain` library — rejected: couples services and hides boundaries.

## Consequences and trade-offs

- Services evolve independently.
- Some concepts (e.g. Employee/Department) are referenced by id only.

## Related documents or code

- `docs/architecture/system-overview.md`
