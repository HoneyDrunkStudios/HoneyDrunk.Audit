using HoneyDrunk.Audit.Abstractions;
using HoneyDrunk.Data.Abstractions.Transactions;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// HoneyDrunk.Data-backed forensic audit reader.
/// </summary>
public sealed class DataAuditQuery(IUnitOfWork<AuditDataContext> unitOfWork) : IAuditQuery
{
    private readonly IUnitOfWork<AuditDataContext> _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));

    /// <inheritdoc />
    public async Task<IReadOnlyList<AuditEntry>> ReadAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        if (filter.Until < filter.Since)
        {
            throw new ArgumentException("Until must be greater than or equal to Since.", nameof(filter));
        }

        var records = await _unitOfWork.Repository<AuditRecord>().FindAsync(
            record => record.OccurredAt >= filter.Since &&
                record.OccurredAt <= filter.Until &&
                (filter.Actor == null || record.Actor == filter.Actor) &&
                (filter.Category == null || record.Category == filter.Category.Value) &&
                (filter.EventName == null || record.EventName == filter.EventName) &&
                (filter.TargetType == null || record.TargetType == filter.TargetType) &&
                (filter.TargetId == null || record.TargetId == filter.TargetId) &&
                (filter.CorrelationId == null || record.CorrelationId == filter.CorrelationId) &&
                (filter.TenantId == null || record.TenantId == filter.TenantId.Value.ToString()),
            cancellationToken).ConfigureAwait(false);

        var ordered = records
            .OrderBy(record => record.OccurredAt)
            .ThenBy(record => record.Id)
            .Select(record => record.ToEntry());

        if (filter.Limit is > 0)
        {
            ordered = ordered.Take(filter.Limit.Value);
        }

        return ordered.ToArray();
    }
}
