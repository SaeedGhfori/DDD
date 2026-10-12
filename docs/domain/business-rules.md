# Business Rules — Mini Procurement (canonical)

This is the canonical business-rules document. Vocabulary is in
`docs/domain/ubiquitous-language.md`; the detailed model is in
`docs/architecture/domain-model-v0.2.md`.

Status values:

- **Confirmed** — explicitly stated business rule.
- **Assumption** — believed true but not yet confirmed by the business.
- **Open Question** — unresolved; must be clarified before changing behaviour.

Do not silently change a rule. When the code and a rule conflict, report it (see
`AGENTS.md` §D).

## Rules

| ID | Rule | Status | Evidence / source | Implementation | Related tests |
| --- | --- | --- | --- | --- | --- |
| BR-01 | Any employee may create a purchase request. | Confirmed | `docs/architecture/domain-model-v0.2.md` §2 | `CreatePurchaseRequestHandler` (no restriction) | `Create_with_valid_input_returns_a_draft_request` |
| BR-02 | A request is associated with its requester and department. | Confirmed | model docs | `PurchaseRequest` ctor (`RequesterId`, `DepartmentId`) | `Requester_id_must_not_be_empty`, `Department_id_must_not_be_empty` |
| BR-03 | A request requires at least one item before submission. | Confirmed | model docs | `PurchaseRequest.Submit()` | `Submit_without_items_is_rejected`, `Submit_with_at_least_one_item_succeeds` |
| BR-04 | Item description must be non-empty (trimmed). | Confirmed | item docs | `PurchaseRequestItem` ctor | `Description_must_not_be_empty_or_whitespace`, `Description_is_trimmed_when_valid` |
| BR-05 | Item quantity must be greater than zero. | Confirmed | item docs | `PurchaseRequestItem` ctor | `Quantity_must_be_greater_than_zero` |
| BR-06 | The initial request status is `Draft`. | Confirmed | model docs | `PurchaseRequest` ctor | `New_request_starts_in_draft_without_items` |
| BR-07 | The requester's department manager makes the approval decision. | Confirmed | model docs | **Not enforced** — `ApproverId` is supplied by the caller | — |
| BR-08 | Self-approval is currently allowed. | Confirmed | model docs | no guard on requester vs approver | `Self_approval_is_allowed_when_approver_is_the_requester` |
| BR-09 | `ReturnedForCorrection` is different from final `Rejected`. | Confirmed | model docs | `PurchaseRequestStatus` enum; distinct methods | `Submitted_request_can_be_returned_for_correction`, `Submitted_request_can_be_rejected` |
| BR-10 | A returned request can be corrected and resubmitted. | Confirmed | model docs | `AddItem` + `Submit` from `ReturnedForCorrection` | `Returned_request_can_be_corrected_and_resubmitted` |
| BR-11 | `Approved` and `Rejected` are final states. | Confirmed | model docs | transition guards | `Final_status_cannot_be_changed` |
| BR-12 | Only a `Submitted` request can receive a manager decision. | Confirmed | model docs | `EnsureSubmitted()` | `Decision_is_rejected_when_request_is_not_submitted` |
| BR-13 | Editing behaviour while a request is `Submitted` is not yet decided. | Open Question | ADR-0010 | temporary restriction (editing only in `Draft`/`ReturnedForCorrection`) | `Items_cannot_be_added_once_submitted` |
| BR-14 | The approver is assumed to be the department manager; verification is not implemented. | Assumption | README/current-state | **Not implemented** | — |

## Open questions (must be answered by the business)

1. Editing while `Submitted`: allowed? what may change? does it reset status?
2. Must corrections be audited (who / when / what)?
3. How is the approver resolved from organization data (and can it be overridden)?
4. Is self-approval permanent or a temporary convenience?
5. Should decisions carry a reason/comment and timestamp?
6. Can a `Rejected` request ever be revived?
7. Can items be removed/edited, or only appended?
8. Which context owns `Employee` / `Department`, and how are they referenced?

## Explicitly out of scope

Price, currency, tax, Supplier, Purchase Order, Invoice, Payment, Warehouse, and
cross-service integration are out of scope and intentionally absent from the
model and the language.
