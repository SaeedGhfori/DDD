# Ubiquitous Language — Procurement (canonical)

Version: v0.2
Scope: the `Procurement` microservice, initial domain modeling.

This glossary is the shared vocabulary between the business and the engineering
team. A business term is **not** automatically a class; the "Model concept?"
column states whether the term exists as a first-class concept, a role, or an
external reference.

---

## Core terms

| English term | Persian | Definition | Distinction | Model concept? | Code reference |
| --- | --- | --- | --- | --- | --- |
| Employee | کارمند | A member of the organization who can act in the system. | Generic; not tied to a specific request. | External concept (other service/context). | referenced by `RequesterId` / `ApproverId` |
| Requester | درخواست‌کننده | The employee who creates and owns a purchase request. | A **role** an Employee plays on a specific request, not a separate entity. | Role. | `PurchaseRequest.RequesterId` |
| Department | واحد سازمانی | An organizational unit a request belongs to. | Owns a budget/manager; those concerns are outside this scope. | External concept. | `PurchaseRequest.DepartmentId` |
| Department Manager | مدیر واحد | The manager responsible for approving requests of a department. | A **role** (decision maker); currently equal to the approver. | Role. | `PurchaseRequest.ApproverId` (BR-07, BR-14) |
| Purchase Request | درخواست خرید | A request to purchase one or more items, moving through a review lifecycle. | Aggregate root of this context. | Yes — aggregate root. | `PurchaseRequest` |
| Purchase Request Item | قلم درخواست | A single line of a purchase request. | Belongs to exactly one request. | Yes — child entity. | `PurchaseRequestItem` |

## Status terms

| English term | Persian | Definition | Distinction | Model concept? | Code reference |
| --- | --- | --- | --- | --- | --- |
| Draft | پیش‌نویس | Initial state; request is being prepared and is editable. | One of the two editable states. | Yes — status value. | `PurchaseRequestStatus.Draft` |
| Submitted | ارسال‌شده | Request has been sent to the manager and awaits a decision. | Not editable under the current temporary rule (BR-13). | Yes — status value. | `PurchaseRequestStatus.Submitted` |
| Approved | تأییدشده | Manager approved the request. | A **final** state. | Yes — status value. | `PurchaseRequestStatus.Approved` |
| Rejected | رد نهایی | Manager rejected the request. | A **final** state and **distinct** from Returned for Correction. | Yes — status value. | `PurchaseRequestStatus.Rejected` |
| ReturnedForCorrection | بازگشت برای اصلاح | Manager sent the request back so the requester can fix it. | Not final; can be resubmitted. Different from Rejected. | Yes — status value. | `PurchaseRequestStatus.ReturnedForCorrection` |

## Action terms

| English term | Persian | Definition | Distinction | Model concept? | Code reference |
| --- | --- | --- | --- | --- | --- |
| Submit | ارسال برای بررسی | Send a valid request for review. | Also used to resubmit from `ReturnedForCorrection`. | Method. | `PurchaseRequest.Submit()` |
| Approve | تأیید | Approve a submitted request. | Only valid from `Submitted`. | Method. | `PurchaseRequest.Approve()` |
| Reject | رد نهایی | Finally reject a submitted request. | Only valid from `Submitted`; leads to a final state. | Method. | `PurchaseRequest.Reject()` |
| Return for Correction | بازگرداندن برای اصلاح | Return a submitted request for correction. | Only valid from `Submitted`; not final. | Method. | `PurchaseRequest.ReturnForCorrection()` |
| Resubmit | ارسال مجدد | Submit again after corrections. | **Not a separate operation** — it is `Submit` applied while `ReturnedForCorrection`. | Same as Submit. | `PurchaseRequest.Submit()` |
| Add item | افزودن قلم | Add a line to the request. | Only in `Draft` / `ReturnedForCorrection` (BR-13). | Method. | `PurchaseRequest.AddItem(...)` |

---

## Important distinctions

- **Rejected vs ReturnedForCorrection** — `Rejected` is terminal; `ReturnedForCorrection`
  is a "please fix and resend" signal that keeps the request alive.
- **Employee vs Requester / Department Manager** — `Employee` is the general
  member; `Requester` and `Department Manager` are *roles*.
- **Submit vs Resubmit** — two words, one operation with two legal start states.
- **Approver (role) vs Approve (action)** — the role is `ApproverId`; the action
  is the `Approve()` method.
- **Approval (concept)** — today only `Status = Approved` + `ApproverId`. A richer
  Approval record (who/when/comment) is **not modeled** (open question).

## Ambiguous / unresolved terminology

- The exact meaning of "editing" while `Submitted` is undefined (BR-13).
- "Manager" resolution is not modeled (BR-07, BR-14).

## Explicitly out of vocabulary for now

Price, currency, tax, Supplier, Purchase Order, Invoice, Payment, Warehouse —
**out of scope** in v0.2.
