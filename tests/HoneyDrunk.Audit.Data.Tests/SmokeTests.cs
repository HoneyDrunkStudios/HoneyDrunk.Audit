using Xunit;
using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Audit.Data.Tests.Fixtures;
using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class SmokeTests
{
    [Fact]
    public async Task WriteThroughAuditLog_ReadsBackThroughAuditQuery()
    {
        var store = new InMemoryAuditStore();
        IAuditLog log = new InMemoryAuditLog(store);
        IAuditQuery query = new InMemoryAuditQuery(store);
        var now = DateTimeOffset.UtcNow;

        await log.AppendAsync(new AuditEntry(
            AuditEntryId.Empty,
            now,
            "user:123",
            "customer.updated",
            AuditCategory.DataChange,
            AuditOutcome.Succeeded,
            new AuditTarget("Customer", "customer-1"),
            TenantId.Internal,
            "corr-1",
            AuditOperation.Update,
            [new AuditChange("Email", Redacted: true)],
            new Dictionary<string, string> { ["source"] = "smoke" }), TestContext.Current.CancellationToken);

        var results = await query.ReadAsync(new AuditQueryFilter(
            now.AddMinutes(-1),
            now.AddMinutes(1),
            Category: AuditCategory.DataChange,
            TargetType: "Customer",
            TargetId: "customer-1"), TestContext.Current.CancellationToken);

        var entry = Assert.Single(results);
        Assert.False(entry.Id.IsEmpty);
        Assert.Equal("customer.updated", entry.EventName);
        Assert.Equal(AuditOperation.Update, entry.Operation);
        Assert.True(entry.Changes![0].Redacted);
    }
}
