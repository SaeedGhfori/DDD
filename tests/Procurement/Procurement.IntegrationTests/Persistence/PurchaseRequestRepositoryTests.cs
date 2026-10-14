using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Procurement.Domain.PurchaseRequests.Aggregates;
using Procurement.Domain.PurchaseRequests.Enums;
using Procurement.Domain.PurchaseRequests.ValueObjects;
using Procurement.Persistence;
using Procurement.Persistence.Repositories;
using Xunit;

namespace Procurement.IntegrationTests.Persistence;

public sealed class PurchaseRequestRepositoryTests
{
    private static DbContextOptions<ProcurementDbContext> Options(string databaseName) =>
        new DbContextOptionsBuilder<ProcurementDbContext>()
            .UseInMemoryDatabase(databaseName)
            .Options;

    [Fact]
    public async Task Add_then_get_roundtrips_the_aggregate_with_its_items()
    {
        var databaseName = Guid.NewGuid().ToString();

        var request = PurchaseRequest.Create(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid());
        request.AddItem("Laptop", Quantity.From(2));
        request.Submit();

        await using (var context = new ProcurementDbContext(Options(databaseName)))
        {
            var repository = new PurchaseRequestRepository(context);
            await repository.AddAsync(request);
        }

        await using (var context = new ProcurementDbContext(Options(databaseName)))
        {
            var repository = new PurchaseRequestRepository(context);

            var loaded = await repository.GetByIdAsync(request.Id);

            loaded.Should().NotBeNull();
            loaded!.Id.Should().Be(request.Id);
            loaded.Status.Should().Be(PurchaseRequestStatus.Submitted);
            loaded.Items.Should().ContainSingle();
            var item = loaded.Items.Single();
            item.Description.Should().Be("Laptop");
            item.Quantity.Value.Should().Be(2);
        }
    }

    [Fact]
    public async Task GetById_returns_null_when_the_aggregate_does_not_exist()
    {
        await using var context = new ProcurementDbContext(Options(Guid.NewGuid().ToString()));
        var repository = new PurchaseRequestRepository(context);

        var loaded = await repository.GetByIdAsync(PurchaseRequestId.New());

        loaded.Should().BeNull();
    }
}
