# Current State

This document describes what actually exists in the repository. It must be
updated when material implementation changes occur.

Solution file: `DDD.slnx` (21 projects). SDK `10.0.400`, target `net10.0`.

## Status legend

- **Implemented & verified** — code exists and tests/build confirm it.
- **Implemented, not verified** — code exists but lacks confirmation.
- **Planned** — designed/scaffolded, not implemented.
- **Not implemented** — no code.
- **Unknown** — not inspected / cannot confirm.

## Projects

| Project | State |
| --- | --- |
| `Procurement.Domain` | Implemented & verified (`PurchaseRequest`, `PurchaseRequestItem`, `PurchaseRequestStatus`) |
| `Procurement.Application` | Implemented & verified (`CreatePurchaseRequestHandler` + Command/Result) |
| `Procurement.Presentation` | Implemented & verified (`POST /api/purchase-requests`, health, root) |
| `Procurement.Infrastructure` | Planned — empty scaffold |
| `Procurement.Persistence` | Planned — scaffold references EF Core + Npgsql, no code |
| `Notification.*` (5 projects) | Planned — empty scaffolds; only `Program.cs` (health + root) |
| `DDD.ApiGateway` | Implemented, not verified — YARP routes configured to ports 5101/5102 |
| `DDD.Contracts` | Not implemented — empty (reserved for integration contracts) |

## Implemented endpoints and use cases

| Endpoint | Use case | Behaviour |
| --- | --- | --- |
| `GET /` | — | Service liveness JSON (`Procurement`) |
| `GET /health` | — | Health check |
| `POST /api/purchase-requests` | `CreatePurchaseRequest` | `201` on success, `400` invalid input, `409` domain rule violation |

`CreatePurchaseRequest` accepts `requesterId`, `departmentId`, `approverId`,
creates the aggregate in memory, and returns `{ purchaseRequestId, status }`
with `status = "Draft"`.

## Implemented domain models

- `PurchaseRequest` — aggregate-root candidate: `Id`, `RequesterId`,
  `DepartmentId`, `ApproverId` (non-empty), `Status` (starts `Draft`), `Items`
  (read-only), and behaviour `AddItem`, `Submit`, `Approve`,
  `ReturnForCorrection`, `Reject`.
- `PurchaseRequestItem` — `Id` (non-empty), `Description` (trimmed, non-empty),
  `Quantity` (> 0).
- `PurchaseRequestStatus` — `Draft`, `Submitted`, `ReturnedForCorrection`,
  `Approved`, `Rejected`.

## Test projects and categories

| Project | Category | Tests |
| --- | --- | --- |
| `Procurement.Domain.Tests` | Unit | 28 |
| `Procurement.Application.Tests` | Unit | 7 |
| `Procurement.ArchitectureTests` | Architecture | 2 |
| `Procurement.IntegrationTests` | Integration | 2 |
| `Notification.Application.Tests` | — | Empty scaffold |
| `Notification.ArchitectureTests` | — | Empty scaffold |
| `Notification.Domain.Tests` | — | Empty scaffold |
| `Notification.IntegrationTests` | — | Empty scaffold |
| `DDD.ApiGateway.Tests` | — | Empty scaffold |

Total executed: **39 passing** (last verified run — see "Build & test
verification" below). Test counts are the number of executed test cases; this is
**not** a coverage measurement.

## Feature status

| Feature | State |
| --- | --- |
| Persistence | **Not implemented** — the create endpoint does not store data |
| Messaging / integration events | **Not implemented** |
| Gateway operational integration | **Not verified** — routes configured only |
| Notification business logic | **Not implemented** |
| Authentication / authorization | **Not implemented** |
| Approver resolution (department manager) | **Not implemented** — `ApproverId` is a temporary input |
| Coverage measurement | **Unknown** — never measured |

## Build & test verification

Commands (run from the repository root, using the real solution file):

```powershell
dotnet restore DDD.slnx
dotnet build   DDD.slnx -c Debug
dotnet test    DDD.slnx
```

Last observed result: restore succeeded, build succeeded with 0 warnings and 0
errors, tests: 39 passed / 0 failed / 0 skipped.

> `dotnet build DDD.sln` fails with `MSB1009` because only `DDD.slnx` exists.
> Always use `DDD.slnx`.

## Known limitations

- No persistence; created requests are not stored.
- Only one use case (`CreatePurchaseRequest`); no submit/decision endpoints.
- `ApproverId` is not validated as the real department manager.
- `Notification`, `Infrastructure`, `Persistence` are empty scaffolds.
- No auth, no messaging, no gateway operational verification.
- No coverage measurement.

## Next technical milestones (established)

1. Tactical DDD building blocks and resolution of open domain questions.
2. Persistence (EF Core `DbContext`, mappings, migrations, repository/unit of work).
3. Remaining Procurement use cases and endpoints (submit / decision flow).
