using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Canonical durable audit envelope for attributable Grid events.
/// </summary>
public sealed record AuditEntry(
    AuditEntryId Id,
    DateTimeOffset OccurredAt,
    string Actor,
    string EventName,
    AuditCategory Category,
    AuditOutcome Outcome,
    AuditTarget Target,
    TenantId TenantId,
    string? CorrelationId = null,
    AuditOperation Operation = AuditOperation.None,
    IReadOnlyList<AuditChange>? Changes = null,
    IReadOnlyDictionary<string, string>? Metadata = null,
    string? Reason = null)
{
    /// <summary>
    /// Returns an entry with a writer-assigned id and append timestamp when absent.
    /// </summary>
    /// <param name="clock">The timestamp to use when <see cref="OccurredAt"/> is default.</param>
    /// <returns>A normalized entry suitable for durable append.</returns>
    public AuditEntry NormalizeForAppend(DateTimeOffset clock) => this with
    {
        Id = Id.IsEmpty ? AuditEntryId.New() : Id,
        OccurredAt = OccurredAt == default ? clock : OccurredAt,
        Changes = Changes?.ToArray() ?? Array.Empty<AuditChange>(),
        Metadata = Metadata is null
            ? new Dictionary<string, string>()
            : new Dictionary<string, string>(Metadata, StringComparer.Ordinal),
    };
}
