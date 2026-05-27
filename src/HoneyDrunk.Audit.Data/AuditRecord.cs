using HoneyDrunk.Audit.Abstractions;

namespace HoneyDrunk.Audit.Data;

/// <summary>
/// Persistence model for one appended audit entry.
/// </summary>
public sealed class AuditRecord
{
    /// <summary>Gets or sets the durable audit id.</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets when the event occurred.</summary>
    public DateTimeOffset OccurredAt { get; set; }

    /// <summary>Gets or sets the actor.</summary>
    public string Actor { get; set; } = string.Empty;

    /// <summary>Gets or sets the event name.</summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>Gets or sets the category.</summary>
    public AuditCategory Category { get; set; }

    /// <summary>Gets or sets the outcome.</summary>
    public AuditOutcome Outcome { get; set; }

    /// <summary>Gets or sets the target type.</summary>
    public string TargetType { get; set; } = string.Empty;

    /// <summary>Gets or sets the target id.</summary>
    public string TargetId { get; set; } = string.Empty;

    /// <summary>Gets or sets the target display name.</summary>
    public string? TargetDisplayName { get; set; }

    /// <summary>Gets or sets the tenant id.</summary>
    public string TenantId { get; set; } = string.Empty;

    /// <summary>Gets or sets the correlation id.</summary>
    public string? CorrelationId { get; set; }

    /// <summary>Gets or sets the data-change operation.</summary>
    public AuditOperation Operation { get; set; }

    /// <summary>Gets or sets the optional reason.</summary>
    public string? Reason { get; set; }

    /// <summary>Gets or sets serialized changes.</summary>
    public string ChangesJson { get; set; } = "[]";

    /// <summary>Gets or sets serialized metadata.</summary>
    public string MetadataJson { get; set; } = "{}";

    /// <summary>Creates a persistence record from an audit entry.</summary>
    /// <param name="entry">The audit entry.</param>
    /// <returns>A persistence record.</returns>
    public static AuditRecord FromEntry(AuditEntry entry) => new()
    {
        Id = entry.Id.ToString(),
        OccurredAt = entry.OccurredAt,
        Actor = entry.Actor,
        EventName = entry.EventName,
        Category = entry.Category,
        Outcome = entry.Outcome,
        TargetType = entry.Target.Type,
        TargetId = entry.Target.Id,
        TargetDisplayName = entry.Target.DisplayName,
        TenantId = entry.TenantId.ToString(),
        CorrelationId = entry.CorrelationId,
        Operation = entry.Operation,
        Reason = entry.Reason,
        ChangesJson = System.Text.Json.JsonSerializer.Serialize(entry.Changes ?? (IReadOnlyList<AuditChange>)[]),
        MetadataJson = System.Text.Json.JsonSerializer.Serialize(entry.Metadata ?? (IReadOnlyDictionary<string, string>)new Dictionary<string, string>()),
    };

    /// <summary>Converts this persistence record back to a contract entry.</summary>
    /// <returns>The contract entry.</returns>
    public AuditEntry ToEntry() => new(
        new AuditEntryId(Id),
        OccurredAt,
        Actor,
        EventName,
        Category,
        Outcome,
        new AuditTarget(TargetType, TargetId, TargetDisplayName),
        HoneyDrunk.Kernel.Abstractions.Identity.TenantId.TryParse(TenantId, out var tenantId) ? tenantId : HoneyDrunk.Kernel.Abstractions.Identity.TenantId.Internal,
        CorrelationId,
        Operation,
        System.Text.Json.JsonSerializer.Deserialize<IReadOnlyList<AuditChange>>(ChangesJson) ?? [],
        System.Text.Json.JsonSerializer.Deserialize<IReadOnlyDictionary<string, string>>(MetadataJson) ?? new Dictionary<string, string>(),
        Reason);
}
