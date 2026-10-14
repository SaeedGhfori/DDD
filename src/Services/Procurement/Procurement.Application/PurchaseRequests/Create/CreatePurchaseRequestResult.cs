using Procurement.Domain.PurchaseRequests.Enums;

namespace Procurement.Application.PurchaseRequests.Create;

public sealed record CreatePurchaseRequestResult(
    Guid PurchaseRequestId,
    PurchaseRequestStatus Status);