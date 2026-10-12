# DDD

A learning-oriented .NET 10 solution built around **Domain-Driven Design (DDD)**,
**Clean Architecture** and a **microservice-oriented** layout.

This repository is developed iteratively as part of the training series:

> Domain-Driven Design for Senior .NET / Enterprise Backend — 2026

## Solution overview

The solution contains two independent microservices and one API gateway:

| Component      | Path                                   | Responsibility                                  |
| -------------- | -------------------------------------- | ----------------------------------------------- |
| `Procurement`  | `src/Services/Procurement`             | Purchase request capability                     |
| `Notification` | `src/Services/Notification`            | Outbound notification capability                |
| `DDD.ApiGateway` | `src/ApiGateway/DDD.ApiGateway`      | YARP reverse proxy in front of the services     |

Shared communication contracts live in `src/BuildingBlocks/DDD.Contracts`.

## Layers per service

Each microservice is split into independent projects following Clean Architecture:

```
Presentation  ──►  Application  ──►  Domain
Infrastructure ──►  Application / Domain
Persistence    ──►  Application / Domain
```

- **Domain** — entities, value objects and domain rules. No external dependencies.
- **Application** — use cases / orchestration. Depends on `Domain`.
- **Infrastructure** — technical concerns (integrations, adapters).
- **Persistence** — Entity Framework Core + PostgreSQL data access.
- **Presentation** — ASP.NET Core Minimal API composition root.

`Presentation` is the composition root and references the other layers through
Dependency Injection.

## Prerequisites

- [.NET SDK 10.0.400](https://dotnet.microsoft.com/) (see `global.json`)
- (Optional) PostgreSQL 16+ for future database work
- (Optional) Docker for the future `deploy` assets

## Build & test

```powershell
dotnet restore DDD.slnx
dotnet build DDD.slnx -c Debug
dotnet test DDD.slnx
```

Run a single service or the gateway:

```powershell
dotnet run --project src/Services/Procurement/Procurement.Presentation
dotnet run --project src/Services/Notification/Notification.Presentation
dotnet run --project src/ApiGateway/DDD.ApiGateway
```

## First use case — `CreatePurchaseRequest`

`POST /api/purchase-requests` creates a new purchase request from the requester,
department and approver identifiers.

```powershell
dotnet run --project src/Services/Procurement/Procurement.Presentation
# http://localhost:5101/api/purchase-requests
```

```http
POST /api/purchase-requests
{
  "requesterId": "11111111-1111-1111-1111-111111111111",
  "departmentId": "22222222-2222-2222-2222-222222222222",
  "approverId": "33333333-3333-3333-3333-333333333333"
}
```

Responses: `201 Created` on success, `400 Bad Request` for invalid input,
`409 Conflict` for a domain rule violation.

> **No persistence yet.** The request is created in memory by the Domain and is
> **not stored**. The endpoint exists only to exercise the HTTP → Application →
> Domain path; nothing survives the process. `ApproverId` is a temporary input
> until an organization service can resolve the requester's department manager.

## Central Package Management

NuGet versions are managed centrally. **All** package versions are declared in
`Directory.Packages.props`; individual `.csproj` files reference packages
*wifhout* a `Version` attribute:

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" />
```

Common build properties (`TargetFramework`, `Nullable`, `ImplicitUsings`,
`TreatWarningsAsErrors`) are defined once in `Directory.Build.props`.

## Current status

Implemented in the current iteration:

- Full solution / project skeleton with layer boundaries.
- Shared build & package configuration, `.editorconfig`, `.gitignore`, `global.json`.
- Initial `Procurement` domain model (`PurchaseRequest`, `PurchaseRequestItem`).
- First application use case `CreatePurchaseRequest` (Application + Minimal API endpoint).
- Minimal API entry points and a YARP-based API gateway reading routes from configuration.
- Domain unit tests for `PurchaseRequest`, application tests for the use case,
  layered-architecture tests and an initial HTTP integration test for the create endpoint.

Not implemented yet (planned for later iterations):

- Persistence: the create endpoint does not store data (EF Core `DbContext`,
  model configuration and migrations are still pending).
- Remaining business endpoints (submit/approve/reject/return) for Procurement.
- Notification business endpoints and messaging / integration events (`DDD.Contracts`).
- Authentication, Docker, observability, resilience policies.

See `docs/architecture` and `docs/decisions` for details.
