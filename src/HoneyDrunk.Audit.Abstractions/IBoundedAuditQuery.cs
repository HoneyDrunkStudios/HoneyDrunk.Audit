namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Optional capability for source-bounded, newest-first forensic reads.
/// </summary>
/// <remarks>
/// Existing <see cref="IAuditQuery"/> implementations do not promise this capability.
/// Consumers requiring bounded reads must not fall back to an unbounded legacy read.
/// </remarks>
public interface IBoundedAuditQuery : IAuditQuery
{
    /// <summary>
    /// Reads at most the positive <see cref="AuditQueryFilter.Limit"/> entries matching the filter,
    /// ordered by occurrence time descending and then audit id descending.
    /// </summary>
    /// <param name="filter">The inclusive time range and optional exact-match filters, with a required positive limit.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>The newest matching entries.</returns>
    /// <remarks>
    /// Filtering, ordering and limiting occur at the source before materialization.
    /// Audit-id comparison follows the source's collation. At an equal-time limit boundary,
    /// callers combining separately limited results must not assume global ordinal-id selection.
    /// Unsupported storage providers must fail explicitly, without an unbounded fallback.
    /// A call follows the store's isolation semantics; separate calls are not a shared snapshot.
    /// </remarks>
    Task<IReadOnlyList<AuditEntry>> ReadLatestAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default);
}
