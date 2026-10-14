using FluentAssertions;
using Procurement.Domain.PurchaseRequests.Aggregates;
using Procurement.Domain.PurchaseRequests.Entities;
using Procurement.Domain.PurchaseRequests.Enums;
using Procurement.Domain.PurchaseRequests.ValueObjects;
using Xunit;

namespace Procurement.Domain.Tests.PurchaseRequests;

public sealed class PurchaseRequestTests
{
    [Fact]
    public void New_request_starts_in_draft_without_items()
    {
        var request = CreateDraft();

        request.Id.Value.Should().NotBeEmpty();
        request.Status.Should().Be(PurchaseRequestStatus.Draft);
        request.Items.Should().BeEmpty();
    }

    [Fact]
    public void Requester_id_must_not_be_empty()
    {
        var act = () => PurchaseRequest.Create(Guid.Empty, Guid.NewGuid(), Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Department_id_must_not_be_empty()
    {
        var act = () => PurchaseRequest.Create(Guid.NewGuid(), Guid.Empty, Guid.NewGuid());

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Approver_id_must_not_be_empty()
    {
        var act = () => PurchaseRequest.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Valid_item_can_be_added_to_draft_request()
    {
        var request = CreateDraft();

        var item = request.AddItem("Laptop", Quantity.From(1));

        request.Items.Should().ContainSingle().Which.Should().Be(item);
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
        request.AddItem("Laptop", Quantity.From(1));

        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
    }

    [Fact]
    public void Submitted_request_can_be_approved()
    {
        var request = CreateSubmitted();

        request.Approve(request.ApproverId);

        request.Status.Should().Be(PurchaseRequestStatus.Approved);
    }

    [Fact]
    public void Submitted_request_can_be_returned_for_correction()
    {
        var request = CreateSubmitted();

        request.ReturnForCorrection(request.ApproverId);

        request.Status.Should().Be(PurchaseRequestStatus.ReturnedForCorrection);
    }

    [Fact]
    public void Submitted_request_can_be_rejected()
    {
        var request = CreateSubmitted();

        request.Reject(request.ApproverId);

        request.Status.Should().Be(PurchaseRequestStatus.Rejected);
    }

    [Fact]
    public void Returned_request_can_be_corrected_and_resubmitted()
    {
        var request = CreateSubmitted();
        request.ReturnForCorrection(request.ApproverId);

        request.AddItem("Monitor", Quantity.From(1));
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

        var act = () => Invoke(request, operation, request.ApproverId);

        act.Should().Throw<InvalidOperationException>();
        request.Status.Should().Be(PurchaseRequestStatus.Draft);
    }

    [Fact]
    public void Items_cannot_be_added_once_submitted()
    {
        var request = CreateSubmitted();

        var act = () => request.AddItem("Laptop", Quantity.From(1));

        act.Should().Throw<InvalidOperationException>();
        request.Items.Should().ContainSingle();
    }

    [Theory]
    [InlineData(nameof(PurchaseRequest.Submit))]
    [InlineData(nameof(PurchaseRequest.Approve))]
    [InlineData(nameof(PurchaseRequest.ReturnForCorrection))]
    [InlineData(nameof(PurchaseRequest.Reject))]
    public void Final_status_cannot_be_changed(string operation)
    {
        var approved = CreateSubmitted();
        approved.Approve(approved.ApproverId);
        var approveAct = () => Invoke(approved, operation, approved.ApproverId);
        approveAct.Should().Throw<InvalidOperationException>();
        approved.Status.Should().Be(PurchaseRequestStatus.Approved);

        var rejected = CreateSubmitted();
        rejected.Reject(rejected.ApproverId);
        var rejectAct = () => Invoke(rejected, operation, rejected.ApproverId);
        rejectAct.Should().Throw<InvalidOperationException>();
        rejected.Status.Should().Be(PurchaseRequestStatus.Rejected);
    }

    [Fact]
    public void Self_approval_is_allowed_when_approver_is_the_requester()
    {
        var employeeId = Guid.NewGuid();
        var request = PurchaseRequest.Create(employeeId, Guid.NewGuid(), employeeId);
        request.AddItem("Laptop", Quantity.From(1));
        request.Submit();

        request.Approve(employeeId);

        request.Status.Should().Be(PurchaseRequestStatus.Approved);
    }

    [Theory]
    [InlineData(nameof(PurchaseRequest.Approve))]
    [InlineData(nameof(PurchaseRequest.ReturnForCorrection))]
    [InlineData(nameof(PurchaseRequest.Reject))]
    public void Decision_by_non_approver_is_rejected(string operation)
    {
        var request = CreateSubmitted();

        var act = () => Invoke(request, operation, Guid.NewGuid());

        act.Should().Throw<UnauthorizedAccessException>();
        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
    }

    [Fact]
    public void First_scenario_create_add_two_items_and_submit()
    {
        var request = PurchaseRequest.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        request.AddItem("Laptop", Quantity.From(1));
        request.AddItem("Docking station", Quantity.From(2));

        request.Submit();

        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
        request.Items.Should().HaveCount(2);
    }

    [Fact]
    public void Requests_with_the_same_id_are_equal()
    {
        var id = PurchaseRequestId.New();
        var first = PurchaseRequest.Reconstitute(
            id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PurchaseRequestStatus.Draft, []);
        var second = PurchaseRequest.Reconstitute(
            id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PurchaseRequestStatus.Approved, []);

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Reconstitute_restores_status_and_items()
    {
        var item = new PurchaseRequestItem(PurchaseRequestItemId.New(), "Laptop", Quantity.From(3));
        var id = PurchaseRequestId.New();

        var request = PurchaseRequest.Reconstitute(
            id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), PurchaseRequestStatus.Submitted, [item]);

        request.Id.Should().Be(id);
        request.Status.Should().Be(PurchaseRequestStatus.Submitted);
        request.Items.Should().ContainSingle().Which.Should().Be(item);
    }

    private static PurchaseRequest CreateDraft() =>
        PurchaseRequest.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

    private static PurchaseRequest CreateSubmitted()
    {
        var request = CreateDraft();
        request.AddItem("Laptop", Quantity.From(1));
        request.Submit();
        return request;
    }

    private static void Invoke(PurchaseRequest request, string operation, Guid actorId)
    {
        switch (operation)
        {
            case nameof(PurchaseRequest.Submit):
                request.Submit();
                break;
            case nameof(PurchaseRequest.Approve):
                request.Approve(actorId);
                break;
            case nameof(PurchaseRequest.ReturnForCorrection):
                request.ReturnForCorrection(actorId);
                break;
            case nameof(PurchaseRequest.Reject):
                request.Reject(actorId);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(operation), operation, "Unknown operation.");
        }
    }
}
