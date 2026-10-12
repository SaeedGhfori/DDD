namespace Procurement.Domain.PurchaseRequests;

public sealed class PurchaseRequest
{
    private readonly List<PurchaseRequestItem> _items = [];

    private PurchaseRequest()
    {
    }

    public PurchaseRequest(Guid id, Guid requesterId, DateTime createdAtUtc)
    {
        Id = id;
        RequesterId = requesterId;
        CreatedAtUtc = createdAtUtc;
        Status = PurchaseRequestStatus.Draft;
    }

    public Guid Id { get; private set; }

    public Guid RequesterId { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public PurchaseRequestStatus Status { get; private set; }

    public IReadOnlyCollection<PurchaseRequestItem> Items => _items;

    public decimal TotalAmount => _items.Sum(item => item.LineTotal);

    public void AddItem(PurchaseRequestItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _items.Add(item);
    }

    public void Submit()
    {
        if (Status != PurchaseRequestStatus.Draft)
        {
            throw new InvalidOperationException("Only draft purchase requests can be submitted.");
        }

        Status = PurchaseRequestStatus.Submitted;
    }
}
