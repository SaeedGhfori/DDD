using Microsoft.EntityFrameworkCore;
using Procurement.Domain.PurchaseRequests.Aggregates;
using Procurement.Domain.PurchaseRequests.Repositories;
using Procurement.Domain.PurchaseRequests.ValueObjects;

namespace Procurement.Persistence.Repositories;

public sealed class PurchaseRequestRepository : IPurchaseRequestRepository
{
    private readonly ProcurementDbContext _context;

    public PurchaseRequestRepository(ProcurementDbContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        _context = context;
    }

    public async Task AddAsync(PurchaseRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _context.PurchaseRequests.AddAsync(request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PurchaseRequest?> GetByIdAsync(PurchaseRequestId id, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseRequests
            .FirstOrDefaultAsync(request => request.Id == id, cancellationToken);
    }
}
