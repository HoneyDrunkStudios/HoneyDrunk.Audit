# HoneyDrunk.Audit agent instructions

Own durable attributable audit records through IAuditLog/IAuditQuery. Audit history is distinct from Pulse telemetry. Keep shared audit schema and storage policy here; consumers must not duplicate audit storage or confuse delivery logs with canonical audit records.

Start with [README.md](README.md) and the relevant source/tests. Project files and lockfiles own SDK, framework and dependency versions.

Read the [shared engineering conventions](https://github.com/HoneyDrunkStudios/HoneyDrunk.Standards/blob/main/HoneyDrunk.Standards/docs/CONVENTIONS.md) and this repository's owning documentation before editing. Apply the parts relevant to this stack; preserve existing public contracts, dependency direction and repository-specific behavior. Verify shared capabilities in current code before reusing them; a catalog entry or scaffold is not an implemented integration.

Work within the selected request. Preserve unrelated changes and use a separate worktree when needed. Review the final diff, use Conventional Commits and ready-for-review PRs with exactly one accurate `Authorship:` line and a `Request:` line; include the authorship in commit trailers. Run meaningful checks for the affected behavior and report the reviewed/tested revision, failures and unrun checks. For documentation-only changes, check links, paths and instruction consistency. Preserve required checks and inspect actual latest-head Sonar new-code findings where analysis applies; do not suppress findings or weaken gates to obtain a pass. Legacy Grid Review is retired; do not restore its workers, queues or bypass labels. A configured replacement reviewer is not evidence of a completed review or enforcing merge check.

## Verification

From the repository root for code/build changes:

```sh
dotnet restore HoneyDrunk.Audit.slnx
dotnet build HoneyDrunk.Audit.slnx -c Release --no-restore
dotnet test HoneyDrunk.Audit.slnx -c Release --no-build
```

Use the checked-in workflow and relevant test documentation for additional integration prerequisites, coverage and consumer checks. Do not use live resources or credentials merely to make a local check pass.

## Code Review Rules

Apply the [shared review criteria](https://github.com/HoneyDrunkStudios/HoneyDrunk.Standards/blob/main/HoneyDrunk.Standards/docs/CONVENTIONS.md#code-review) to changed behavior, using the repository boundaries above. Report actionable findings with the failing path, concrete impact and a small corrective action; disclose unavailable evidence. These rules grant no cross-repository access or merge authority.

- Preserve attributable canonical audit history through the existing IAuditLog/IAuditQuery contracts. Keep shared schema/storage policy here; Pulse telemetry and delivery logs are not substitute audit records.
- Trace actor/tenant/context attribution, append-only behavior, retry and partial persistence failure. Flag history rewriting, missing records masked as success, cross-tenant reads or sensitive payload disclosure; bound audit queries and retention work.
- Require contract, attribution, persistence and replay/failure tests for changed behavior. Check schema compatibility and actual shared ownership without imposing product outcomes on the audit layer or claiming durability from an in-memory test alone.
