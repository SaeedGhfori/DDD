using FluentAssertions;
using Procurement.Domain.PurchaseRequests.Entities;
using Procurement.Domain.PurchaseRequests.ValueObjects;
using Xunit;

namespace Procurement.Domain.Tests.PurchaseRequests;

public sealed class PurchaseRequestItemTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Description_must_not_be_empty_or_whitespace(string description)
    {
        var act = () => new PurchaseRequestItem(PurchaseRequestItemId.New(), description, Quantity.From(1));

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Description_is_trimmed_when_valid()
    {
        var item = new PurchaseRequestItem(PurchaseRequestItemId.New(), "  Laptop  ", Quantity.From(2));

        item.Description.Should().Be("Laptop");
        item.Quantity.Value.Should().Be(2);
    }

    [Fact]
    public void Items_with_the_same_id_are_equal()
    {
        var id = PurchaseRequestItemId.New();
        var first = new PurchaseRequestItem(id, "Laptop", Quantity.From(1));
        var second = new PurchaseRequestItem(id, "Monitor", Quantity.From(5));

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Items_with_different_ids_are_not_equal()
    {
        var first = new PurchaseRequestItem(PurchaseRequestItemId.New(), "Laptop", Quantity.From(1));
        var second = new PurchaseRequestItem(PurchaseRequestItemId.New(), "Laptop", Quantity.From(1));

        first.Should().NotBe(second);
    }
}
