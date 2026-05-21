# HoneyDrunk.Audit.Abstractions

Contracts for the HoneyDrunk Audit substrate.

- `IAuditLog` is append-only and exposes no update/delete methods.
- `IAuditQuery` provides filtered, time-ordered forensic reads.
- `AuditEntry` is the durable envelope for security, activity, system, integration, agent, and data-change events.
- `AuditChange` values must be redacted before append when fields contain secrets, credentials, tokens, regulated data, or sensitive PII.
