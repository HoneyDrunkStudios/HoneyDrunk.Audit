namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Outcome of an auditable event.
/// </summary>
public enum AuditOutcome
{
    /// <summary>The action completed successfully.</summary>
    Succeeded = 0,

    /// <summary>The action was denied by policy or authorization.</summary>
    Denied = 1,

    /// <summary>The action failed after it was attempted.</summary>
    Failed = 2,

    /// <summary>The action was accepted or started but not yet complete.</summary>
    Pending = 3,
}
