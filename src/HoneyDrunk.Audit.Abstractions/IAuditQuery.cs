namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Time-ordered and filtered read surface for incident reconstruction and forensic retrieval.
/// </summary>
public interface IAuditQuery
{
    /// <summary>
    /// Reads entries matching the filter in time order, earliest first.
    /// </summary>
    /// <param name="filter">The query filter.</param>
    /// <param name="cancellationToken">A token to observe for cancellation.</param>
    /// <returns>Matching audit entries.</returns>
    Task<IReadOnlyList<AuditEntry>> ReadAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default);
}
