# HoneyDrunk.Audit.Data

Data-backed runtime implementation for HoneyDrunk.Audit.

Hosts compose HoneyDrunk.Data for `IUnitOfWork<IAuditDataContext>` and register Audit services with `AddHoneyDrunkAuditData()`.

Phase 1 is append-only by interface and is explicitly **not** tamper-evident. Hash-chain/WORM integrity remains future scope.

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
