# Changelog

## [0.3.0] - 2026-11-11

### Added

- Optional `IBoundedAuditQuery.ReadLatestAsync` capability for newest-first audit reads with a required positive limit. Existing `IAuditQuery` and `AuditQueryFilter` constructor/deconstruction signatures remain unchanged.
- Real SQLite tests for filtering, inclusive windows, ties, source limits and insert-boundary behavior; SQL Server compatibility-150 query-translation tests; explicit legacy-provider rejection and API compatibility checks.

### Changed

- Positive-limit Data-backed reads apply ordering and limit at the persistence source before materializing the flat audit records. Bounded reads require Data 0.8.0's `IBoundedReadOnlyRepository<AuditRecord>` capability and fail explicitly when unsupported.
- Earliest-first defaults and historical unbounded null/zero/negative-limit reads remain available through `IAuditQuery.ReadAsync`; bounded consumers must opt into the new capability.
- Runtime packages require .NET 11; Abstractions retain .NET 10 and add .NET 11.

## [0.2.2] - 2026-11-05

### Changed

- Refresh NuGet dependencies and adopt upstream package releases in dependency order. Preserve target frameworks and public contracts.

| Dependency | Previous | Updated |
| --- | --- | --- |
| HoneyDrunk.Data.Abstractions | 0.7.1 | 0.7.2 |
| HoneyDrunk.Kernel.Abstractions | 0.8.1 | 0.8.2 |
| HoneyDrunk.Standards | 0.3.0 | 0.4.3 |
| HoneyDrunk.Standards.Tests | 0.3.0 | 0.4.3 |
| Microsoft.Extensions.DependencyInjection | 10.0.12 | 10.0.14 |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.12 | 10.0.14 |
| Microsoft.Extensions.Logging.Abstractions | 10.0.12 | 10.0.14 |
| Microsoft.Extensions.Options | 10.0.12 | 10.0.14 |

## [0.2.1] - 2026-09-26

### Changed

- Refresh dependency and shared build-tooling versions; preserve target frameworks and existing public contracts. See the [repository dependency changes](../../CHANGELOG.md).

## [Unreleased]

## [0.2.0] - 2026-05-27

### Changed

- `AuditEntry.NormalizeForAppend` uses collection-expression `[]` for the empty `Changes` / `Metadata` fallbacks (Sonar S6602 — collection-initialization simplification). No behavior change.
- Bumped `HoneyDrunk.Kernel.Abstractions` `0.7.0 → 0.8.0`.
- Refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing/tooling alignment.
- Package version aligned with the HoneyDrunk.Audit 0.2.0 Sonar follow-up release (ADR-0011 D11).

## 0.1.0 - 2026-05-21

- Initial Audit abstractions package with `IAuditLog`, `IAuditQuery`, `AuditEntry`, and supporting audit taxonomy/value types.
