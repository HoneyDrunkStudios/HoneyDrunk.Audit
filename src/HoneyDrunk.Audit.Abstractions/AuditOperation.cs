namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Optional persistence operation classification for data-change audit events.
/// </summary>
public enum AuditOperation
{
    /// <summary>No persistence operation was classified.</summary>
    None = 0,

    /// <summary>A resource was created.</summary>
    Create = 1,

    /// <summary>A resource was updated.</summary>
    Update = 2,

    /// <summary>A resource was deleted.</summary>
    Delete = 3,
}
