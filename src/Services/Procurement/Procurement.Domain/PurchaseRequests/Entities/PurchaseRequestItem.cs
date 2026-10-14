using Procurement.Domain.PurchaseRequests.ValueObjects;

namespace Procurement.Domain.PurchaseRequests.Entities;

public sealed class PurchaseRequestItem : IEquatable<PurchaseRequestItem>
{
    private PurchaseRequestItem()
    {
    }

    public PurchaseRequestItem(PurchaseRequestItemId id, string description, Quantity quantity)
    {
        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be empty or whitespace.", nameof(description));
        }

        Id = id;
        Description = description.Trim();
        Quantity = quantity;
    }

    public PurchaseRequestItemId Id { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public Quantity Quantity { get; private set; }

    public bool Equals(PurchaseRequestItem? other) =>
        other is not null && GetType() == other.GetType() && Id.Equals(other.Id);

    public override bool Equals(object? obj) => Equals(obj as PurchaseRequestItem);

    public override int GetHashCode() => Id.GetHashCode();
}