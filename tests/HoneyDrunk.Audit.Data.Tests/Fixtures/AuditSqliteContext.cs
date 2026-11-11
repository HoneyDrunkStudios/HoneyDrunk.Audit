using Microsoft.EntityFrameworkCore;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class AuditSqliteContext(DbContextOptions<AuditSqliteContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditRecord>().HasKey(record => record.Id);
        modelBuilder.Entity<AuditRecord>().Property(record => record.Id).UseCollation("BINARY");
        // SQLite cannot order native DateTimeOffset values; this test host stores UTC ticks.
        modelBuilder.Entity<AuditRecord>().Property(record => record.OccurredAt)
            .HasConversion(value => value.UtcTicks, value => new DateTimeOffset(value, TimeSpan.Zero));
    }
}
