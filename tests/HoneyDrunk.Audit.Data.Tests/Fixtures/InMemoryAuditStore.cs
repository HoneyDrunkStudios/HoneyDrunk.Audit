using Xunit;
using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class InMemoryAuditStore
{
    private readonly List<AuditEntry> _entries = [];

    public IReadOnlyList<AuditEntry> Entries => _entries;

    public void Append(AuditEntry entry) => _entries.Add(entry.NormalizeForAppend(DateTimeOffset.UtcNow));

    public IReadOnlyList<AuditEntry> Query(AuditQueryFilter filter)
    {
        IEnumerable<AuditEntry> query = _entries
            .Where(entry => entry.OccurredAt >= filter.Since && entry.OccurredAt <= filter.Until)
            .Where(entry => filter.Actor is null || entry.Actor == filter.Actor)
            .Where(entry => filter.Category is null || entry.Category == filter.Category.Value)
            .Where(entry => filter.EventName is null || entry.EventName == filter.EventName)
            .Where(entry => filter.TargetType is null || entry.Target.Type == filter.TargetType)
            .Where(entry => filter.TargetId is null || entry.Target.Id == filter.TargetId)
            .Where(entry => filter.CorrelationId is null || entry.CorrelationId == filter.CorrelationId)
            .Where(entry => filter.TenantId is null || entry.TenantId == filter.TenantId.Value)
            .OrderBy(entry => entry.OccurredAt)
            .ThenBy(entry => entry.Id.ToString());

        if (filter.Limit is > 0)
        {
            query = query.Take(filter.Limit.Value);
        }

        return query.ToArray();
    }
}
