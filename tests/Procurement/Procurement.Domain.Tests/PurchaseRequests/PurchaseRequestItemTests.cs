using FluentAssertions;
using Procurement.Domain.PurchaseRequests;
using Xunit;

namespace Procurement.Domain.Tests.PurchaseRequests;

public sealed class PurchaseRequestItemTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Description_must_not_be_empty_or_whitespace(string description)
    {
        var act = () => new PurchaseRequestItem(Guid.NewGuid(), description, 1);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Quantity_must_be_greater_than_zero(int quantity)
    {
        var act = () => new PurchaseRequestItem(Guid.NewGuid(), "Laptop", quantity);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Empty_item_id_is_rejected()
    {
        var act = () => new PurchaseRequestItem(Guid.Empty, "Laptop", 1);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Description_is_trimmed_when_valid()
    {
        var item = new PurchaseRequestItem(Guid.NewGuid(), "  Laptop  ", 2);

        item.Description.Should().Be("Laptop");
        item.Quantity.Should().Be(2);
    }
}
