namespace Procurement.Application.PurchaseRequests.Create;

public sealed record CreatePurchaseRequestCommand(
    Guid RequesterId,
    Guid DepartmentId,
    Guid ApproverId);