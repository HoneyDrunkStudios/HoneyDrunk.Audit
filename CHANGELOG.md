# Changelog

## [Unreleased]

### Changed

- Onboarded Audit to SonarQube Cloud (ADR-0011 D11). Wired a `sonarcloud` job in `pr.yml` (renamed from `pr-core.yml` to match the Grid convention) that calls `HoneyDrunkStudios/HoneyDrunk.Actions/.github/workflows/job-sonarcloud.yml` on both `pull_request` (after `pr-core` succeeds) and `push` to `main` (standalone), both gated by the existing `preflight` solution-detection. PR analysis gates the merge on new-code findings; main-branch analysis populates the SonarCloud Overview dashboard and the leak-period baseline. Per-project source/test classification is discovered automatically from MSBuild `IsTestProject` properties; per-repo Sonar overrides can be added later via `Directory.Build.props` `<SonarQubeSetting>` items or as new inputs to `job-sonarcloud.yml`. Branch-protection requirement added separately after the first successful run lands.
- Enabled ADR-0044 Grid Review request workflow and repo-local OpenClaw/Codex review configuration.

- Adopted HoneyDrunk.Standards.Tests 0.2.9 for Audit test projects, refreshed HoneyDrunk.Standards to 0.2.9 for ADR-0047 testing alignment, and normalized test cancellation tokens so the repo can use the shared test-stack package.

## 0.1.0 - 2026-05-21

- Initial HoneyDrunk.Audit scaffold with Abstractions and Data packages.
- Added append-only `IAuditLog`, filtered `IAuditQuery`, `AuditEntry`, and audit taxonomy/value types for activity/security/system and data-change audit.
- Added Data-backed runtime services, DI registration, retention policy slot, internal in-memory test fixture, and smoke tests.
