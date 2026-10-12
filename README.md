# DDD

A learning-oriented .NET 10 solution built around **Domain-Driven Design (DDD)**,
**Clean Architecture** and a **microservice-oriented** layout.

> Domain-Driven Design for Senior .NET / Enterprise Backend — 2026

**Start here (for humans and AI agents):** [`AGENTS.md`](AGENTS.md) is the primary
entry point for working in this repository.

## Business purpose

**Mini Procurement** — employees create purchase requests; a department manager
approves, finally rejects, or returns them for correction. A returned request can
be corrected and resubmitted.

## Repository structure

```
src/
  Services/Procurement/      Procurement microservice (Domain, Application, Infrastructure, Persistence, Presentation)
  Services/Notification/     Notification microservice (scaffolding only)
  ApiGateway/DDD.ApiGateway  YARP reverse proxy
  BuildingBlocks/DDD.Contracts  Cross-service integration contracts (empty)
tests/
  Procurement/               Domain, Application, Architecture, Integration tests
  Notification/              Empty test scaffolds
  ApiGateway/                Empty test scaffold
docs/
  architecture/              system overview, current state, ADRs, model detail, reports
  domain/                    business rules, ubiquitous language
deploy/                      Docker assets (placeholders)
```

The solution file is **`DDD.slnx`** (there is no classic `DDD.sln`). It contains
21 projects.

## Prerequisites

- [.NET SDK 10.0.400](https://dotnet.microsoft.com/) (pinned in `global.json`, `rollForward: latestMinor`)
- (Optional) PostgreSQL 16+ for future persistence work
- (Optional) Docker for the future `deploy` assets

## Restore, build, test

```powershell
dotnet restore DDD.slnx
dotnet build   DDD.slnx -c Debug
dotnet test    DDD.slnx
```

Run the Procurement service:

```powershell
dotnet run --project src/Services/Procurement/Procurement.Presentation
# POST http://localhost:5101/api/purchase-requests
```

## Current implementation status (summary)

- **Implemented & verified:** Procurement domain model + rules, the
  `CreatePurchaseRequest` use case (HTTP → Application → Domain), layered
  dependency rules (architecture tests).
- **Not implemented:** persistence (created requests are **not stored**),
  remaining use cases/endpoints, approver resolution, Notification logic,
  messaging, authentication, gateway operational integration.

Full, maintained detail: [`docs/architecture/current-state.md`](docs/architecture/current-state.md).

## Documentation

| Document | Purpose |
| --- | --- |
| [`AGENTS.md`](AGENTS.md) | Working instructions for AI coding agents |
| [`docs/architecture/system-overview.md`](docs/architecture/system-overview.md) | Context, components, boundaries |
| [`docs/architecture/current-state.md`](docs/architecture/current-state.md) | Implemented vs planned |
| [`docs/architecture/decisions/`](docs/architecture/decisions/README.md) | Architecture Decision Records |
| [`docs/domain/business-rules.md`](docs/domain/business-rules.md) | Canonical business rules |
| [`docs/domain/ubiquitous-language.md`](docs/domain/ubiquitous-language.md) | Canonical vocabulary |
| [`docs/architecture/domain-model-v0.2.md`](docs/architecture/domain-model-v0.2.md) | Procurement model detail |
| [`docs/architecture/chat-01-audit-report.md`](docs/architecture/chat-01-audit-report.md) | Audit & readiness |

## Central package management

All package versions are declared centrally in `Directory.Packages.props`;
`PackageReference` entries in `.csproj` files have **no** `Version` attribute.
Shared build properties live in `Directory.Build.props`.
