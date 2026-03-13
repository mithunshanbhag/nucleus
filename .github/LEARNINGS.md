# Learnings

## 2026-03-11

- Document repository-specific paths explicitly: the active package source is in `src\Nucleus\` and the active tests are in `tests\Nucleus.UnitTests\`.
- For this repository, package installation, usage, and build/test instructions are more useful than app-style walkthroughs because Nucleus is a reusable NuGet package.
- `run-local.ps1` is the supported local entry point: it defaults to running all tests, supports `unit-tests` and `app`, and the `e2e-tests` target intentionally exits non-zero until dedicated E2E projects exist.
- `run-local.ps1 package-size` compares the Debug and Release `.nupkg` and `Nucleus.dll` outputs so package publishing decisions can be based on measured size differences.
