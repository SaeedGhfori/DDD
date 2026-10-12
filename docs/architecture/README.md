# Architecture

## High level

```
                         +---------------------+
                         |   DDD.ApiGateway    |
                         |  (YARP reverse proxy)|
                         +----------+----------+
                                    |
             +----------------------+----------------------+
             |                                             |
   +---------v----------+                       +----------v---------+
   |    Procurement     |                       |    Notification    |
   |  (microservice)    |                       |   (microservice)   |
   +--------------------+                       +--------------------+
```

Each microservice owns its own domain and data. There is no shared domain
library between services.

## Layered / dependency direction

```
                            (composition root)
   Presentation ─────────►  Application  ─────────►  Domain
        │                                                ^
        └─────────►  Infrastructure  ────────────────────┘
        │                    │
        └─────────►  Persistence  ────────────────────────┘
```

Rules enforced by project references:

1. `Domain` has **no** project references and no dependency on ASP.NET Core or EF Core.
2. `Application` references `Domain` only.
3. `Infrastructure` and `Persistence` reference `Domain` and `Application`.
4. `Presentation` references `Application`, `Infrastructure` and `Persistence`
   and is the only place where the dependency graph is composed via DI.
5. `DDD.Contracts` is referenced only where cross-service integration contracts
   are needed. It contains no domain logic.
6. `DDD.ApiGateway` is standalone; gateway concerns never leak into service layers.

No cycles exist in the reference graph.

## Per-service layout

```
src/Services/<Service>/
  <Service>.Domain           # entities, value objects, domain rules
  <Service>.Application      # use cases, orchestration
  <Service>.Infrastructure   # technical adapters
  <Service>.Persistence      # EF Core + PostgreSQL
  <Service>.Presentation     # ASP.NET Core Minimal API (composition root)
```

## Cross-cutting

- **Gateway:** YARP reverse proxy, routes/clusters loaded from configuration
  (`ReverseProxy` section). Service addresses are configuration-driven.
- **Contracts:** `DDD.Contracts` is reserved for integration event contracts.
- **Data ownership:** each service is expected to own its schema/database.

## Domain modeling (Procurement)

- [Ubiquitous Language](ubiquitous-language.md) — shared business vocabulary (v0.2).
- [Domain Model v0.2](domain-model-v0.2.md) — scope, rules, scenarios, decisions,
  assumptions and open questions.
- [Chat 01 Final Review](chat-01-final-review.md) — architecture/test validation
  and hand-off state for Chat 02.
- [Chat 01 Audit Report](chat-01-audit-report.md) — evidence-based audit and
  readiness decision.

## First use case — `CreatePurchaseRequest` (Procurement)

```
HTTP  POST /api/purchase-requests
  │
  ▼
Presentation   CreatePurchaseRequestEndpoint   (HTTP DTO, maps errors)
  │            → CreatePurchaseRequestRequest  (separate from the domain entity)
  ▼
Application    CreatePurchaseRequestHandler    (orchestration only)
  │            → CreatePurchaseRequestCommand / CreatePurchaseRequestResult
  ▼
Domain         new PurchaseRequest(id, requesterId, departmentId, approverId)
               (identity guards + initial Status = Draft live here)
```

Responsibility boundaries:

- **Domain** owns all rules (non-empty ids, `Draft` initial state, transitions).
- **Application** orchestrates: builds the aggregate, returns id + status. It does
  not re-implement domain validation and never mutates domain state directly.
- **Presentation** owns the HTTP DTOs, handler invocation and error mapping
  (`ArgumentException` → 400, `InvalidOperationException` → 409).

Not yet implemented for this flow:

- **No persistence.** The created request is not saved; nothing survives the
  process. There is no repository / unit of work yet, by design.
- **No approver resolution.** `ApproverId` is a temporary input until an
  organization service can resolve the requester's department manager.

## Open points

See `docs/decisions` for architecture decisions and items still open.
