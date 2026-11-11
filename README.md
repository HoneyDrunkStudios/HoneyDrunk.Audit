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

## .NET SDK

Build with the .NET 11 SDK pinned in `global.json` (11.0.100, stable patch roll-forward). Runtime and test projects target `net11.0`; Abstractions retain `net10.0` alongside `net11.0` and public contract shapes are unchanged.

## Bounded audit reads

`DataAuditQuery` implements the optional `IBoundedAuditQuery` capability. Resolve the existing
`IAuditQuery` registration and check for this capability when a caller requires bounded reads:

```csharp
if (query is not IBoundedAuditQuery bounded)
{
    throw new NotSupportedException("Source-bounded audit reads are required.");
}

var entries = await bounded.ReadLatestAsync(
    new AuditQueryFilter(since, until, TargetType: "agent", TargetId: agentId, Limit: 50),
    cancellationToken);
```

The Data-backed implementation requires a repository implementing
`IBoundedReadOnlyRepository<AuditRecord>` from HoneyDrunk.Data 0.8.0. Its EF implementation
applies the inclusive time range, optional field predicates, occurrence-time/id ordering and
positive limit in one logical source query before materializing matching root records.
Unsupported repositories or untranslatable queries fail explicitly; there is no unbounded fallback
or count-then-fetch step. Limits bound matching flat audit records, not database scans, sorts,
related graphs or SQL command count. Host-owned mappings and indexes remain unchanged.

`ReadLatestAsync` requires a positive limit and orders occurrence time descending, then audit id
descending. Audit-id comparisons follow the provider's collation. At an equal-time cutoff,
merging separately limited scopes does not promise the same membership as an unbounded global
ordinal-id sort. Each call follows the store's isolation semantics; separate calls are not a shared snapshot.

`IAuditQuery.ReadAsync` retains earliest-first order. A positive limit now uses the source-bounded
repository capability and fails when that capability is absent. Null, zero and negative limits
retain the historical intentional all-results behavior. Existing `IAuditQuery` and positional
`AuditQueryFilter` constructor/deconstruction signatures remain unchanged; legacy query providers
must not be treated as source-bounded without the new capability.
