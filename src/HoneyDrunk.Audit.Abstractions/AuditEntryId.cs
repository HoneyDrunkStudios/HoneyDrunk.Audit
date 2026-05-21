namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Strongly typed identifier for an appended audit entry.
/// </summary>
public readonly record struct AuditEntryId(string Value)
{
    /// <summary>Gets the empty identifier used before append assigns a durable id.</summary>
    public static AuditEntryId Empty { get; } = new(string.Empty);

    /// <summary>Gets a value indicating whether the identifier is empty.</summary>
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);

    /// <summary>Creates a new audit entry identifier.</summary>
    /// <returns>A new identifier.</returns>
    public static AuditEntryId New() => new(Guid.NewGuid().ToString("N"));

    /// <inheritdoc />
    public override string ToString() => Value ?? string.Empty;
}
