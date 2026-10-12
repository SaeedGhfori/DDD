# Domain Model v0.2 — Procurement (Mini Procurement)

Status: Draft (Chat 01)
Related: `docs/domain/ubiquitous-language.md`, `docs/architecture/decisions/`

This document captures the knowledge-crunching outcome for the first slice of
the `Procurement` domain: what is **confirmed**, what is a **temporary
assumption**, and what is **still ambiguous**. Nothing here is treated as a
final business rule unless it was explicitly stated.

---

## 1. Business Scope

Current scope of the Mini Procurement flow:

- An **Employee** creates a **Purchase Request**.
- A request contains **one or more** **Purchase Request Items**.
- A request is submitted for review to the **Department Manager** of the requester.
- The manager can **Approve**, **Reject** (final), or **Return for Correction**.
- A returned request can be corrected and **resubmitted**.
- **Self-approval** is currently allowed.
- Editing is allowed until final approval *in business terms*, but the concrete
  editing behaviour during `Submitted` is **not yet defined**.

Out of scope: price, currency, tax, Supplier, Purchase Order, Invoice, Payment,
Warehouse, and any cross-service integration.

## 2. Business Rules v0.2

### Confirmed (stated by the business)

- BR-1: Any employee can create a purchase request.
- BR-2: A request has a Requester, a Department, and an Approver.
- BR-3: A request must have at least one item.
- BR-4: An item must have a valid description and a quantity greater than zero.
- BR-5: A request starts in `Draft`.
- BR-6: A valid request can be submitted for review.
- BR-7: The requester's department manager is the decision maker.
- BR-8: Self-approval is allowed in the current version.
- BR-9: `ReturnedForCorrection` is different from `Rejected`.
- BR-10: A returned request can be resubmitted after correction.
- BR-11: `Approved` and `Rejected` are final states in the current model.
- BR-12: Only a `Submitted` request can receive a manager decision.
- BR-13: A request without items cannot be submitted.

### Temporary (engineering decision, pending business confirmation)

- TMP-1: Items can only be added while the request is in `Draft` or
  `ReturnedForCorrection`. Editing while `Submitted` is intentionally disabled.
  See §7 and `docs/architecture/decisions/adr-0010-temporary-item-editing-restriction.md`.

### Hypotheses — NOT rules

- The identity fields are treated as UUIDs and must not be `Guid.Empty`.
- The department manager can be derived from the requester's department, but the
  derivation mechanism is not modeled yet.

## 3. Domain Model

### Conceptual

```text
Employee
   |
   v
PurchaseRequest
   +---- RequesterId     (employee who created it)
   +---- DepartmentId    (organizational unit)
   +---- ApproverId      (department manager)
   +---- Status
   |
   +---- PurchaseRequestItem (1..*)
```

### Implemented (current code)

```text
PurchaseRequest                       (aggregate root candidate)
  - Id            : Guid                (not empty)
  - RequesterId   : Guid                (not empty)
  - DepartmentId  : Guid                (not empty)
  - ApproverId    : Guid                (not empty)
  - Status        : PurchaseRequestStatus (private set)
  + Items         : IReadOnlyCollection<PurchaseRequestItem>
  + AddItem(item)                 // Draft | ReturnedForCorrection
  + Submit()                      // Draft | ReturnedForCorrection, needs >= 1 item
  + Approve()                     // Submitted -> Approved
  + ReturnForCorrection()         // Submitted -> ReturnedForCorrection
  + Reject()                      // Submitted -> Rejected

PurchaseRequestItem
  - Id          : Guid            (not empty)
  - Description : string          (trimmed, non-empty)
  - Quantity    : int             (> 0)

PurchaseRequestStatus
  Draft | Submitted | ReturnedForCorrection | Approved | Rejected
```

### Notes on the shape

- `RequesterId`, `DepartmentId`, `ApproverId` are **plain identifiers** in this
  slice. `Employee` and `Department` remain **external concepts** owned by other
  contexts; they are not modeled inside this microservice yet.
- `PurchaseRequest` is the aggregate-root *candidate*; formal aggregate rules
  (invariants, events, repository) are deferred.
- `PurchaseRequestItem` is a child entity with no independent lifecycle.
- Approval is currently represented only by `Status = Approved` + `ApproverId`;
  a dedicated Approval concept is deferred.

## 4. Ubiquitous Language

The full glossary lives in `docs/domain/ubiquitous-language.md`. Key
distinctions:

- **Rejected** (terminal) vs **Returned for Correction** (revivable).
- **Employee** (member) vs **Requester / Department Manager** (roles).
- **Submit** vs **Resubmit** — one operation, two legal starting states.
- **Approver** (role) vs **Approve** (action).

## 5. Business Scenarios

### Scenario A — Successful request

| Step | Actor | Action | Status before → after | Rules applied |
| --- | --- | --- | --- | --- |
| 1 | Requester | Create request | — → `Draft` | BR-1, BR-5, IDs valid |
| 2 | Requester | Add one or more items | `Draft` → `Draft` | BR-4 |
| 3 | Requester | Submit | `Draft` → `Submitted` | BR-3, BR-6, BR-13 |
| 4 | Department Manager | Approve | `Submitted` → `Approved` | BR-7, BR-11, BR-12 |

Open question: does the approver ever add a comment/decision reason? (deferred)

### Scenario B — Returned for correction

| Step | Actor | Action | Status before → after | Rules applied |
| --- | --- | --- | --- | --- |
| 1 | Requester | Submit | `Draft` → `Submitted` | BR-3, BR-6 |
| 2 | Department Manager | Return for correction | `Submitted` → `ReturnedForCorrection` | BR-9, BR-12 |
| 3 | Requester | Correct allowed data | `ReturnedForCorrection` → `ReturnedForCorrection` | BR-10, TMP-1 |
| 4 | Requester | Resubmit (`Submit`) | `ReturnedForCorrection` → `Submitted` | BR-10 |
| 5 | Department Manager | Decide | `Submitted` → `Approved` or `Rejected` | BR-11, BR-12 |

Open questions: what exactly may be changed in step 3 (items only? identifiers?),
and must a change be recorded?

### Scenario C — Final rejection

| Step | Actor | Action | Status before → after | Rules applied |
| --- | --- | --- | --- | --- |
| 1 | Requester | Submit | `Draft` → `Submitted` | BR-3, BR-6 |
| 2 | Department Manager | Reject | `Submitted` → `Rejected` | BR-9, BR-12 |
| 3 | — | (terminal) | `Rejected` → `Rejected` | BR-11 |

No normal operation re-opens a `Rejected` request in this version.

### Code vs scenarios

The current code is **consistent** with all three scenarios:
`AddItem` (Draft/ReturnedForCorrection), `Submit` (Draft/ReturnedForCorrection
with ≥ 1 item), and the three decision methods (Submitted only) match the tables
above. No corrective code change was required.

## 6. Confirmed Decisions

- States: `Draft`, `Submitted`, `ReturnedForCorrection`, `Approved`, `Rejected`.
- Transitions: `Draft→Submit→Submitted`, `ReturnedForCorrection→Submit→Submitted`,
  `Submitted→{Approve|ReturnForCorrection|Reject}`.
- `Approved` and `Rejected` are terminal.
- `ReturnedForCorrection` must stay distinct from `Rejected`.
- A request requires ≥ 1 item before submission; items need a valid description
  and quantity > 0.
- Identity fields must not be empty; `Status` is only mutated through methods.
- Self-approval is allowed.
- Identities are plain IDs; `Employee`/`Department` are external to this context.

## 7. Temporary Assumptions

- TMP-1 (see §2): editing only in `Draft` / `ReturnedForCorrection`.
- The "department manager of the requester is the approver" rule is currently
  not enforced by the model; `ApproverId` is supplied by the caller.

## 8. Remaining Ambiguities (open questions for the business)

1. **Editing while `Submitted`**: allowed? what may change? does an edit reset
   status or keep `Submitted`?
2. **Edit audit**: must corrections be recorded (who/when/what)?
3. **Approver derivation**: is the approver always the requester's department
   manager, and how is that resolved? Can it be overridden?
4. **Self-approval**: is it a permanent rule or a temporary convenience?
5. **Decision metadata**: should decisions carry a reason/comment and timestamp?
6. **Rejected lifecycle**: is a rejected request ever revived (e.g. by a new
   request or an appeal)?
7. **Items**: can items be removed or edited, or only appended?
8. **Department/Employee ownership**: which context owns them and how are they
   referenced (IDs only, or projections)?

## 9. Conceptual Model vs Current Implementation

| Conceptual | Implemented | Gap / note |
| --- | --- | --- |
| Employee | `RequesterId` / `ApproverId` (Guid) | External concept, not modeled |
| Department | `DepartmentId` (Guid) | External concept, not modeled |
| Department Manager | `ApproverId` (role) | No derivation from department yet |
| Purchase Request | `PurchaseRequest` | Aggregate root candidate only |
| Purchase Request Item | `PurchaseRequestItem` | Matches |
| Status lifecycle | `PurchaseRequestStatus` + methods | Matches |
| Approval (rich) | `Status` + `ApproverId` | Not modeled (deferred) |

No implementation change is proposed here; the gaps are deliberate and listed as
open questions or deferred items.
