# ADR-0011 — Repository per aggregate root with EF Core mapping (Procurement)

- **Status:** Accepted
- **Date:** 2026-10-12
- **Related:** ADR-0008, `docs/architecture/chat-02-tactical-ddd-review.md`

## Context

Chat 02 (Tactical DDD) introduced a formal Aggregate Root (`PurchaseRequest`).
Persistence access must not leak EF Core into the Domain layer, and the aggregate
must be reconstitutable from storage. ADR-0008 fixed PostgreSQL (via EF Core +
Npgsql) as the target store, but no persistence was implemented previously.

## Decision

- Expose a domain-oriented `IPurchaseRequestRepository` in the **Domain** layer
  with collection-like semantics (`AddAsync`, `GetByIdAsync`).
- Provide a concrete EF Core implementation (`ProcurementDbContext` +
  `PurchaseRequestRepository`) in **Procurement.Persistence**.
- Map entities with value converters for the typed identifiers
  (`PurchaseRequestId`, `PurchaseRequestItemId`) and `Quantity`; model items as an
  owned collection of the aggregate.
- Verify persistence with the **EF Core InMemory** provider only; **no real
  database is provisioned**.

The repository is **not yet wired** into the running HTTP service.

## Alternatives considered

- Generic `Repository<T>` — rejected: adds indirection without a demonstrated
  need; aggregates need tailored, expressive operations.
- Exposing `IQueryable` from the Domain abstraction — rejected: leaks ORM
  concerns and pushes query logic outside the aggregate.
- Deferring the repository entirely — rejected: Chat 02 scope requires the
  repository building block, and ADR-0008 already fixes the store.

## Consequences and trade-offs

- Domain stays persistence-ignorant; the aggregate can be reconstituted.
- EF Core mapping is confined to the Persistence project.
- Adds an EF Core dependency surface to the solution.
- Verified only against InMemory; Npgsql/provider-specific behaviour is unverified.
- The repository is unused by the application flow today; HTTP-created requests
  are still not stored.

## Related documents or code

- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/IPurchaseRequestRepository.cs`
- `src/Services/Procurement/Procurement.Persistence/ProcurementDbContext.cs`
- `src/Services/Procurement/Procurement.Persistence/Repositories/PurchaseRequestRepository.cs`
- `docs/architecture/current-state.md`
