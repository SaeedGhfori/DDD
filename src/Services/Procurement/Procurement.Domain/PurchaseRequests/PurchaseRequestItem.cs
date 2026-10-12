namespace Procurement.Domain.PurchaseRequests;

public sealed class PurchaseRequestItem
{
    private PurchaseRequestItem()
    {
    }

    public PurchaseRequestItem(Guid id, string description, int quantity, decimal unitPrice)
    {
        Id = id;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public Guid Id { get; private set; }

    public string Description { get; private set; } = string.Empty;

    public int Quantity { get; private set; }

    public decimal UnitPrice { get; private set; }

    public decimal LineTotal => Quantity * UnitPrice;
}
