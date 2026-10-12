# ADR-0004 — ASP.NET Core Minimal APIs for presentation

- **Status:** Accepted
- **Date:** unknown
- **Related:** `src/Services/Procurement/Procurement.Presentation/`

## Context

Services need a lightweight HTTP host with an explicit composition root and
minimal ceremony.

## Decision

Use ASP.NET Core Minimal APIs. The Presentation project is the composition root
(DI) and delegates business operations to Application/Domain.

## Alternatives considered

- MVC controllers — rejected: more ceremony than needed.
- Custom HTTP host — rejected: no benefit.

## Consequences and trade-offs

- Lean hosting and a clear composition root.
- HTTP DTOs must be kept separate from domain models (see `AGENTS.md` §C).

## Related documents or code

- `src/Services/Procurement/Procurement.Presentation/Program.cs`
- `src/Services/Procurement/Procurement.Presentation/PurchaseRequests/CreatePurchaseRequestEndpoint.cs`
