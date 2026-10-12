var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new { service = "Notification", status = "running" }));

app.MapHealthChecks("/health");

app.Run();
