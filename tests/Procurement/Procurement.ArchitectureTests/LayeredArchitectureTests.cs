using System.Reflection;
using FluentAssertions;
using Procurement.Application.PurchaseRequests.Create;
using Procurement.Domain.PurchaseRequests.Aggregates;
using Xunit;

namespace Procurement.ArchitectureTests;

public sealed class LayeredArchitectureTests
{
    [Fact]
    public void Domain_must_not_depend_on_infrastructure_upper_layers_or_the_other_service()
    {
        var references = ReferencedAssemblyNames(typeof(PurchaseRequest).Assembly);

        references.Should().NotContain(name =>
            name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
            || name.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.Ordinal)
            || name.StartsWith("Npgsql", StringComparison.Ordinal)
            || name.StartsWith("Yarp", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Application", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Infrastructure", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Persistence", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Presentation", StringComparison.Ordinal)
            || name.StartsWith("Notification", StringComparison.Ordinal));
    }

    [Fact]
    public void Application_must_not_depend_on_presentation_infrastructure_or_aspnetcore()
    {
        var references = ReferencedAssemblyNames(typeof(CreatePurchaseRequestHandler).Assembly);

        references.Should().NotContain(name =>
            name.StartsWith("Microsoft.AspNetCore", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Presentation", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Infrastructure", StringComparison.Ordinal)
            || name.StartsWith("Procurement.Persistence", StringComparison.Ordinal)
            || name.StartsWith("Notification", StringComparison.Ordinal));
    }

    private static string[] ReferencedAssemblyNames(Assembly assembly) =>
        assembly.GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();
}