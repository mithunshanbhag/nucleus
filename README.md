# Nucleus

![Build status](https://img.shields.io/badge/build-not%20configured-lightgrey)
![Package status](https://img.shields.io/badge/package-alpha-blueviolet)
![Test suite](https://img.shields.io/badge/tests-xUnit-informational)

> Screenshot / GIF placeholder: Nucleus is a reusable NuGet package rather than a standalone app, so there is no product UI capture yet. If a sample host or demo UI is added later, store related assets under [`docs/assets`](docs/assets).

## 📦 Package purpose

Nucleus is a reusable .NET 10 package that groups common building blocks for application development under the `MithunShanbhag.Nucleus` NuGet package identity.

The current package includes:

- ASP.NET Core and Blazor support types such as `NControllerBase` and `NComponentBase`
- repository abstractions and base implementations for Cosmos DB, Blob Storage, and CSV-backed workflows
- Azure Service Bus event stream abstractions
- shared exception, retry, console, and extension-method helpers

The main library lives in [`src/Nucleus`](src/Nucleus) and the automated tests live in [`tests/Nucleus.UnitTests`](tests/Nucleus.UnitTests).

## ✨ Highlights

- **Package ID:** `MithunShanbhag.Nucleus`
- **Target framework:** `net10.0`
- **Output type:** reusable NuGet package generated from `src\Nucleus\Nucleus.csproj`
- **Key dependencies:** FluentValidation, Azure SDKs, Polly, CsvHelper, Spectre.Console

## 🚀 Installation

### From NuGet

```powershell
dotnet add package MithunShanbhag.Nucleus --prerelease
```

### From a local build

```powershell
dotnet pack .\src\Nucleus\Nucleus.csproj -c Release
```

After packing, reference the generated `.nupkg` from your local package source as needed.

## 🧭 Usage

Nucleus is intended to be consumed as a library. Common integration points include:

- injecting repository interfaces such as `ICosmosGenericRepository<TEntity>` or `IBlobGenericRepository`
- deriving from shared support types such as `NComponentBase`
- publishing events through `IServiceBusEventStream<TEvent>`
- wrapping retryable operations with `NRetryHelper`
- standardizing exception-to-response handling with custom types derived from `NExceptionBase`

Example repository usage:

```csharp
using Nucleus.Repositories.Interfaces;

public sealed class CustomerService(ICosmosGenericRepository<CustomerDocument> repository)
{
    public Task<CustomerDocument?> GetAsync(string partitionKey, string id, CancellationToken cancellationToken)
        => repository.GetAsync(partitionKey, id, cancellationToken);
}
```

Example retry helper usage:

```csharp
using Nucleus.Misc.Helpers;

var retryHelper = new NRetryHelper(maxRetryAttempts: 3, delayInMilliSecs: 100);

retryHelper.Execute(() =>
{
    // Call a transient dependency here.
});
```

## 🛠️ Build and run locally

Nucleus is a library repository, and the root-level [`run-local.ps1`](run-local.ps1) script now provides the main local workflow:

```powershell
.\run-local.ps1            # runs all test projects (default target: tests)
.\run-local.ps1 unit-tests # runs the unit test project
.\run-local.ps1 app        # builds the package project in Release
.\run-local.ps1 package-size # compares Debug vs Release .nupkg and DLL sizes
```

If you need the underlying `dotnet` commands directly, use:

```powershell
dotnet restore .\Nucleus.slnx
dotnet build .\Nucleus.slnx
dotnet test .\Nucleus.slnx
dotnet pack .\src\Nucleus\Nucleus.csproj -c Release
```

### Debug vs Release package sizing

If you want to confirm the impact before publishing, run:

```powershell
.\run-local.ps1 package-size
```

On the current preview package, local comparisons show that Release is consistently smaller than Debug:

- `.nupkg`: roughly `1.4 KB` smaller, or about `6%`
- `Nucleus.dll`: roughly `3.5 KB` smaller, or about `7.5%`

Use the script output as the source of truth for the exact current numbers before publishing.

## 🧪 Running the tests

Run the full repository test project:

```powershell
dotnet test .\tests\Nucleus.UnitTests\Nucleus.UnitTests.csproj
```

Or run through the solution file:

```powershell
dotnet test .\Nucleus.slnx
```

## 🗂️ Repository structure

| Path | Purpose |
| --- | --- |
| [`src/Nucleus`](src/Nucleus) | Main package source code |
| [`tests/Nucleus.UnitTests`](tests/Nucleus.UnitTests) | xUnit-based automated tests |
| [`docs/specs/README.md`](docs/specs/README.md) | Package-oriented specification notes |
| [`docs/specs/ui.md`](docs/specs/ui.md) | UI guidance for this otherwise non-UI package repo |
| [`docs/ui-mockups`](docs/ui-mockups) | Reserved for future mockups if a sample/demo UI is introduced |
| [`docs/assets`](docs/assets) | Reserved for screenshots, GIFs, and other documentation assets |
| [`docs/user-manual`](docs/user-manual) | Reserved for deeper consumer guidance if needed |

## 📝 Specs and UI notes

This repository follows the shared documentation conventions in `.github`, but it should be treated as a **reusable package repository**, not as a full application.

- [`docs/specs/README.md`](docs/specs/README.md) captures package scope, structure, and maintenance expectations.
- [`docs/specs/ui.md`](docs/specs/ui.md) explains why UI documentation is intentionally light right now.
- [`docs/ui-mockups`](docs/ui-mockups) remains mostly placeholder content until the repository includes a demo application, sample host, or another user-facing surface worth mocking.

## 🤝 Contributing and project policies

- [Contributing guide](CONTRIBUTING.md)
- [Code of conduct](CODE_OF_CONDUCT.md)
- [Security policy](SECURITY.md)
