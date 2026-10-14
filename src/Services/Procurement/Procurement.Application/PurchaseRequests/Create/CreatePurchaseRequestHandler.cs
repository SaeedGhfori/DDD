using Procurement.Domain.PurchaseRequests.Aggregates;

namespace Procurement.Application.PurchaseRequests.Create;

public sealed class CreatePurchaseRequestHandler
{
    public CreatePurchaseRequestResult Handle(CreatePurchaseRequestCommand command)
    {
        ArgumentNullException.ThrowIfNull(command);

        var purchaseRequest = PurchaseRequest.Create(
            command.RequesterId,
            command.DepartmentId,
            command.ApproverId);

        return new CreatePurchaseRequestResult(purchaseRequest.Id.Value, purchaseRequest.Status);
    }
}