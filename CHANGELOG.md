# Changelog

## [Unreleased]

## [0.2.0] - 2026-05-27

### Changed (breaking)

- **`AuditDataContext` (class) replaced by `IAuditDataContext` (interface).** The type exists only as a generic type argument for `IUnitOfWork<T>` to discriminate Audit's persistence boundary; the original `sealed` + `private` ctor form tripped Sonar S3453 ("can't be instantiated"), the interim `abstract class` form tripped S2094 ("remove this empty class, write its code or make it an interface"). Interface is the canonical empty-marker shape and `IUnitOfWork<T>` has no `class` constraint, so it composes cleanly. Consumers that referenced `AuditDataContext` directly must update the type + usings.
- **`DataAuditLog` is now `sealed partial class`** so the new source-generated `[LoggerMessage]` partial method can attach. Source-compatible for callers but a binary metadata change.
- **Package versions bumped** to `HoneyDrunk.Audit* 0.2.0` per pre-1.0 semver.

### Changed

- `DataAuditLog.AppendAsync` "Appended audit entry" log now uses a source-generated `[LoggerMessage]` partial method (`LogAppendedEntry`) — Sonar CA1848 / S6664 ("evaluation of this argument may be expensive and unnecessary if logging is disabled"). Argument evaluation is now gated on `IsEnabled(LogLevel.Information)` inside the generated code.
- `DataAuditQuery` private `And` / `GreaterThanOrEqual` / `LessThanOrEqual` helpers return `BinaryExpression` instead of `Expression` (Sonar return-type-narrowing finding). The methods actually returned `BinaryExpression` already; the declared type now matches.
- `DataAuditQuery.ReadAsync` returns `[.. ordered]` instead of `ordered.ToArray()` (Sonar collection-expression simplification).
- `AuditEntry.NormalizeForAppend` uses collection-expression `[]` for the empty fallbacks (Sonar S6602).
- `AuditRecord.FromEntry` / `ToEntry` likewise use collection-expression `[]` for empty fallbacks.

### Security

- **Workflow permissions tightened.** Sonar S6671 was flagging write permissions at the workflow level on `.github/workflows/nightly-security.yml`, `publish.yml`, and `weekly-deps.yml`. Workflow-level `permissions:` now declare only `contents: read`; the relevant writes (`security-events: write`, `issues: write`, `contents: write`, `pull-requests: write`, `packages: write`, `id-token: write`) are scoped to the specific job that needs them. The preflight detection job runs with read-only.

### Internal

- Bumped `HoneyDrunk.Kernel.Abstractions` `0.7.0 → 0.8.0` (consumed by Abstractions).
- Bumped `Microsoft.Extensions.DependencyInjection` / `.DependencyInjection.Abstractions` / `.Logging.Abstractions` / `.Options` `10.0.5 → 10.0.8`.
- Sonar test-code findings cleared: explicit-typed `IAuditLog`/`IAuditQuery` locals in tests narrowed to `var` (variable-type-narrowing rule); `_records.Where(...).ToArray()` / `query.ToArray()` switched to collection-expression form `[.. ...]`.
- Onboarded Audit to SonarQube Cloud (ADR-0011 D11). Wired a `sonarcloud` job in `pr.yml` (renamed from `pr-core.yml` to match the Grid convention) that calls `HoneyDrunkStudios/HoneyDrunk.Actions/.github/workflows/job-sonarcloud.yml` on both `pull_request` (after `pr-core` succeeds) and `push` to `main` (standalone), both gated by the existing `preflight` solution-detection. PR analysis gates the merge on new-code findings; main-branch analysis populates the SonarCloud Overview dashboard and the leak-period baseline. Per-project source/test classification is discovered automatically from MSBuild `IsTestProject` properties; per-repo Sonar overrides can be added later via `Directory.Build.props` `<SonarQubeSetting>` items or as new inputs to `job-sonarcloud.yml`. Branch-protection requirement added separately after the first successful run lands.
- Enabled ADR-0044 Grid Review request workflow and repo-local OpenClaw/Codex review configuration.
- Adopted HoneyDrunk.Standards.Tests 0.2.9 for Audit test projects, refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing alignment, and normalized test cancellation tokens so the repo can use the shared test-stack package.

## 0.1.0 - 2026-05-21

- Initial HoneyDrunk.Audit scaffold with Abstractions and Data packages.
- Added append-only `IAuditLog`, filtered `IAuditQuery`, `AuditEntry`, and audit taxonomy/value types for activity/security/system and data-change audit.
- Added Data-backed runtime services, DI registration, retention policy slot, internal in-memory test fixture, and smoke tests.
