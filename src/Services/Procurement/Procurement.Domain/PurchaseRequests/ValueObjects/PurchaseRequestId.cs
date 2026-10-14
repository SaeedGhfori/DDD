namespace Procurement.Domain.PurchaseRequests.ValueObjects;

public readonly record struct PurchaseRequestId
{
    private PurchaseRequestId(Guid value) => Value = value;

    public Guid Value { get; }

    public static PurchaseRequestId New() => new(Guid.NewGuid());

    public static PurchaseRequestId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Purchase request id must not be empty.", nameof(value));
        }

        return new PurchaseRequestId(value);
    }

    public override string ToString() => Value.ToString();
}