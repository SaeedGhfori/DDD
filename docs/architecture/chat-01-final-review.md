# Chat 01 — Final Review (Procurement DDD, iteration 0.2)

Status: Final for Chat 01
Solution file: **`DDD.slnx`** (the classic `DDD.sln` is not present; all commands
below use `DDD.slnx`)
Related: `docs/domain/ubiquitous-language.md`,
`docs/architecture/domain-model-v0.2.md`, `docs/architecture/decisions/`

This document reflects the **actual** state of the code, not an ideal one.

---

## 1. Purpose of Chat 01

Establish a clean baseline before tactical DDD: a layered .NET 10 solution, an
initial `Procurement` domain model, a shared business vocabulary, the first
end-to-end use case (`CreatePurchaseRequest`), and a real green build/test run.

## 2. Business Scope

Mini Procurement:

- An employee creates a purchase request containing one or more items.
- It is submitted to the requester's department manager for review.
- The manager may approve, finally reject, or return it for correction.
- A returned request can be corrected and resubmitted.
- Self-approval is allowed.
- Editing is allowed until final approval *in business terms*, but the concrete
  behaviour while `Submitted` is not yet decided.

Out of scope: price, currency, tax, Supplier, Purchase Order, Invoice, Payment,
Warehouse, and all cross-service integration.

## 3. Business Rules v0.2

**Confirmed**

- BR-1: Any employee can create a purchase request.
- BR-2: A request has a Requester, a Department and an Approver.
- BR-3: A request must have at least one item to be submitted.
- BR-4: An item needs a valid description and quantity > 0.
- BR-5: A request starts in `Draft`.
- BR-6: A valid request can be submitted.
- BR-7: The requester's department manager is the decision maker.
- BR-8: Self-approval is allowed.
- BR-9: `ReturnedForCorrection` differs from `Rejected`.
- BR-10: A returned request can be resubmitted.
- BR-11: `Approved` and `Rejected` are final.
- BR-12: Only a `Submitted` request can receive a decision.
- BR-13: A request without items cannot be submitted.

**Temporary engineering decision (not a confirmed rule)**

- TMP-1: Items may only be added in `Draft` or `ReturnedForCorrection`; editing
  while `Submitted` is intentionally disabled (see ADR-0010 and §10).

## 4. Current Domain Model

```text
PurchaseRequest (aggregate-root candidate)
  Id, RequesterId, DepartmentId, ApproverId : Guid   (all must be non-empty)
  Status : PurchaseRequestStatus                     (private set, starts Draft)
  Items  : IReadOnlyCollection<PurchaseRequestItem>  (>= 0)
  + AddItem(item)              // Draft | ReturnedForCorrection
  + Submit()                   // Draft | ReturnedForCorrection, needs >= 1 item
  + Approve()                  // Submitted -> Approved
  + ReturnForCorrection()      // Submitted -> ReturnedForCorrection
  + Reject()                   // Submitted -> Rejected

PurchaseRequestItem (child entity)
  Id : Guid (non-empty), Description : string (trimmed, non-empty), Quantity : int (> 0)

PurchaseRequestStatus
  Draft | Submitted | ReturnedForCorrection | Approved | Rejected
```

Transitions: `Draft→Submitted`, `ReturnedForCorrection→Submitted`,
`Submitted→Approved|ReturnedForCorrection|Rejected`. `Approved`/`Rejected` are
terminal. `Employee` and `Department` are **external** concepts referenced only
by id.

## 5. Ubiquitous Language

See `docs/domain/ubiquitous-language.md`. Key distinctions:

- **Rejected** (terminal) vs **Returned for Correction** (revivable).
- **Employee** (member) vs **Requester / Department Manager** (roles).
- **Submit** vs **Resubmit** — one operation (`Submit`), two legal start states.
- **Approver** (role) vs **Approve** (action).

## 6. `CreatePurchaseRequest` flow

```
HTTP  POST /api/purchase-requests
  │
  ▼
Presentation  CreatePurchaseRequestEndpoint   (HTTP DTO + error mapping)
  │           CreatePurchaseRequestRequest/Response  (separate from the entity)
  ▼
Application   CreatePurchaseRequestHandler    (orchestration)
  │           CreatePurchaseRequestCommand / CreatePurchaseRequestResult
  ▼
Domain        new PurchaseRequest(Guid.NewGuid(), requesterId, departmentId, approverId)
              → id guards + Status = Draft
```

Responses: `201 Created` (with `purchaseRequestId`, `status`),
`400 Bad Request` (`ArgumentException`), `409 Conflict` (`InvalidOperationException`).
**No persistence** — the request is not stored anywhere.

## 7. Layer responsibilities (as implemented)

| Layer | Responsibility | Depends on | State |
| --- | --- | --- | --- |
| **Domain** | Entities, rules, controlled state transitions | _nothing_ | Implemented (`PurchaseRequest`, `PurchaseRequestItem`, status enum) |
| **Application** | Use-case orchestration, no rule duplication | Domain | Implemented (`CreatePurchaseRequestHandler`) |
| **Infrastructure** | Technical adapters | Domain, Application | Scaffold only (no code) |
| **Persistence** | EF Core + PostgreSQL | Domain, Application, EF Core, Npgsql | Scaffold only (no `DbContext`) |
| **Presentation** | HTTP input, invoke use case, produce response | Application, Infrastructure, Persistence | Implemented (Minimal API) |
| **ApiGateway** | Reverse proxy (YARP) | (standalone) | Scaffold; routing configured, not wired to both services operationally |
| **DDD.Contracts** | Cross-service integration contracts | (none) | Empty (correctly not a domain-entity dump) |

Dependency direction is verified by `Procurement.ArchitectureTests`.

## 8. Smart UI review

Result: **no Smart UI**. Business rules are **not** duplicated in the UI layer.

- `Program.cs` (Procurement) only registers health checks + the handler and maps
  endpoints — no business logic.
- `CreatePurchaseRequestEndpoint` contains only HTTP concerns: null-body check,
  DTO→command mapping and exception→status mapping (`ArgumentException`→400,
  `InvalidOperationException`→409). It does not check item counts, statuses, or
  transitions.
- Input `Guid.Empty` is not special-cased in the UI; it flows to the Domain,
  which rejects it (`ArgumentException`). Verified by a test asserting the
  `approverId` parameter name.
- No domain logic exists in `Notification.Presentation` or `DDD.ApiGateway`.

## 9. Existing tests

| Project | Count | Covers |
| --- | --- | --- |
| `Procurement.Domain.Tests` | 27 | identity guards, item validation (description/quantity/id), initial `Draft`, submit-without-item rejection, valid submit, all decisions, returned→resubmit, final-state immutability, scenario walkthrough |
| `Procurement.Application.Tests` | 7 | valid create → `Draft`, new non-empty id, empty-id rejection (per field), does not bypass domain validation, null command |
| `Procurement.ArchitectureTests` | 2 | Domain free of infra/upper layers/other service; Application free of Presentation/Infrastructure/Persistence/ASP.NET |
| `Procurement.IntegrationTests` | 2 | endpoint 201 + body shape; empty requester → 400 |
| Total | **38** | all passing |

Tests validate behaviour/contracts; none exist purely to inflate counts. Other
test projects (`Notification.*`, `DDD.ApiGateway.Tests`) are empty scaffolds.

## 10. Open ambiguities

1. Editing while `Submitted` (allowed? what changes? does it reset status?).
2. Whether corrections must be audited (who/when/what).
3. Approver resolution: always the requester's department manager? overridable?
4. Is self-approval permanent or a temporary convenience?
5. Should decisions carry a reason/comment and timestamp?
6. Can a `Rejected` request ever be revived?
7. Can items be removed/edited, or only appended?
8. Ownership of `Employee`/`Department` and how they are referenced.

## 11. Current solution limitations

- **No persistence.** The create endpoint returns a request that is not stored;
  nothing survives the process. No `DbContext`, no migrations, no repository.
- **Only one use case.** Submit / Approve / Return / Reject have no endpoints yet.
- **Approver is a temporary input**, not resolved from organization data.
- **`Notification` has no business logic** (expected at this stage).
- **ApiGateway is not operationally wired** to both services.
- **Infrastructure/Persistence are empty scaffolds.**
- **No auth, Docker, observability, messaging.**

## 12. Work remaining for Chat 02 (Tactical DDD)

- Formalize the aggregate and its invariants; introduce value objects where
  justified; decide the aggregate boundary.
- Resolve open domain questions (§10) with the business.
- Persistence: `DbContext`, mappings, migrations, repository/unit of work.
- Additional use cases and endpoints (submit/decision flow).
- Only then consider domain events, integration events, outbox, saga, CQRS —
  none of which are in Chat 01 scope.

## Validation (real)

```
dotnet restore DDD.slnx   → success
dotnet build   DDD.slnx -c Debug → Build succeeded (0 Warning, 0 Error)
dotnet test    DDD.slnx   → 38 passed, 0 failed, EXITCODE=0
```
