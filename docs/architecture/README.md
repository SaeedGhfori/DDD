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

## Open points

See `docs/decisions` for architecture decisions and items still open.
