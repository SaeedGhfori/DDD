# Architecture Decision Records (ADRs)

Canonical location for architecture decisions in this repository.

## Index

| ID | Title | Status |
| --- | --- | --- |
| [ADR-0001](adr-0001-clean-architecture-layers.md) | Clean Architecture with layered services | Accepted |
| [ADR-0002](adr-0002-independent-domain-per-microservice.md) | One independent domain per microservice | Accepted |
| [ADR-0003](adr-0003-contracts-for-integration-only.md) | `DDD.Contracts` for integration contracts only | Accepted |
| [ADR-0004](adr-0004-minimal-api-presentation.md) | ASP.NET Core Minimal APIs for presentation | Accepted |
| [ADR-0005](adr-0005-central-package-management.md) | Central Package Management | Accepted |
| [ADR-0006](adr-0006-shared-build-props.md) | Shared build settings via `Directory.Build.props` | Accepted |
| [ADR-0007](adr-0007-yarp-api-gateway.md) | YARP for the API gateway | Accepted |
| [ADR-0008](adr-0008-postgresql-target-store.md) | PostgreSQL as the target relational store | Accepted |
| [ADR-0009](adr-0009-strict-compiler-settings.md) | Warnings as errors, nullable + implicit usings | Accepted |
| [ADR-0010](adr-0010-temporary-item-editing-restriction.md) | Temporary: item editing only in `Draft`/`ReturnedForCorrection` | Accepted (temporary) |

## Open (no ADR yet — decide before implementing)

- Messaging / integration events transport (e.g. RabbitMQ + MassTransit), contracts, versioning.
- Database ownership (database-per-service vs schema-per-service) and migration strategy.
- Outbox / Inbox for reliable messaging.
- Saga / process managers for cross-service workflows.
- Gateway concerns (auth, rate limiting, resilience, API aggregation).
- Authentication provider and token strategy.
- Observability (logging, tracing, metrics).
- Docker / local orchestration.

These are **not** implemented. Do not introduce them without a new ADR
(`AGENTS.md` §G).

## Template

Use [`template.md`](template.md). Keep each ADR short and factual. Record only
decisions that are actually adopted; do not turn assumptions into accepted
decisions.
