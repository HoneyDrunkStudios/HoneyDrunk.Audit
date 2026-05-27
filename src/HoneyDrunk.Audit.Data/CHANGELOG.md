# Changelog

## [Unreleased]

## [0.2.0] - 2026-05-27

### Changed (breaking)

- **`AuditDataContext` (class) replaced by `IAuditDataContext` (interface).** Marker type for `IUnitOfWork<T>` composition — Sonar wanted the original `sealed`+`private` ctor form replaced (S3453); the interim `abstract class` form then tripped S2094 ("empty class → use interface"). Interface is the right shape for a phantom marker, and `IUnitOfWork<T>` has no `class` constraint so it composes cleanly. Consumers that referenced `AuditDataContext` directly must update the type + usings.
- **`DataAuditLog` is now `sealed partial class`** so the new source-generated `[LoggerMessage]` partial method can attach. Source-compatible for callers; binary metadata change.

### Changed

- `DataAuditLog.AppendAsync` "Appended audit entry" log now uses a source-generated `[LoggerMessage]` partial method (CA1848 / S6664). Argument evaluation gated on `IsEnabled(LogLevel.Information)` inside generated code.
- `DataAuditQuery` private `And` / `GreaterThanOrEqual` / `LessThanOrEqual` helpers now return `BinaryExpression` instead of `Expression` (Sonar return-type-narrowing).
- `DataAuditQuery.ReadAsync` uses `[.. ordered]` instead of `ordered.ToArray()` (Sonar S6602).
- `AuditRecord.FromEntry` / `ToEntry` use collection-expression `[]` for empty fallbacks (Sonar S6602).
- Bumped `Microsoft.Extensions.DependencyInjection.Abstractions` / `Logging.Abstractions` / `Options` `10.0.5 → 10.0.8`.
- Refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing/tooling alignment.
- Package version aligned with the HoneyDrunk.Audit 0.2.0 Sonar follow-up release (ADR-0011 D11).

## 0.1.0 - 2026-05-21

- Initial Data-backed Audit runtime with append/query services, retention policy slot, and DI registration.
