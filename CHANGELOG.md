# Changelog

## [Unreleased]

### Changed

- Onboarded Audit to SonarQube Cloud (ADR-0011 D11). Added `sonar-project.properties` at the repo root and wired a `sonarcloud` job in `pr.yml` (renamed from `pr-core.yml` to match the Grid convention) that calls `HoneyDrunk.Actions/.github/workflows/job-sonarcloud.yml` after `pr-core`, gated by the existing `preflight` solution-detection. Branch-protection requirement added separately after the first successful run lands.
- Enabled ADR-0044 Grid Review request workflow and repo-local OpenClaw/Codex review configuration.

- Adopted HoneyDrunk.Standards.Tests 0.2.9 for Audit test projects, refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing alignment, and normalized test cancellation tokens so the repo can use the shared test-stack package.

## 0.1.0 - 2026-05-21

- Initial HoneyDrunk.Audit scaffold with Abstractions and Data packages.
- Added append-only `IAuditLog`, filtered `IAuditQuery`, `AuditEntry`, and audit taxonomy/value types for activity/security/system and data-change audit.
- Added Data-backed runtime services, DI registration, retention policy slot, internal in-memory test fixture, and smoke tests.
