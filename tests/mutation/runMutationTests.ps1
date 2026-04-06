# runMutationTests
param(
  [string]$ConfigFile = "./stryker-config.json",
  [string]$TestProjectPath = "../unit"
)

# Resolve paths
$CurrentDir = Get-Location
$ConfigPath = Join-Path $CurrentDir $ConfigFile
$ResolvedTestPath = Resolve-Path -Path $TestProjectPath

# Report output path
$OutputDir = Join-Path $CurrentDir "results"
$UniqueId = "report-" + $(Get-Date -Format "yyyy-MM-dd_HH-mm-ss")
$UniqueOutputDir = Join-Path $OutputDir $UniqueId

# Validate config file
if (-not (Test-Path -Path $ConfigPath)) {
  Write-Error "Config file not found: $ConfigPath"
  exit 1
}

# Validate test project path
if (-not $ResolvedTestPath) {
  Write-Error "Test project path not found: $TestProjectPath"
  exit 1
}

# Restore local tools if using a tool manifest
try {
  dotnet tool restore | Out-Null
} catch {
  Write-Host "dotnet tool restore failed or no local tool manifest found. Continuing..." -ForegroundColor Yellow
}

# Run Stryker with the specified config and test project path
Write-Host "Running Stryker with config: $ConfigPath" -ForegroundColor Cyan
Write-Host "Test project: $ResolvedTestPath" -ForegroundColor Cyan

# Run mutation testing on unit tests
Set-Location "$ResolvedTestPath"; dotnet stryker --config-file "$ConfigPath" --output "$UniqueOutputDir" --open-report; Set-Location "$CurrentDir"
