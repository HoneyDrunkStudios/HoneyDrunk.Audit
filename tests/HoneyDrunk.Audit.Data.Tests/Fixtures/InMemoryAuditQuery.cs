using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class InMemoryAuditQuery(InMemoryAuditStore store) : IAuditQuery
{
    public Task<IReadOnlyList<AuditEntry>> ReadAsync(
        AuditQueryFilter filter,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(store.Query(filter));
    }
}
