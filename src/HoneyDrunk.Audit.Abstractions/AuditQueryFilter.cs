using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Forensic query filter. Null fields are ignored.
/// </summary>
public sealed record AuditQueryFilter(
    DateTimeOffset Since,
    DateTimeOffset Until,
    string? Actor = null,
    AuditCategory? Category = null,
    string? EventName = null,
    string? TargetType = null,
    string? TargetId = null,
    string? CorrelationId = null,
    TenantId? TenantId = null,
    int? Limit = null);
