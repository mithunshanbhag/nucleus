[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet('app', 'tests', 'unit-tests', 'e2e-tests', 'package-size')]
    [string]$Target = 'tests'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$repoRoot = $PSScriptRoot
$solutionPath = Join-Path -Path $repoRoot -ChildPath 'Nucleus.slnx'
$packageProjectPath = Join-Path -Path $repoRoot -ChildPath 'src\Nucleus\Nucleus.csproj'
$unitTestProjectPath = Join-Path -Path $repoRoot -ChildPath 'tests\Nucleus.UnitTests\Nucleus.UnitTests.csproj'
$testsRoot = Join-Path -Path $repoRoot -ChildPath 'tests'

function Assert-PathExists {
    param(
        [Parameter(Mandatory)]
        [string]$Path,

        [Parameter(Mandatory)]
        [string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path)) {
        throw "$Description was not found: $Path"
    }
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory)]
        [string[]]$Arguments
    )

    Write-Host ('> dotnet {0}' -f ($Arguments -join ' ')) -ForegroundColor Cyan
    & dotnet @Arguments

    if ($LASTEXITCODE -ne 0) {
        throw "The dotnet command failed with exit code $LASTEXITCODE."
    }
}

function Restore-Target {
    param(
        [Parameter(Mandatory)]
        [string]$Path
    )

    Invoke-DotNet -Arguments @('restore', $Path, '--nologo')
}

function Get-AllTestProjects {
    if (-not (Test-Path -LiteralPath $testsRoot)) {
        return @()
    }

    return @(
        Get-ChildItem -Path $testsRoot -Recurse -Filter '*.csproj' -File |
        Select-Object -ExpandProperty FullName
    )
}

function Get-E2eTestProjects {
    $e2ePattern = '(?i)(^|[._-])(e2e|endtoend|end-to-end)([._-]|$)'

    return @(
        Get-AllTestProjects |
        Where-Object {
            [System.IO.Path]::GetFileNameWithoutExtension($_) -match $e2ePattern -or
            (Split-Path -Path $_ -Parent) -match $e2ePattern
        }
    )
}

function Invoke-TestProjects {
    param(
        [Parameter(Mandatory)]
        [string[]]$ProjectPaths,

        [Parameter(Mandatory)]
        [string]$Label
    )

    if ($ProjectPaths.Count -eq 0) {
        throw "No $Label test projects were found under $testsRoot."
    }

    foreach ($projectPath in $ProjectPaths) {
        Restore-Target -Path $projectPath
        Invoke-DotNet -Arguments @('test', $projectPath, '--configuration', 'Release', '--nologo', '--no-restore')
    }
}

function Get-LatestPackageFile {
    param(
        [Parameter(Mandatory)]
        [string]$Configuration
    )

    $packageDirectory = Join-Path -Path $repoRoot -ChildPath ("src\Nucleus\bin\{0}" -f $Configuration)

    $packageFile = Get-ChildItem -Path $packageDirectory -Filter '*.nupkg' -File |
        Sort-Object -Property LastWriteTimeUtc -Descending |
        Select-Object -First 1

    if ($null -eq $packageFile) {
        throw "No package file was found under $packageDirectory."
    }

    return $packageFile
}

function Get-SizeReductionPercent {
    param(
        [Parameter(Mandatory)]
        [long]$DebugSize,

        [Parameter(Mandatory)]
        [long]$ReleaseSize
    )

    if ($DebugSize -le 0) {
        return 0.0
    }

    return (($DebugSize - $ReleaseSize) / $DebugSize) * 100
}

function Write-PackageSizeComparison {
    Assert-PathExists -Path $packageProjectPath -Description 'Package project'

    Restore-Target -Path $packageProjectPath
    Invoke-DotNet -Arguments @('build', $packageProjectPath, '--configuration', 'Debug', '--nologo', '--no-restore')
    Invoke-DotNet -Arguments @('build', $packageProjectPath, '--configuration', 'Release', '--nologo', '--no-restore')

    $debugPackage = Get-LatestPackageFile -Configuration 'Debug'
    $releasePackage = Get-LatestPackageFile -Configuration 'Release'
    $debugDllPath = Join-Path -Path $repoRoot -ChildPath 'src\Nucleus\bin\Debug\net10.0\Nucleus.dll'
    $releaseDllPath = Join-Path -Path $repoRoot -ChildPath 'src\Nucleus\bin\Release\net10.0\Nucleus.dll'

    Assert-PathExists -Path $debugPackage.FullName -Description 'Debug package'
    Assert-PathExists -Path $releasePackage.FullName -Description 'Release package'
    Assert-PathExists -Path $debugDllPath -Description 'Debug assembly'
    Assert-PathExists -Path $releaseDllPath -Description 'Release assembly'

    $debugPackageSize = (Get-Item -LiteralPath $debugPackage.FullName).Length
    $releasePackageSize = (Get-Item -LiteralPath $releasePackage.FullName).Length
    $debugDllSize = (Get-Item -LiteralPath $debugDllPath).Length
    $releaseDllSize = (Get-Item -LiteralPath $releaseDllPath).Length

    $packageReductionPercent = Get-SizeReductionPercent -DebugSize $debugPackageSize -ReleaseSize $releasePackageSize
    $dllReductionPercent = Get-SizeReductionPercent -DebugSize $debugDllSize -ReleaseSize $releaseDllSize

    Write-Host 'Debug vs Release package size comparison:' -ForegroundColor Yellow
    Write-Host ("  Package (.nupkg): Debug = {0} bytes, Release = {1} bytes, Reduction = {2} bytes ({3:N2}%)" -f $debugPackageSize, $releasePackageSize, ($debugPackageSize - $releasePackageSize), $packageReductionPercent)
    Write-Host ("  Assembly (.dll):  Debug = {0} bytes, Release = {1} bytes, Reduction = {2} bytes ({3:N2}%)" -f $debugDllSize, $releaseDllSize, ($debugDllSize - $releaseDllSize), $dllReductionPercent)
}

Push-Location -LiteralPath $repoRoot

try {
    Assert-PathExists -Path $solutionPath -Description 'Solution file'

    switch ($Target) {
        'app' {
            Assert-PathExists -Path $packageProjectPath -Description 'Package project'
            Write-Host "This repository produces a NuGet package, so target 'app' builds the package project in Release (which also generates the package on build)." -ForegroundColor Yellow
            Restore-Target -Path $packageProjectPath
            Invoke-DotNet -Arguments @('build', $packageProjectPath, '--configuration', 'Release', '--nologo', '--no-restore')
        }

        'tests' {
            Invoke-TestProjects -ProjectPaths @(Get-AllTestProjects) -Label 'automated'
        }

        'unit-tests' {
            Assert-PathExists -Path $unitTestProjectPath -Description 'Unit test project'
            Invoke-TestProjects -ProjectPaths @($unitTestProjectPath) -Label 'unit'
        }

        'e2e-tests' {
            $e2eProjects = @(Get-E2eTestProjects)

            if ($e2eProjects.Count -eq 0) {
                throw "No E2E test projects were found under $testsRoot. This repository currently contains unit tests only, so target 'e2e-tests' exits with a non-zero status intentionally."
            }

            Invoke-TestProjects -ProjectPaths $e2eProjects -Label 'E2E'
        }

        'package-size' {
            Write-PackageSizeComparison
        }
    }

    Write-Host ("Target '{0}' completed successfully." -f $Target) -ForegroundColor Green
}
catch {
    Write-Error -Message $_.Exception.Message
    exit 1
}
finally {
    Pop-Location
}
