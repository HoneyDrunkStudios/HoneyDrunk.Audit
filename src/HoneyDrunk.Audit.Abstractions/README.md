# HoneyDrunk.Audit.Abstractions

Contracts for the HoneyDrunk Audit substrate.

- `IAuditLog` is append-only and exposes no update/delete methods.
- `IAuditQuery` provides filtered, time-ordered forensic reads.
- `AuditEntry` is the durable envelope for security, activity, system, integration, agent, and data-change events.
- `AuditChange` values must be redacted before append when fields contain secrets, credentials, tokens, regulated data, or sensitive PII.

- `IBoundedAuditQuery` is an optional newest-first capability with a required positive limit. Filtering,
  ordering and limiting must occur at the source; unsupported implementations fail rather than fall back.
- Audit-id tie ordering follows provider collation. Combining separately limited results does not imply
  global ordinal-id cutoff membership or a shared snapshot across calls.
- Existing `IAuditQuery` and `AuditQueryFilter` signatures remain unchanged.
