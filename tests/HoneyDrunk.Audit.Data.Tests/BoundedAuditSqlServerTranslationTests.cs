using System.Data;
using System.Data.Common;
using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Transactions;
using HoneyDrunk.Data.EntityFramework.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NSubstitute;
using Xunit;

namespace HoneyDrunk.Audit.Data.Tests;

// This verifies the real SQL Server provider's generated command, not a live SQL Server execution.
// Both connection opening and command execution are suppressed. SQLite tests execute real SQL.
public sealed class BoundedAuditSqlServerTranslationTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task BoundedRead_SqlServer150TranslatesTopFilterAndBothOrderingKeys(bool newestFirst)
    {
        using var emptyResults = new DataTable();
        var capture = new CommandCapture(emptyResults);
        await using var context = new SqlServerAuditContext(new DbContextOptionsBuilder<SqlServerAuditContext>()
            .UseSqlServer("Server=not-used;Database=not-used;Integrated Security=true", sql => sql.UseCompatibilityLevel(150))
            .AddInterceptors(new NoConnection(), capture).Options);
        var unitOfWork = Substitute.For<IUnitOfWork<IAuditDataContext>>();
        unitOfWork.Repository<AuditRecord>().Returns(new EfRepository<AuditRecord, SqlServerAuditContext>(context));
        var query = new DataAuditQuery(unitOfWork);
        var filter = new AuditQueryFilter(
            new DateTimeOffset(2026, 11, 10, 0, 0, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 11, 11, 0, 0, 0, TimeSpan.Zero),
            TargetType: "agent", TargetId: "a", Limit: 50);

        var rows = newestFirst ? await query.ReadLatestAsync(filter) : await query.ReadAsync(filter);

        Assert.Empty(rows);
        var sql = Assert.Single(capture.Commands);
        Assert.Contains("SELECT TOP(", sql, StringComparison.Ordinal);
        Assert.Contains("[a].[OccurredAt] >=", sql, StringComparison.Ordinal);
        Assert.Contains("[a].[OccurredAt] <=", sql, StringComparison.Ordinal);
        Assert.Contains("[a].[TargetType] =", sql, StringComparison.Ordinal);
        Assert.Contains("[a].[TargetId] =", sql, StringComparison.Ordinal);
        Assert.Contains(newestFirst ? "ORDER BY [a].[OccurredAt] DESC, [a].[Id] DESC" : "ORDER BY [a].[OccurredAt], [a].[Id]", sql, StringComparison.Ordinal);
        Assert.Contains(50, capture.Parameters);
        Assert.DoesNotContain("COUNT", sql, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class SqlServerAuditContext(DbContextOptions<SqlServerAuditContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.Entity<AuditRecord>().HasKey(row => row.Id);
    }

    private sealed class NoConnection : DbConnectionInterceptor
    {
        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection, ConnectionEventData eventData, InterceptionResult result,
            CancellationToken cancellationToken = default) => ValueTask.FromResult(InterceptionResult.Suppress());
    }

    private sealed class CommandCapture(DataTable results) : DbCommandInterceptor
    {
        public List<string> Commands { get; } = [];

        public List<object> Parameters { get; } = [];

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            Commands.Add(command.CommandText);
            Parameters.AddRange(command.Parameters.Cast<DbParameter>().Select(parameter => parameter.Value!));
            return ValueTask.FromResult(InterceptionResult<DbDataReader>.SuppressWithResult(results.CreateDataReader()));
        }
    }
}
