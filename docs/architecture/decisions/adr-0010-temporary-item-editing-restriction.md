# ADR-0010 — Temporary: item editing only in `Draft` / `ReturnedForCorrection`

- **Status:** Accepted (temporary)
- **Date:** unknown
- **Related:** `docs/domain/business-rules.md` (BR-13), `docs/architecture/domain-model-v0.2.md`

## Context

Business states that editing is allowed until final approval, but the concrete
editing behaviour while a request is in `Submitted` (who may edit, what may
change, whether it resets the approval) is **not yet defined**.

## Decision

`PurchaseRequest.AddItem` is permitted only while the request is in `Draft` or
`ReturnedForCorrection`. Editing while `Submitted` is intentionally disabled
rather than guessed.

> Previously recorded as "ADR-010" in the former `docs/decisions/README.md`.

## Alternatives considered

- Allow editing while `Submitted` — rejected: the rules are undefined; guessing
  could silently change business behaviour.
- Allow unrestricted editing — rejected: contradicts the approval lifecycle.

## Consequences and trade-offs

- The code does not encode an unconfirmed business decision.
- Until the open question is answered, submitted requests are locked for edits.

## Related documents or code

- `src/Services/Procurement/Procurement.Domain/PurchaseRequests/PurchaseRequest.cs`
- `docs/domain/business-rules.md` (BR-13)
