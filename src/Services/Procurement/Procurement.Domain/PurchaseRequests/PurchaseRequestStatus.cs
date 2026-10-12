namespace Procurement.Domain.PurchaseRequests;

public enum PurchaseRequestStatus
{
    Draft = 0,
    Submitted = 1,
    ReturnedForCorrection = 2,
    Approved = 3,
    Rejected = 4
}
