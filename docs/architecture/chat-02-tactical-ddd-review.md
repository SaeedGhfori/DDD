# Chat 02 — Tactical DDD Implementation (Review)

## Summary

Tactical DDD building blocks were introduced into the `Procurement` domain
without expanding the solution scope. Domain logic became explicit, the
aggregate boundary was strengthened, and persistence access was formalised.

## Existing repository state

- 39 tests passing prior to this task (Domain 28, Application 7, Architecture 2, Integration 2).
- Domain model: `PurchaseRequest`, `PurchaseRequestItem`, `PurchaseRequestStatus`.
- Application: `CreatePurchaseRequestHandler` creating the aggregate in memory.
- No repository; persistence project had only package references and project references.

## Files created

- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/PurchaseRequestId.cs`
- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/PurchaseRequestItemId.cs`
- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/Quantity.cs`
- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/IPurchaseRequestRepository.cs`
- `src/Services/Procurement/Procurement.Persistence/ProcurementDbContext.cs`
- `src/Services/Procurement/Procurement.Persistence/Configurations/PurchaseRequestConfiguration.cs`
- `src/Services/Procurement/Procurement.Persistence/Repositories/PurchaseRequestRepository.cs`
- `tests/Procurement/Procurement.Domain.Tests/PurchaseRequests/ValueObjectTests.cs`
- `tests/Procurement/Procurement.IntegrationTests/Persistence/PurchaseRequestRepositoryTests.cs`
- `docs/architecture/decisions/adr-0011-repository-per-aggregate.md`

## Files modified

- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/PurchaseRequest.cs` — refactored to use typed ids, `Factory` methods (`Create`, `Reconstitute`), explicit equality, encapsulated collection; `AddItem` now takes `(string description, Quantity quantity)` and returns the created item.
- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/PurchaseRequestItem.cs` — made immutable/value-equality by id, uses `PurchaseRequestItemId` + `Quantity`.
- `src/Services/Procurement/Procurement.Application/PurchaseRequests/Create/CreatePurchaseRequestHandler.cs` — uses `PurchaseRequest.Create` and returns `purchaseRequest.Id.Value`.
- `tests/Procurement/Procurement.Domain.Tests/PurchaseRequests/PurchaseRequestItemTests.cs` — updated to new constructor; added id-equality checks.
- `tests/Procurement/Procurement.Domain.Tests/PurchaseRequests/PurchaseRequestTests.cs` — adapted to `Create`/`AddItem` signatures, added `Reconstitute` equality test and reconstruction test.
- `tests/Procurement/Procurement.IntegrationTests/Procurement.IntegrationTests.csproj` — added `Microsoft.EntityFrameworkCore.InMemory` and project reference to `Procurement.Persistence`.

## Tactical DDD concepts

- **Value Objects:** `PurchaseRequestId`, `PurchaseRequestItemId`, `Quantity` (strong invariants: non-empty / > 0; `record struct` semantics where appropriate).
- **Entity:** `PurchaseRequestItem` equality by identity (`Id`).
- **Aggregate & AR:** `PurchaseRequest` is Aggregate Root; controls all changes, enforces status rules, guards invariants.
- **Factory:** static `Create(...)` and `Reconstitute(...)` on the aggregate.
- **Repository:** domain abstraction `IPurchaseRequestRepository`; EF Core implementation in Persistence (collection-like, no `IQueryable` leakage).
- **Domain Service / Specification:** **not implemented** — after inspecting rules, no real domain operation forced a stateless domain service; no invariant required a reusable specification. This is documented as deliberate.
- **Module:** existing namespace `Procurement.Domain.PurchaseRequests` kept (simple and coherent).

## Key modelling decisions

- **Identity control:** AR generates item identities (`PurchaseRequestItemId.New()` inside `AddItem`). External code no longer creates `PurchaseRequestItem` instances with arbitrary IDs outside AR control.
- **Encapsulation:** `_items` is private; `Items` exposes `IReadOnlyCollection<PurchaseRequestItem>` and EF mapping uses field access (`UsePropertyAccessMode.Field`) + `OwnsMany`.
- **Persistence mapping:** `DbContext` + `IEntityTypeConfiguration<PurchaseRequest>` with value converters. **Repository not wired into HTTP flow** (kept minimal as per chat scope).
- **Preservation of business rules:** all confirmed rules preserved. Self-approval allowed (confirmed). Editing restriction remains as per domain logic/tests.
- **Primitive Obsession reduced** while keeping external references (`RequesterId/DepartmentId/ApproverId`) as `Guid` (concepts outside this context).

## Business invariants enforced

- Non-empty IDs (request + item) via VO/from.
- `Quantity > 0`.
- `Description` non-empty/whitespace + trimmed.
- Initial state `Draft`.
- `AddItem` only in `Draft` or `ReturnedForCorrection`.
- `Submit` requires ≥ 1 item and only from `Draft`/`ReturnedForCorrection`.
- Decisions (`Approve/ReturnForCorrection/Reject`) only from `Submitted`.
- `Approved`/`Rejected` terminal.

## Tests and results

- **Domain.Tests:** 36 (added `ValueObjectTests`, updated existing tests) — all passing.
- **Application.Tests:** 7 — all passing (adapted handler to new API).
- **Architecture.Tests:** 2 — all passing (layering unchanged).
- **Integration.Tests:** 4 (2 endpoint + 2 repository round-trip) — all passing.
- **Total:** 49 / 0 failed / 0 skipped.

Commands executed:

```powershell
dotnet build DDD.slnx -c Debug --no-restore
dotnet test  DDD.slnx -c Debug --no-build --verbosity quiet
```

## Known limitations

- Repository is **not registered in DI** and **not used** by the HTTP endpoint; out of scope for minimal validation.
- EF mapping tested only with **InMemory**; Npgsql-specific behaviour not verified.
- `Reconstitute` assumes items are valid (no re-validation) — standard for persistence rehydration.
- `GetByIdAsync` does not include `Include` semantics (single aggregate small) but correct for owned collection in this mapping.
- `AddItem` still enforces the temporary restriction (BR-13/Open Question) — unchanged.

## Unresolved decisions (deferred)

- Wiring repository into Application/Presentation, UoW, transactions, migrations (later chats).
- Specification / Domain Service — documented as unnecessary for current confirmed rules.

## Recommendation

Ready to commit. The changes strengthen the domain model without changing runtime behaviour of the existing endpoint. Tests validate new building blocks and round-trip reconstitution.
