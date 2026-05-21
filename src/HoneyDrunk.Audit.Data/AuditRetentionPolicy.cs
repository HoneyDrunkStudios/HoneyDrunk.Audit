namespace HoneyDrunk.Audit.Data;

/// <summary>
/// Audit-class retention settings. Values are host-composed from configuration.
/// </summary>
public sealed class AuditRetentionPolicy
{
    /// <summary>Default retention window for durable audit records.</summary>
    public static readonly TimeSpan DefaultRetention = TimeSpan.FromDays(365 * 7);

    /// <summary>Gets or sets the retention window.</summary>
    public TimeSpan Retention { get; set; } = DefaultRetention;
}
