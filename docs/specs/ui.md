# UI notes

## Current state

Nucleus is a reusable package repository and does not currently include a standalone application UI. Because of that, there are no required screen flows, end-user pages, or product mockups to maintain at this time.

## What still matters for UI-related docs

- The package does include shared support for UI consumers, such as `NComponentBase` for Blazor-based scenarios.
- If a sample host, demo app, admin tool, or other user-facing surface is added later, document its intended flows here before creating mockups.
- Keep any future UI notes aligned with the actual repository structure under `src\Nucleus\` and related test coverage under `tests\Nucleus.UnitTests\`.

## `docs\ui-mockups` guidance

The [`docs/ui-mockups`](../ui-mockups) folder is intentionally minimal right now.

Only add mockups when there is a concrete UI surface to describe, for example:

- a demo app that showcases package features
- a documentation site with package configuration flows
- a sample Blazor or ASP.NET Core host maintained in this repository

Until then, the placeholder content in `docs\ui-mockups` is expected.
