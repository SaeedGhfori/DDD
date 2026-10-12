# ADR-0003 — `DDD.Contracts` for integration contracts only

- **Status:** Accepted
- **Date:** unknown
- **Related:** `AGENTS.md` §C

## Context

Cross-service communication needs shared message shapes, but sharing domain
models would couple services.

## Decision

`DDD.Contracts` is reserved for integration contracts (messages/events) only. It
must not contain domain entities or business logic. It is currently empty.

## Alternatives considered

- Put shared entities in `DDD.Contracts` — rejected: violates service boundaries.

## Consequences and trade-offs

- Clear separation between integration contracts and domain models.
- Requires discipline to keep it free of domain logic.

## Related documents or code

- `src/BuildingBlocks/DDD.Contracts/`
