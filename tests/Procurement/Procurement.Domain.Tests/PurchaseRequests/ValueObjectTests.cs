using FluentAssertions;
using Procurement.Domain.PurchaseRequests.ValueObjects;
using Xunit;

namespace Procurement.Domain.Tests.PurchaseRequests;

public sealed class ValueObjectTests
{
    [Fact]
    public void Purchase_request_id_new_is_not_empty()
    {
        PurchaseRequestId.New().Value.Should().NotBeEmpty();
    }

    [Fact]
    public void Purchase_request_id_from_empty_guid_is_rejected()
    {
        var act = () => PurchaseRequestId.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Purchase_request_ids_with_the_same_value_are_equal()
    {
        var value = Guid.NewGuid();

        PurchaseRequestId.From(value).Should().Be(PurchaseRequestId.From(value));
    }

    [Fact]
    public void Item_id_from_empty_guid_is_rejected()
    {
        var act = () => PurchaseRequestItemId.From(Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void Quantity_must_be_greater_than_zero(int value)
    {
        var act = () => Quantity.From(value);

        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Quantity_with_a_positive_value_is_valid()
    {
        Quantity.From(3).Value.Should().Be(3);
    }
}
