using Xunit;
using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Audit.Data.Tests.Fixtures;
using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class DataAuditQueryTests
{
    [Fact]
    public async Task ReadAsync_ReturnsEntriesInTimeOrderAndAppliesLimit()
    {
        var store = new InMemoryAuditStore();
        var log = new InMemoryAuditLog(store);
        var query = new InMemoryAuditQuery(store);
        var now = DateTimeOffset.UtcNow;

        await log.AppendAsync(NewEntry(now.AddMinutes(2), "third"), CancellationToken.None);
        await log.AppendAsync(NewEntry(now, "first"), CancellationToken.None);
        await log.AppendAsync(NewEntry(now.AddMinutes(1), "second"), CancellationToken.None);

        var results = await query.ReadAsync(new AuditQueryFilter(
            now.AddMinutes(-1),
            now.AddMinutes(3),
            Limit: 2), CancellationToken.None);

        Assert.Equal(["first", "second"], [.. results.Select(entry => entry.EventName)]);
    }

    private static AuditEntry NewEntry(DateTimeOffset occurredAt, string eventName) => new(
        AuditEntryId.Empty,
        occurredAt,
        "system",
        eventName,
        AuditCategory.SystemAction,
        AuditOutcome.Succeeded,
        new AuditTarget("Job", eventName),
        TenantId.Internal);
}
