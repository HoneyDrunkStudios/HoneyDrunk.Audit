# HoneyDrunk.Audit.Data

Data-backed runtime implementation for HoneyDrunk.Audit.

Hosts compose HoneyDrunk.Data for `IUnitOfWork<AuditDataContext>` and register Audit services with `AddHoneyDrunkAuditData()`.

Phase 1 is append-only by interface and is explicitly **not** tamper-evident. Hash-chain/WORM integrity remains future scope.
