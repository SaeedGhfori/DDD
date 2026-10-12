# Architecture Decisions

This folder records architecture decisions and explicitly-open questions.

## Decided

| #   | Decision                                                            | Rationale                                                        |
| --- | ------------------------------------------------------------------- | ---------------------------------------------------------------- |
| 1   | Clean Architecture with Domain / Application / Infrastructure / Persistence / Presentation | Keeps domain independent and testable.              |
| 2   | One independent domain per microservice; no shared domain project    | Preserves service boundaries (no shared-domain coupling).        |
| 3   | `DDD.Contracts` limited to integration contracts only               | Prevents leaking domain models between services.                 |
| 4   | ASP.NET Core Minimal APIs for presentation                          | Lean service hosting with a clear composition root.              |
| 5   | Central Package Management (`Directory.Packages.props`)             | Single source of truth for package versions.                     |
| 6   | `Directory.Build.props` for shared build settings                   | Consistency across all projects; avoids repetition.              |
| 7   | YARP for the API gateway                                            | First-class .NET reverse proxy with config-driven routing.       |
| 8   | PostgreSQL as the relational store                                  | Target database for EF Core persistence.                         |
| 9   | Treat warnings as errors, nullable + implicit usings enabled        | Higher baseline code quality.                                    |

## Open / to decide in later iterations

- **Messaging / Integration events**: transport (e.g. RabbitMQ + MassTransit), message contracts, versioning.
- **Database ownership**: one database per service vs. schema-per-service; migration strategy.
- **Outbox / Inbox**: reliable messaging and idempotency.
- **Saga / Process managers**: cross-service workflows.
- **Gateway concerns**: authentication/authorization, rate limiting, retry & resilience policies, Swagger aggregation.
- **Authentication**: provider, token strategy, service-to-service auth.
- **Observability**: logging sinks (Serilog), distributed tracing, metrics.
- **Docker / local orchestration**: compose files and service images.

## Notes

Nothing listed under "Open" is implemented yet; the projects are scaffolded so
these concerns can be added without restructuring.
