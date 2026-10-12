using FluentAssertions;
using Procurement.Domain.PurchaseRequests;
using Xunit;

namespace Procurement.Domain.Tests.PurchaseRequests;

public sealed class PurchaseRequestTests
{
    [Fact]
    public void New_request_starts_in_draft_without_items()
    {
        var request = CreateDraft();

        request.Status.Should().Be(PurchaseRequestStatus.Draft);
        request.Items.Should().BeEmpty();
    }

    [Fact]
    public void Requester_id_must_not_be_empty()
    {
        var act = () => new PurchaseRequest(Guid.NewGuid(), Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Department_id_must_not_be_empty()
    {
        var act = () => new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approver_id_must_not_be_empty()
    {
        var act = () => new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Valid_item_can_be_added_to_draft_request()
    {
        var request = CreateDraft();

        request.AddItem(CreateItem());

        request.Items.Should().ContainSingle();
        request.Status.Should().Be(PurchaseRequestStatus.Draft);
    }

    [Fact]
    public void Submit_without_items_is_rejected()
    {
        var request = CreateDraft();

        var act = () => request.Submit();

        act.Should().Throw<InvalidOperationException>();
        request.Status.Should().Be(PurchaseRequestStatus.Draft);
    }

    [Fact]
    public void Submit_with_at_least_one_item_succeeds()
    {
        var request = CreateDraft();
        request.AddItem(CreateItem());

        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
    }

    [Fact]
    public void Submitted_request_can_be_approved()
    {
        var request = CreateSubmitted();

        request.Approve();

        request.Status.Should().Be(PurchaseRequestStatus.Approved);
    }

    [Fact]
    public void Submitted_request_can_be_returned_for_correction()
    {
        var request = CreateSubmitted();

        request.ReturnForCorrection();

        request.Status.Should().Be(PurchaseRequestStatus.ReturnedForCorrection);
    }

    [Fact]
    public void Submitted_request_can_be_rejected()
    {
        var request = CreateSubmitted();

        request.Reject();

        request.Status.Should().Be(PurchaseRequestStatus.Rejected);
    }

    [Fact]
    public void Returned_request_can_be_corrected_and_resubmitted()
    {
        var request = CreateSubmitted();
        request.ReturnForCorrection();

        request.AddItem(CreateItem("Monitor"));
        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
        request.Items.Should().HaveCount(2);
    }

    [Theory]
    [InlineData(nameof(PurchaseRequest.Approve))]
    [InlineData(nameof(PurchaseRequest.ReturnForCorrection))]
    [InlineData(nameof(PurchaseRequest.Reject))]
    public void Decision_is_rejected_when_request_is_not_submitted(string operation)
    {
        var request = CreateDraft();

        var act = () => Invoke(request, operation);

        act.Should().Throw<InvalidOperationException>();
        request.Status.Should().Be(PurchaseRequestStatus.Draft);
    }

    [Fact]
    public void Items_cannot_be_added_once_submitted()
    {
        var request = CreateSubmitted();

        var act = () => request.AddItem(CreateItem());

        act.Should().Throw<InvalidOperationException>();
        request.Items.Should().ContainSingle();
    }

    [Theory]
    [InlineData(nameof(PurchaseRequest.Submit))]
    [InlineData(nameof(PurchaseRequest.Approve))]
    [InlineData(nameof(PurchaseRequest.ReturnForCorrection))]
    [InlineData(nameof(PurchaseRequest.Reject))]
    [InlineData(nameof(PurchaseRequest.AddItem))]
    public void Final_status_cannot_be_changed(string operation)
    {
        var approved = CreateSubmitted();
        approved.Approve();
        var approveAct = () => Invoke(approved, operation);
        approveAct.Should().Throw<InvalidOperationException>();
        approved.Status.Should().Be(PurchaseRequestStatus.Approved);

        var rejected = CreateSubmitted();
        rejected.Reject();
        var rejectAct = () => Invoke(rejected, operation);
        rejectAct.Should().Throw<InvalidOperationException>();
        rejected.Status.Should().Be(PurchaseRequestStatus.Rejected);
    }

    [Fact]
    public void Self_approval_is_allowed_when_approver_is_the_requester()
    {
        var employeeId = Guid.NewGuid();
        var request = new PurchaseRequest(Guid.NewGuid(), employeeId, Guid.NewGuid(), employeeId);
        request.AddItem(CreateItem());
        request.Submit();

        request.Approve();

        request.Status.Should().Be(PurchaseRequestStatus.Approved);
    }

    [Fact]
    public void First_scenario_create_add_two_items_and_submit()
    {
        var request = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        request.AddItem(new PurchaseRequestItem(Guid.NewGuid(), "Laptop", 1));
        request.AddItem(new PurchaseRequestItem(Guid.NewGuid(), "Docking station", 2));

        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
        request.Items.Should().HaveCount(2);
    }

    private static PurchaseRequest CreateDraft() =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private static PurchaseRequest CreateSubmitted()
    {
        var request = CreateDraft();
        request.AddItem(CreateItem());
        request.Submit();
        return request;
    }

    private static PurchaseRequestItem CreateItem(string description = "Laptop") =>
        new(Guid.NewGuid(), description, 1);

    private static void Invoke(PurchaseRequest request, string operation)
    {
        switch (operation)
        {
            case nameof(PurchaseRequest.Submit):
                request.Submit();
                break;
            case nameof(PurchaseRequest.Approve):
                request.Approve();
                break;
            case nameof(PurchaseRequest.ReturnForCorrection):
                request.ReturnForCorrection();
                break;
            case nameof(PurchaseRequest.Reject):
                request.Reject();
                break;
            case nameof(PurchaseRequest.AddItem):
                request.AddItem(CreateItem());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, "Unknown operation.");
        }
    }
}
