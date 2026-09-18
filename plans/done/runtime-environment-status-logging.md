# Periodic runtime environment status logging

**Status:** Implemented (2026-09-18)

Added a periodic background service that logs a single structured event (memory, GC-reported
cgroup-aware available memory, CPU usage %, background task queue length, GC generation
collection counts, thread pool availability, and process uptime) to aid operational visibility
into the running OpenShift pod, reusing the existing Serilog pipeline (no new external
dependencies, no Prometheus/OpenTelemetry).

Follows the existing `Models/StructuredLog` convention (`LogUserIdentification`/`LogApiInfo`)
rather than inventing a separate logging path: added a `LogRuntimeStatus` model and a dedicated
`MESSAGE_TEMPLATE_RUNTIME_STATUS = "{@LogRuntimeStatus}"` in `LogContent`. Unlike request-scoped
logs, the runtime status event isn't paired with `LogUserIdentification`/`LogApiInfo` since it
isn't tied to a request/user — it logs `LogRuntimeStatus` on its own.

Interval is configurable via `RuntimeStatusLogging:IntervalSeconds` in `appsettings.json`
(default 300s; `<= 0` disables it). The first event logs immediately on startup, so pod status is
visible right away rather than after the first interval elapses; subsequent events follow the
configured interval.

`CpuUsagePercent` is computed against `CpuQuotaCores`, a fractional CPU quota (e.g. 0.5 for an
OpenShift "500m" limit) read directly from cgroup v2/v1 files, not `Environment.ProcessorCount`
— which rounds sub-1-core quotas up to 1 and would otherwise understate CPU usage. `ProcessorCount`
is still logged for reference, alongside `CpuQuotaCores`.
visible right away rather than after the first interval elapses; subsequent events follow the
configured interval.

Files touched:
- `aspnetcore/src/api/Background/BackgroundTaskQueue.cs` — added `Count` to
  `IBackgroundTaskQueue`/`BackgroundTaskQueue`.
- `aspnetcore/src/api/Background/RuntimeStatusLoggerService.cs` — new `BackgroundService`,
  `PeriodicTimer`-based, with a pure/testable `CalculateCpuUsagePercent` helper.
- `aspnetcore/src/api/Models/StructuredLog/LogRuntimeStatus.cs` — new structured log model.
- `aspnetcore/src/api/Models/StructuredLog/LogContent.cs` — new `MESSAGE_TEMPLATE_RUNTIME_STATUS`
  message template constant.
- `aspnetcore/src/api/Startup.cs` — registered `RuntimeStatusLoggerService` as a hosted service.
- `aspnetcore/src/api/appsettings.json` — new `RuntimeStatusLogging:IntervalSeconds` setting.
- `aspnetcore/src/api.Tests/Background_Tests/BackgroundTaskQueueTest.cs` — new, covers `Count`.
- `aspnetcore/src/api.Tests/Background_Tests/RuntimeStatusLoggerServiceTest.cs` — new, covers
  `CalculateCpuUsagePercent` edge cases.

Full solution `dotnet build`/`dotnet test` pass (328 tests).
