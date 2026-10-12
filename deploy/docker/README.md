# Docker

Docker assets for the solution (not implemented yet).

## Planned contents

- `Dockerfile` per deployable (`Procurement.Presentation`, `Notification.Presentation`, `DDD.ApiGateway`).
- Multi-stage builds based on the .NET 10 SDK/runtime images.
- `docker-compose.yml` wiring the gateway, both services and PostgreSQL.
- `.env.example` for non-secret configuration; real secrets stay out of source control.

## Not yet implemented

The Dockerfiles and compose files are intentionally deferred to a later
iteration. No containers are defined at this time.
