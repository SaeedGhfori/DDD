using Procurement.Domain.PurchaseRequests;

namespace Procurement.Application.PurchaseRequests.Create;

public sealed record CreatePurchaseRequestResult(
    Guid PurchaseRequestId,
    PurchaseRequestStatus Status);