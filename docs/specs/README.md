# Nucleus package specifications

## Overview

Nucleus is maintained as a reusable NuGet package repository. The primary deliverable is the package built from [`src/Nucleus/Nucleus.csproj`](../../src/Nucleus/Nucleus.csproj), with validation performed by [`tests/Nucleus.UnitTests/Nucleus.UnitTests.csproj`](../../tests/Nucleus.UnitTests/Nucleus.UnitTests.csproj).

## Scope

The package currently focuses on shared .NET building blocks such as:

- base types for controllers and Blazor components
- repository abstractions and implementations for Cosmos DB, Blob Storage, and CSV workflows
- Azure Service Bus event-stream contracts
- shared exception, retry, console, and extension-method helpers

## Repository structure

| Path                       | Role                                                   |
| -------------------------- | ------------------------------------------------------ |
| `src/Nucleus/`             | Package source and package metadata                    |
| `tests/Nucleus.UnitTests/` | Automated test coverage for the package                |
| `docs/specs/`              | Package-level specification notes                      |
| `docs/ui-mockups/`         | Reserved for future mockups if a demo UI is introduced |

## Non-goals for this repo

- This repository does **not** currently ship a standalone UI application.
- UI mockups are optional and should only be added when a sample app, documentation host, or other user-facing surface exists.
- The documentation should stay package-focused unless the repository grows beyond a pure library.

## Local verification

Use the following commands for local validation:

```powershell
dotnet restore .\Nucleus.slnx
dotnet build .\Nucleus.slnx
dotnet test .\Nucleus.slnx
dotnet pack .\src\Nucleus\Nucleus.csproj -c Release
```

## Maintenance notes

Update this file when any of the following change:

- package purpose or supported feature areas
- source or test project paths
- package identity, target framework, or local validation workflow
- expectations around UI mockups or consumer-facing documentation
