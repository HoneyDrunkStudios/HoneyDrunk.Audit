namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Identifies the resource, entity, or external target an audit event acted upon.
/// </summary>
/// <param name="Type">Resource or entity type.</param>
/// <param name="Id">Resource or entity identifier.</param>
/// <param name="DisplayName">Optional human-readable target label. Do not place secrets here.</param>
public sealed record AuditTarget(
    string Type,
    string Id,
    string? DisplayName = null)
{
    /// <summary>Gets an empty target for events that do not address a single resource.</summary>
    public static AuditTarget None { get; } = new(string.Empty, string.Empty);
}
