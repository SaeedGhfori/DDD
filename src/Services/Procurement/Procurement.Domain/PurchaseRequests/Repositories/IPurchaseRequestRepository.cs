using Procurement.Domain.PurchaseRequests.Aggregates;
using Procurement.Domain.PurchaseRequests.ValueObjects;

namespace Procurement.Domain.PurchaseRequests.Repositories;

public interface IPurchaseRequestRepository
{
    Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default);

    Task<PurchaseRequest?> GetByIdAsync(PurchaseRequestId id, CancellationToken cancellationToken = default);
}