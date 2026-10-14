using Procurement.Domain.PurchaseRequests.Entities;
using Procurement.Domain.PurchaseRequests.Enums;
using Procurement.Domain.PurchaseRequests.ValueObjects;

namespace Procurement.Domain.PurchaseRequests.Aggregates;

public sealed class PurchaseRequest : IEquatable<PurchaseRequest>
{
    private readonly List<PurchaseRequestItem> _items = [];

    private PurchaseRequest()
    {
    }

    private PurchaseRequest(
        PurchaseRequestId id,
        Guid requesterId,
        Guid departmentId,
        Guid approverId,
        PurchaseRequestStatus status)
    {
        Id = id;
        RequesterId = requesterId;
        DepartmentId = departmentId;
        ApproverId = approverId;
        Status = status;
    }

    public PurchaseRequestId Id { get; private set; }

    public Guid RequesterId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid ApproverId { get; private set; }

    public PurchaseRequestStatus Status { get; private set; }

    public IReadOnlyCollection<PurchaseRequestItem> Items => _items;

    public static PurchaseRequest Create(Guid requesterId, Guid departmentId, Guid approverId)
    {
        EnsureNotEmpty(requesterId, nameof(requesterId));
        EnsureNotEmpty(departmentId, nameof(departmentId));
        EnsureNotEmpty(approverId, nameof(approverId));

        return new PurchaseRequest(
            PurchaseRequestId.New(),
            requesterId,
            departmentId,
            approverId,
            PurchaseRequestStatus.Draft);
    }

    public static PurchaseRequest Reconstitute(
        PurchaseRequestId id,
        Guid requesterId,
        Guid departmentId,
        Guid approverId,
        PurchaseRequestStatus status,
        IEnumerable<PurchaseRequestItem> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        EnsureNotEmpty(requesterId, nameof(requesterId));
        EnsureNotEmpty(departmentId, nameof(departmentId));
        EnsureNotEmpty(approverId, nameof(approverId));

        var request = new PurchaseRequest(id, requesterId, departmentId, approverId, status);
        request._items.AddRange(items);
        return request;
    }

    public PurchaseRequestItem AddItem(string description, Quantity quantity)
    {
        if (Status is not (PurchaseRequestStatus.Draft or PurchaseRequestStatus.ReturnedForCorrection))
        {
            throw new InvalidOperationException(
                $"Items can only be added while the request is in {PurchaseRequestStatus.Draft} or {PurchaseRequestStatus.ReturnedForCorrection} status.");
        }

        var item = new PurchaseRequestItem(PurchaseRequestItemId.New(), description, quantity);
        _items.Add(item);
        return item;
    }

    public void Submit()
    {
        if (Status is not (PurchaseRequestStatus.Draft or PurchaseRequestStatus.ReturnedForCorrection))
        {
            throw new InvalidOperationException(
                $"Only {PurchaseRequestStatus.Draft} or {PurchaseRequestStatus.ReturnedForCorrection} requests can be submitted.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("A purchase request must contain at least one item before it can be submitted.");
        }

        Status = PurchaseRequestStatus.Submitted;
    }

    public void Approve(Guid actorId)
    {
        EnsureSubmitted();
        EnsureActorIsApprover(actorId);
        Status = PurchaseRequestStatus.Approved;
    }

    public void ReturnForCorrection(Guid actorId)
    {
        EnsureSubmitted();
        EnsureActorIsApprover(actorId);
        Status = PurchaseRequestStatus.ReturnedForCorrection;
    }

    public void Reject(Guid actorId)
    {
        EnsureSubmitted();
        EnsureActorIsApprover(actorId);
        Status = PurchaseRequestStatus.Rejected;
    }

    public bool Equals(PurchaseRequest? other) =>
        other is not null && GetType() == other.GetType() && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as PurchaseRequest);

    public override int GetHashCode() => Id.GetHashCode();

    private static void EnsureNotEmpty(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Identifier must not be empty.", parameterName);
        }
    }

    private void EnsureSubmitted()
    {
        if (Status != PurchaseRequestStatus.Submitted)
        {
            throw new InvalidOperationException(
                $"This decision is only allowed while the request is in {PurchaseRequestStatus.Submitted} status.");
        }
    }

    private void EnsureActorIsApprover(Guid actorId)
    {
        if (actorId != ApproverId)
        {
            throw new UnauthorizedAccessException("Only the assigned approver may decide on this request.");
        }
    }
}