using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Canonical durable audit envelope for attributable Grid events.
/// </summary>
/// <param name="Id">The durable audit entry identifier; empty before the writer assigns it.</param>
/// <param name="OccurredAt">The event timestamp, normalized by the writer when absent.</param>
/// <param name="Actor">The attributable actor identifier.</param>
/// <param name="EventName">The application event name.</param>
/// <param name="Category">The audit event category.</param>
/// <param name="Outcome">The event outcome.</param>
/// <param name="Target">The resource addressed by the event.</param>
/// <param name="TenantId">The tenant that owns this operation.</param>
/// <param name="CorrelationId">The optional operation correlation identifier.</param>
/// <param name="Operation">The audited operation classification.</param>
/// <param name="Changes">The optional recorded field changes.</param>
/// <param name="Metadata">The optional event metadata; exclude secrets.</param>
/// <param name="Reason">The optional human-readable result reason.</param>
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
        Changes = Changes?.ToArray() ?? [],
        Metadata = Metadata is null
            ? []
            : new Dictionary<string, string>(Metadata, StringComparer.Ordinal),
    };
}
