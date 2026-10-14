namespace Procurement.Domain.PurchaseRequests.ValueObjects;

public readonly record struct Quantity
{
    private Quantity(int value) => Value = value;

    public int Value { get; }

    public static Quantity From(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Quantity must be greater than zero.");
        }

        return new Quantity(value);
    }

    public override string ToString() => Value.ToString();
}