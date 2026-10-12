using Procurement.Domain.PurchaseRequests;

namespace Procurement.Application.PurchaseRequests.Create;

public sealed class CreatePurchaseRequestHandler
{
    public CreatePurchaseRequestResult Handle(CreatePurchaseRequestCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var purchaseRequest = new PurchaseRequest(
            Guid.NewGuid(),
            command.RequesterId,
            command.DepartmentId,
            command.ApproverId);

        return new CreatePurchaseRequestResult(purchaseRequest.Id, purchaseRequest.Status);
    }
}