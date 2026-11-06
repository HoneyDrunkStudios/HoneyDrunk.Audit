using HoneyDrunk.Kernel.Abstractions.Identity;

namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Forensic query filter. Null fields are ignored.
/// </summary>
/// <param name="Since">The beginning of the query time range.</param>
/// <param name="Until">The end of the query time range.</param>
/// <param name="Actor">The optional actor identifier filter.</param>
/// <param name="Category">The audit event category.</param>
/// <param name="EventName">The application event name.</param>
/// <param name="TargetType">The optional target resource type filter.</param>
/// <param name="TargetId">The optional target identifier filter.</param>
/// <param name="CorrelationId">The optional operation correlation identifier.</param>
/// <param name="TenantId">The optional tenant identifier filter.</param>
/// <param name="Limit">The optional maximum number of returned entries.</param>
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
