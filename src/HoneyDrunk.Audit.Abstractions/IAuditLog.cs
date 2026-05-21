namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Append-only write surface for the Grid's durable, attributable audit record.
/// </summary>
public interface IAuditLog
{
    /// <summary>
    /// Appends a single audit entry to the durable record. The entry is immutable once appended.
    /// </summary>
    /// <param name="entry">The audit entry to append.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>A task representing the append operation.</returns>
    Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
