using System.Collections.ObjectModel;

namespace Procurement.Domain.PurchaseRequests;

public sealed class PurchaseRequest
{
    private readonly List<PurchaseRequestItem> _items = [];
    private readonly ReadOnlyCollection<PurchaseRequestItem> _readOnlyItems;

    private PurchaseRequest()
    {
        _readOnlyItems = _items.AsReadOnly();
    }

    public PurchaseRequest(Guid id, Guid requesterId, Guid departmentId, Guid approverId)
        : this()
    {
        EnsureNotEmpty(id, nameof(id));
        EnsureNotEmpty(requesterId, nameof(requesterId));
        EnsureNotEmpty(departmentId, nameof(departmentId));
        EnsureNotEmpty(approverId, nameof(approverId));

        Id = id;
        RequesterId = requesterId;
        DepartmentId = departmentId;
        ApproverId = approverId;
        Status = PurchaseRequestStatus.Draft;
    }

    public Guid Id { get; private set; }

    public Guid RequesterId { get; private set; }

    public Guid DepartmentId { get; private set; }

    public Guid ApproverId { get; private set; }

    public PurchaseRequestStatus Status { get; private set; }

    public IReadOnlyCollection<PurchaseRequestItem> Items => _readOnlyItems;

    public void AddItem(PurchaseRequestItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status is not (PurchaseRequestStatus.Draft or PurchaseRequestStatus.ReturnedForCorrection))
        {
            throw new InvalidOperationException(
                $"Items can only be added while the request is in {PurchaseRequestStatus.Draft} or {PurchaseRequestStatus.ReturnedForCorrection} status.");
        }

        _items.Add(item);
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

    public void Approve()
    {
        EnsureSubmitted();
        Status = PurchaseRequestStatus.Approved;
    }

    public void ReturnForCorrection()
    {
        EnsureSubmitted();
        Status = PurchaseRequestStatus.ReturnedForCorrection;
    }

    public void Reject()
    {
        EnsureSubmitted();
        Status = PurchaseRequestStatus.Rejected;
    }

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
}
