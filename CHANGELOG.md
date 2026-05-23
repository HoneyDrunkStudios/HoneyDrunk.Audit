# Changelog

## [Unreleased]

### Changed

- Adopted HoneyDrunk.Standards.Tests 0.2.9 for Audit test projects, refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing alignment, and normalized test cancellation tokens so the repo can use the shared test-stack package.

## 0.1.0 - 2026-05-21

- Initial HoneyDrunk.Audit scaffold with Abstractions and Data packages.
- Added append-only `IAuditLog`, filtered `IAuditQuery`, `AuditEntry`, and audit taxonomy/value types for activity/security/system and data-change audit.
- Added Data-backed runtime services, DI registration, retention policy slot, internal in-memory test fixture, and smoke tests.
