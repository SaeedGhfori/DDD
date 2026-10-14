namespace Procurement.Domain.PurchaseRequests.ValueObjects;

public readonly record struct PurchaseRequestItemId
{
    private PurchaseRequestItemId(Guid value) => Value = value;

    public Guid Value { get; }

    public static PurchaseRequestItemId New() => new(Guid.NewGuid());

    public static PurchaseRequestItemId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Item id must not be empty.", nameof(value));
        }

        return new PurchaseRequestItemId(value);
    }

    public override string ToString() => Value.ToString();
}