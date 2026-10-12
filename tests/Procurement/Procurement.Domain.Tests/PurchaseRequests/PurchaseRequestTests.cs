using FluentAssertions;
using Procurement.Domain.PurchaseRequests;
using Xunit;

namespace Procurement.Domain.Tests;

public sealed class PurchaseRequestTests
{
    [Fact]
    public void New_purchase_request_starts_as_draft_without_items()
    {
        var request = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        request.Status.Should().Be(PurchaseRequestStatus.Draft);
        request.Items.Should().BeEmpty();
        request.TotalAmount.Should().Be(0m);
    }

    [Fact]
    public void Adding_items_updates_total_amount()
    {
        var request = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        request.AddItem(new PurchaseRequestItem(Guid.NewGuid(), "Laptop", 2, 1500m));
        request.AddItem(new PurchaseRequestItem(Guid.NewGuid(), "Monitor", 3, 300m));

        request.Items.Should().HaveCount(2);
        request.TotalAmount.Should().Be(3900m);
    }

    [Fact]
    public void Submit_moves_request_from_draft_to_submitted()
    {
        var request = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);

        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
    }

    [Fact]
    public void Submit_twice_is_not_allowed()
    {
        var request = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow);
        request.Submit();

        var act = () => request.Submit();

        act.Should().Throw<InvalidOperationException>();
    }
}
