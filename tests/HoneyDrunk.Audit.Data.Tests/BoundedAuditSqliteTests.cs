using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Audit.Data.Tests.Fixtures;
using HoneyDrunk.Data.Abstractions.Transactions;
using HoneyDrunk.Data.EntityFramework.Repositories;
using HoneyDrunk.Kernel.Abstractions.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit;

namespace HoneyDrunk.Audit.Data.Tests;

public sealed class BoundedAuditSqliteTests : IAsyncDisposable
{
    private static readonly DateTimeOffset Start = new(2026, 11, 11, 0, 0, 0, TimeSpan.Zero);
    private readonly SqliteConnection _connection = new("Data Source=:memory:");
    private readonly AuditQueryObserver _observer = new();
    private readonly AuditSqliteContext _context;
    private readonly DataAuditQuery _query;

    public BoundedAuditSqliteTests()
    {
        _connection.Open();
        _context = new AuditSqliteContext(new DbContextOptionsBuilder<AuditSqliteContext>()
            .UseSqlite(_connection).AddInterceptors(_observer).Options);
        _context.Database.EnsureCreated();
        var repository = new EfRepository<AuditRecord, AuditSqliteContext>(_context);
        var unitOfWork = Substitute.For<IUnitOfWork<IAuditDataContext>>();
        unitOfWork.Repository<AuditRecord>().Returns(repository);
        _query = new DataAuditQuery(unitOfWork);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BoundedRead_FiltersOrdersAndLimitsBeforeMaterialization(bool newestFirst)
    {
        await SeedAsync();
        var filter = Filter(10);

        var rows = newestFirst ? await _query.ReadLatestAsync(filter) : await _query.ReadAsync(filter);

        var indexes = newestFirst ? Enumerable.Range(490, 10).Reverse() : Enumerable.Range(0, 10);
        Assert.Equal(indexes.Select(index => index.ToString("x32")), rows.Select(row => row.Id.Value));
        Assert.Equal(10, _observer.Materialized);
        var sql = Assert.Single(_observer.Commands);
        Assert.Contains("WHERE", sql, StringComparison.Ordinal);
        Assert.Contains(newestFirst ? "ORDER BY \"a\".\"OccurredAt\" DESC, \"a\".\"Id\" DESC" : "ORDER BY \"a\".\"OccurredAt\", \"a\".\"Id\"", sql, StringComparison.Ordinal);
        Assert.Contains("LIMIT", sql, StringComparison.Ordinal);
        Assert.Contains(10, _observer.Parameters);
    }

    [Fact]
    public async Task ReadLatestAsync_InclusiveZeroWidthRangeKeepsDeterministicTies()
    {
        await SeedAsync();
        var rows = await _query.ReadLatestAsync(Filter(3) with { Since = Start, Until = Start });
        Assert.Equal(Enumerable.Range(2, 3).Reverse().Select(index => index.ToString("x32")), rows.Select(row => row.Id.Value));
        Assert.Equal(3, _observer.Materialized);
        Assert.Single(_observer.Commands);
    }

    [Fact]
    public async Task ReadLatestAsync_InsertAtReadBoundaryCannotBreakLimit()
    {
        await SeedAsync();
        _observer.BeforeRead = async () =>
        {
            await using var writer = new AuditSqliteContext(new DbContextOptionsBuilder<AuditSqliteContext>()
                .UseSqlite(_connection).Options);
            writer.Add(Record(9999, Start.AddMinutes(99)));
            await writer.SaveChangesAsync();
        };

        var rows = await _query.ReadLatestAsync(Filter(10));

        Assert.Equal(9999.ToString("x32"), rows[0].Id.Value);
        Assert.Equal(10, rows.Count);
        Assert.Equal(10, _observer.Materialized);
        Assert.Single(_observer.Commands);
        Assert.DoesNotContain("COUNT", _observer.Commands[0], StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task ReadAsync_ExistingUnboundedSemanticsRemainExplicit(int? limit)
    {
        await SeedAsync();
        var rows = await _query.ReadAsync(Filter(limit));
        Assert.Equal(500, rows.Count);
        Assert.Equal(Enumerable.Range(0, 500).Select(index => index.ToString("x32")), rows.Select(row => row.Id.Value));
    }

    [Fact]
    public async Task ReadLatestAsync_CancellationPreventsAnyRead()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => _query.ReadLatestAsync(Filter(10), cancellation.Token));
        Assert.Empty(_observer.Commands);
        Assert.Equal(0, _observer.Materialized);
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    private static AuditQueryFilter Filter(int? limit) => new(
        Start, Start.AddMinutes(99), Actor: "actor", Category: AuditCategory.Security,
        EventName: "event", TargetType: "agent", TargetId: "a", CorrelationId: "correlation",
        TenantId: TenantId.Internal, Limit: limit);

    private static AuditRecord Record(int id, DateTimeOffset occurredAt) => new()
    {
        Id = id.ToString("x32"), OccurredAt = occurredAt, Actor = "actor", EventName = "event",
        Category = AuditCategory.Security, Outcome = AuditOutcome.Succeeded, TargetType = "agent",
        TargetId = "a", CorrelationId = "correlation", TenantId = TenantId.Internal.ToString(),
    };

    private async Task SeedAsync()
    {
        for (var index = 499; index >= 0; index--)
        {
            _context.Add(Record(index, Start.AddMinutes(index / 5)));
        }

        // Every filter must reach the database, otherwise these newer/tied records displace matches.
        Action<AuditRecord>[] mismatches =
        [
            row => row.Actor = "other", row => row.Category = AuditCategory.SystemAction,
            row => row.EventName = "other", row => row.TargetType = "user", row => row.TargetId = "other",
            row => row.CorrelationId = "other", row => row.TenantId = "other",
        ];
        for (var index = 0; index < mismatches.Length; index++)
        {
            var record = Record(1000 + index, Start.AddMinutes(99));
            mismatches[index](record);
            _context.Add(record);
        }

        _context.Add(Record(2000, Start.AddTicks(-1)));
        _context.Add(Record(2001, Start.AddMinutes(99).AddTicks(1)));
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        _observer.Reset();
    }
}
