using Xunit;
using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Abstractions.Tests;

public sealed class ContractSurfaceTests
{
    [Fact]
    public void PublicSurface_IncludesExpectedContractTypes()
    {
        Type[] expected =
        [
            typeof(IAuditLog),
            typeof(IAuditQuery),
            typeof(AuditEntry),
            typeof(AuditEntryId),
            typeof(AuditQueryFilter),
            typeof(AuditCategory),
            typeof(AuditOutcome),
            typeof(AuditOperation),
            typeof(AuditTarget),
            typeof(AuditChange),
        ];

        var publicTypes = typeof(IAuditLog).Assembly.GetExportedTypes();

        foreach (var type in expected)
        {
            Assert.Contains(type, publicTypes);
        }
    }

    [Fact]
    public void AuditEntry_SupportsDataChangeAuditShape()
    {
        var entry = new AuditEntry(
            AuditEntryId.Empty,
            DateTimeOffset.UtcNow,
            "user:123",
            "customer.updated",
            AuditCategory.DataChange,
            AuditOutcome.Succeeded,
            new AuditTarget("Customer", "customer-1"),
            HoneyDrunk.Kernel.Abstractions.Identity.TenantId.Internal,
            Operation: AuditOperation.Update,
            Changes:
            [
                new AuditChange("Email", Redacted: true),
            ]);

        Assert.Equal(AuditCategory.DataChange, entry.Category);
        Assert.Equal(AuditOperation.Update, entry.Operation);
        Assert.Single(entry.Changes!);
        Assert.True(entry.Changes![0].Redacted);
    }
}
