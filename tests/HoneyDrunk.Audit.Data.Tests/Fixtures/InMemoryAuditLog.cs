using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Data.Tests.Fixtures;

internal sealed class InMemoryAuditLog(InMemoryAuditStore store) : IAuditLog
{
    public Task AppendAsync(AuditEntry entry, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        store.Append(entry);
        return Task.CompletedTask;
    }
}
