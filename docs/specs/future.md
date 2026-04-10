# Future Considerations

This document captures future-facing ideas, improvements, and technical recommendations that may be worth revisiting as the package evolves.

## General Housekeeping

- Migrate away from `AutoMapper` to `Mapperly` (recommended) or `Mapster`.
- Migrate away from `MediatR` to some other alternative (needs more research and evaluation).

## Cosmos Repository

### Context

The current Cosmos repository base is a clean, intentionally thin wrapper over the Azure Cosmos DB .NET SDK. It already does a few important things well:

- keeps operations asynchronous
- passes cancellation tokens through to SDK calls
- uses point reads for `GetAsync`
- makes cross-partition queries explicit
- supports transactional batch operations within a single logical partition

That said, the biggest remaining opportunities are not basic CRUD correctness. They are mostly around reliability, concurrency safety, observability, and taking fuller advantage of SDK-level performance and availability features.

As of April 9, 2026, the package references `Microsoft.Azure.Cosmos` `3.57.1`, while NuGet shows `3.58.0` as the latest stable version.

### Prioritized Recommendations

#### 1. Standardize `CosmosClient` resiliency configuration

Highest priority.

The repository base cannot compensate for weak `CosmosClient` construction. The most important reliability recommendation is to standardize client creation in consuming applications and document the expected setup clearly.

Recommended focus areas:

- singleton client lifetime
- direct mode connectivity
- preferred regions or application region configuration
- sane timeout and retry-related settings
- newer SDK availability features such as cross-region hedging and partition-level circuit breaker

This is likely the single biggest reliability improvement available without changing repository behavior.

#### 2. Add optimistic concurrency support

The current write paths are blind writes and deletes. That is simple, but it can allow lost updates in multi-writer systems.

Recommended direction:

- support ETag-aware update and delete patterns
- expose overloads that allow `IfMatchEtag`
- treat `412 Precondition Failed` as a first-class concurrency outcome

This would make the repository more robust in realistic distributed workloads.

#### 3. Replace `Exists` implementations that rely on `COUNT`

The current `Exists` methods effectively perform count queries and then compare the result to zero. That is easy to reason about, but it is usually more RU-expensive than necessary.

Recommended direction:

- prefer `SELECT TOP 1 VALUE 1 ...` for existence checks
- prefer point reads when `id` and partition key are already known

This is a practical performance and cost optimization.

#### 4. Surface diagnostics and RU usage

The repository currently returns entities or booleans, but it discards SDK response metadata such as diagnostics and request charge.

Recommended direction:

- expose request charge where useful
- capture diagnostics for slow requests and failures
- make it easier for consuming services to log query latency, RU cost, and retry activity

Without this, operational debugging becomes harder than it needs to be.

#### 5. De-emphasize raw string SQL and raw filter fragments

The current API supports raw query strings and string-based filter fragments. This is flexible, but it also encourages non-parameterized SQL and makes query review harder.

Recommended direction:

- favor `QueryDefinition` overloads in calling code
- prefer parameterized queries over string interpolation or concatenation
- treat string filter helpers as convenience methods, not the preferred long-term pattern

This is mainly a design and maintainability recommendation.

#### 6. Take fuller advantage of built-in SDK write and throughput optimizations

The latest SDK already includes optimizations that the repository does not currently help consumers adopt.

Recommended focus areas:

- `EnableContentResponseOnWrite = false` for write paths that do not need response bodies
- `AllowBulkExecution = true` for high-throughput ingestion scenarios
- optimistic direct execution where eligible for single-partition query workloads

These are built-in optimizations and should be preferred over custom retry or batching logic where the SDK already provides support.

#### 7. Add Patch-based update paths

The repository currently exposes full-document upsert behavior, but not partial document updates.

Recommended direction:

- support Cosmos Patch operations for targeted field changes
- use patching where only a subset of document properties change

This can reduce RU consumption, reduce payload size, and narrow the conflict surface compared to whole-document replacement.

#### 8. Improve batch failure ergonomics

Returning `TransactionalBatchResponse` is flexible, but it still places responsibility on every caller to inspect success and failure details correctly.

Recommended direction:

- define a clearer calling convention for batch success and failure handling
- document expected caller behavior around `IsSuccessStatusCode`
- consider whether higher-level helpers are warranted for common batch patterns

This is more of an API ergonomics improvement than a correctness issue.

#### 9. Expand tests toward resilience and concurrency behavior

The current test suite covers core happy-path and boundary behavior well, but it is still focused on CRUD mechanics.

Recommended additions:

- concurrency conflict behavior such as `412 Precondition Failed`
- diagnostics and observability expectations
- throttling and retry-facing behavior
- transactional batch failure semantics

This would improve confidence in the repository under real production conditions.

### Built-In SDK Reliability Mechanisms Worth Leveraging

The Azure Cosmos DB .NET SDK already provides reliability and availability mechanisms that should be preferred over ad hoc custom wrappers:

- automatic retries for throttling and other transient failures
- cross-region hedging for read availability and latency improvement
- partition-level circuit breaker for unhealthy physical partitions
- per-request and client-level diagnostics
- bulk execution support
- patch support for partial updates

These SDK-native capabilities should shape future repository guidance and client configuration.
