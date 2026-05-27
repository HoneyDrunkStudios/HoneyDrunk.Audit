# Changelog

## [Unreleased]

## [0.2.0] - 2026-05-27

### Changed

- `AuditEntry.NormalizeForAppend` uses collection-expression `[]` for the empty `Changes` / `Metadata` fallbacks (Sonar S6602 — collection-initialization simplification). No behavior change.
- Bumped `HoneyDrunk.Kernel.Abstractions` `0.7.0 → 0.8.0`.
- Refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing/tooling alignment.
- Package version aligned with the HoneyDrunk.Audit 0.2.0 Sonar follow-up release (ADR-0011 D11).

## 0.1.0 - 2026-05-21

- Initial Audit abstractions package with `IAuditLog`, `IAuditQuery`, `AuditEntry`, and supporting audit taxonomy/value types.
