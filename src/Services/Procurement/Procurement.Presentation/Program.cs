using Procurement.Application.PurchaseRequests.Create;
using Procurement.Presentation.PurchaseRequests;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddScoped<CreatePurchaseRequestHandler>();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Procurement", status = "running" }));

app.MapHealthChecks("/health");

app.MapCreatePurchaseRequestEndpoint();

app.Run();

public partial class Program { }