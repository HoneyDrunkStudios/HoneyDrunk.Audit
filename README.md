# HoneyDrunk.Audit

HoneyDrunk.Audit is the Grid's durable audit substrate.

It owns attributable audit records for:

- security and authorization events
- user/activity events
- system/operator/agent/integration events
- data-change events such as create/update/delete

Audit records are not Pulse telemetry, Loki logs, or OpenTelemetry traces. They are durable records written through `IAuditLog` and read through `IAuditQuery`.

## Packages

- `HoneyDrunk.Audit.Abstractions` — contracts and value types
- `HoneyDrunk.Audit.Data` — HoneyDrunk.Data-backed runtime implementation

## Phase-1 honest limitation

Phase 1 is append-only by interface: `IAuditLog` exposes only append and no update/delete methods. It is explicitly **not** tamper-evident. Hash-chain/WORM support is future scope.

## Redaction

Emitters must redact secrets, credentials, tokens, regulated data, and sensitive PII before appending `AuditChange` values. Audit is durable and queryable; it is not a secret store.
