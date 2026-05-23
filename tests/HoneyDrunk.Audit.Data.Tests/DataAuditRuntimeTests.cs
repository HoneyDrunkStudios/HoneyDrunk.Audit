using Xunit;
using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Audit.Data.Tests.Fixtures;
using HoneyDrunk.Kernel.Abstractions.Identity;
using Microsoft.Extensions.Logging.Abstractions;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class DataAuditRuntimeTests
{
    [Fact]
    public async Task DataAuditLog_AppendsNormalizedRecordAndSavesChanges()
    {
        await using var unitOfWork = new FakeAuditUnitOfWork();
        var log = new DataAuditLog(unitOfWork, NullLogger<DataAuditLog>.Instance);

        await log.AppendAsync(
            NewEntry(DateTimeOffset.MinValue, "login.succeeded"),
            CancellationToken.None);

        var record = Assert.Single(unitOfWork.Records);
        Assert.False(string.IsNullOrWhiteSpace(record.Id));
        Assert.NotEqual(default, record.OccurredAt);
        Assert.Equal("login.succeeded", record.EventName);
        Assert.Equal(1, unitOfWork.SaveChangesCalls);
    }

    [Fact]
    public async Task DataAuditQuery_AppliesFiltersOrderingAndLimit()
    {
        await using var unitOfWork = new FakeAuditUnitOfWork();
        var log = new DataAuditLog(unitOfWork, NullLogger<DataAuditLog>.Instance);
        var query = new DataAuditQuery(unitOfWork);
        var now = DateTimeOffset.UtcNow;

        await log.AppendAsync(
            NewEntry(now.AddMinutes(2), "ignored", targetId: "other"),
            CancellationToken.None);
        await log.AppendAsync(
            NewEntry(now, "first", targetId: "customer-1"),
            CancellationToken.None);
        await log.AppendAsync(
            NewEntry(now.AddMinutes(1), "second", targetId: "customer-1"),
            CancellationToken.None);

        var results = await query.ReadAsync(
            new AuditQueryFilter(
                now.AddMinutes(-1),
                now.AddMinutes(3),
                Actor: "user:123",
                Category: AuditCategory.Security,
                EventName: null,
                TargetType: "Account",
                TargetId: "customer-1",
                CorrelationId: "corr-1",
                TenantId: TenantId.Internal,
                Limit: 1),
            CancellationToken.None);

        var entry = Assert.Single(results);
        Assert.Equal("first", entry.EventName);
    }

    [Fact]
    public async Task DataAuditQuery_RejectsInvalidTimeWindow()
    {
        await using var unitOfWork = new FakeAuditUnitOfWork();
        var query = new DataAuditQuery(unitOfWork);
        var now = DateTimeOffset.UtcNow;

        await Assert.ThrowsAsync<ArgumentException>(
            () => query.ReadAsync(
                new AuditQueryFilter(now, now.AddSeconds(-1)),
                CancellationToken.None));
    }

    private static AuditEntry NewEntry(
        DateTimeOffset occurredAt,
        string eventName,
        string targetId = "account-1") => new(
            AuditEntryId.Empty,
            occurredAt,
            "user:123",
            eventName,
            AuditCategory.Security,
            AuditOutcome.Succeeded,
            new AuditTarget("Account", targetId, "Primary account"),
            TenantId.Internal,
            "corr-1",
            AuditOperation.None,
            [],
            new Dictionary<string, string> { ["source"] = "test" },
            "test reason");
}
