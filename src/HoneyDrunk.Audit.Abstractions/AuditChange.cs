namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Describes one field-level data-change audit diff.
/// </summary>
/// <param name="Field">Changed field name.</param>
/// <param name="Before">Optional redacted previous value.</param>
/// <param name="After">Optional redacted new value.</param>
/// <param name="Redacted">Whether before/after values were redacted before append.</param>
public sealed record AuditChange(
    string Field,
    string? Before = null,
    string? After = null,
    bool Redacted = false);
