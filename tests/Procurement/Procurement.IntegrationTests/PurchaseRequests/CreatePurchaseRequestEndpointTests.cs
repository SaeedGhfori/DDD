using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Procurement.IntegrationTests.PurchaseRequests;

public sealed class CreatePurchaseRequestEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CreatePurchaseRequestEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_returns_201_with_draft_status_and_request_id()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            requesterId = Guid.NewGuid(),
            departmentId = Guid.NewGuid(),
            approverId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync("/api/purchase-requests", payload);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("purchaseRequestId").GetGuid().Should().NotBeEmpty();
        body.GetProperty("status").GetString().Should().Be("Draft");
    }

    [Fact]
    public async Task Post_with_empty_requester_id_returns_400()
    {
        var client = _factory.CreateClient();
        var payload = new
        {
            requesterId = Guid.Empty,
            departmentId = Guid.NewGuid(),
            approverId = Guid.NewGuid()
        };

        var response = await client.PostAsJsonAsync("/api/purchase-requests", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}