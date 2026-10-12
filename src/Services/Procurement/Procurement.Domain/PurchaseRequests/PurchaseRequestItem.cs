namespace Procurement.Domain.PurchaseRequests;

public sealed class PurchaseRequestItem
{
    private PurchaseRequestItem()
    {
    }

    public PurchaseRequestItem(Guid id, string description, int quantity)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Item id must not be empty.", nameof(id));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Description must not be empty or whitespace.", nameof(description));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), quantity, "Quantity must be greater than zero.");
        }

        Id = id;
        Description = description.Trim();
        Quantity = quantity;
    }

    public Guid Id { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }
}
