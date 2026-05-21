namespace HoneyDrunk.Audit.Abstractions;

/// <summary>
/// Broad audit event family used for routing, retention review, and forensic filters.
/// </summary>
public enum AuditCategory
{
    /// <summary>Authentication, authorization, MFA, role, and policy events.</summary>
    Security = 0,

    /// <summary>User-facing activity such as workflows, purchases, or domain actions.</summary>
    UserActivity = 1,

    /// <summary>Persistence create, update, and delete events.</summary>
    DataChange = 2,

    /// <summary>System and operator activity such as jobs, deploys, or breaker changes.</summary>
    SystemAction = 3,

    /// <summary>Agent activity such as delegated work, tool execution, or handoffs.</summary>
    AgentAction = 4,

    /// <summary>External integration ingress or egress activity.</summary>
    Integration = 5,
}
