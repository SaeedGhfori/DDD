using FluentAssertions;
using Procurement.Application.PurchaseRequests.Create;
using Procurement.Domain.PurchaseRequests.Enums;
using Xunit;

namespace Procurement.Application.Tests.PurchaseRequests;

public sealed class CreatePurchaseRequestHandlerTests
{
    private readonly CreatePurchaseRequestHandler _handler = new();

    [Fact]
    public void Create_with_valid_input_returns_a_draft_request()
    {
        var command = new CreatePurchaseRequestCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var result = _handler.Handle(command);

        result.Status.Should().Be(PurchaseRequestStatus.Draft);
    }

    [Fact]
    public void Create_returns_a_new_non_empty_request_id()
    {
        var command = new CreatePurchaseRequestCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());

        var result = _handler.Handle(command);

        result.PurchaseRequestId.Should().NotBeEmpty();
        result.PurchaseRequestId.Should()
            .NotBe(command.RequesterId)
            .And.NotBe(command.DepartmentId)
            .And.NotBe(command.ApproverId);
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Create_rejects_empty_required_ids(bool emptyRequester, bool emptyDepartment, bool emptyApprover)
    {
        var command = new CreatePurchaseRequestCommand(
            emptyRequester ? Guid.Empty : Guid.NewGuid(),
            emptyDepartment ? Guid.Empty : Guid.NewGuid(),
            emptyApprover ? Guid.Empty : Guid.NewGuid());

        var act = () => _handler.Handle(command);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Create_does_not_bypass_domain_validation()
    {
        var command = new CreatePurchaseRequestCommand(Guid.NewGuid(), Guid.NewGuid(), Guid.Empty);

        var act = () => _handler.Handle(command);

        act.Should().Throw<ArgumentException>()
            .Which.ParamName.Should().Be("approverId");
    }

    [Fact]
    public void Create_rejects_a_null_command()
    {
        var act = () => _handler.Handle(null!);

        act.Should().Throw<ArgumentNullException>();
    }
}