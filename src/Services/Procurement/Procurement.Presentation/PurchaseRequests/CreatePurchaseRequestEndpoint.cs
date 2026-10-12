using Procurement.Application.PurchaseRequests.Create;

namespace Procurement.Presentation.PurchaseRequests;

public sealed record CreatePurchaseRequestRequest(
    Guid RequesterId,
    Guid DepartmentId,
    Guid ApproverId);

public sealed record CreatePurchaseRequestResponse(
    Guid PurchaseRequestId,
    string Status);

public static class CreatePurchaseRequestEndpoint
{
    public static IEndpointRouteBuilder MapCreatePurchaseRequestEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/purchase-requests", (
            CreatePurchaseRequestRequest request,
            CreatePurchaseRequestHandler handler) =>
        {
            if (request is null)
            {
                return Results.BadRequest(new { error = "A request body is required." });
            }

            try
            {
                var result = handler.Handle(new CreatePurchaseRequestCommand(
                    request.RequesterId,
                    request.DepartmentId,
                    request.ApproverId));

                return Results.Created(
                    $"/api/purchase-requests/{result.PurchaseRequestId}",
                    new CreatePurchaseRequestResponse(result.PurchaseRequestId, result.Status.ToString()));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { error = ex.Message });
            }
        });

        return endpoints;
    }
}