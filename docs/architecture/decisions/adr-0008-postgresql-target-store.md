# ADR-0008 — PostgreSQL as the target relational store

- **Status:** Accepted
- **Date:** unknown
- **Related:** `Directory.Packages.props`, `docs/architecture/system-overview.md`

## Context

The services will eventually need durable relational storage.

## Decision

PostgreSQL is the target relational store, accessed via EF Core with the Npgsql
provider. The Persistence projects reference `Microsoft.EntityFrameworkCore` and
`Npgsql.EntityFrameworkCore.PostgreSQL`. **No persistence is implemented yet.**

## Alternatives considered

- SQL Server — rejected: open-source/portability preference.
- No relational store — rejected: business data must persist.

## Consequences and trade-offs

- Provider is fixed for future persistence work.
- Until persistence is implemented, created data is not stored.

## Related documents or code

- `src/Services/Procurement/Procurement.Persistence/Procurement.Persistence.csproj`
- `docs/architecture/current-state.md`
