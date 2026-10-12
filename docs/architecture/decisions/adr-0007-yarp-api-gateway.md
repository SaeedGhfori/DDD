# ADR-0007 — YARP for the API gateway

- **Status:** Accepted
- **Date:** unknown
- **Related:** `src/ApiGateway/DDD.ApiGateway/`

## Context

Clients need a single entry point in front of the microservices.

## Decision

Use YARP reverse proxy, configured from the `ReverseProxy` configuration section.
Routes currently map `/procurement/**` → cluster on `http://localhost:5101/` and
`/notification/**` → cluster on `http://localhost:5102/`.

## Alternatives considered

- Ocelot — rejected: YARP is first-party and config-driven.
- No gateway — rejected: clients would call services directly.

## Consequences and trade-offs

- Configuration-driven routing, easy to extend.
- Operational end-to-end verification and gateway concerns (auth, resilience)
  are still pending.

## Related documents or code

- `src/ApiGateway/DDD.ApiGateway/Program.cs`
- `src/ApiGateway/DDD.ApiGateway/appsettings.json`
