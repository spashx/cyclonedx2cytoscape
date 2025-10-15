# Run CdxViz Integration Tests
# This script demonstrates how to run tests with different configurations

param(
    [string]$ExecutablePath = "",
    [string]$Formats = "cytoscape,3dforce",
    [switch]$Coverage = $false,
    [switch]$Verbose = $false
)

Write-Host "=== CdxViz Integration Tests ===" -ForegroundColor Cyan

# Set executable path if provided
if ($ExecutablePath) {
    $env:CDXVIZ_EXECUTABLE_PATH = $ExecutablePath
    Write-Host "Using executable: $ExecutablePath" -ForegroundColor Yellow
}

# Set formats to test
$env:CDXVIZ_TEST_FORMATS = $Formats
Write-Host "Testing formats: $Formats" -ForegroundColor Yellow

# Build test arguments
$testArgs = @()

if ($Coverage) {
    Write-Host "Code coverage enabled" -ForegroundColor Yellow
    $testArgs += "--collect:XPlat Code Coverage"
    $testArgs += "--settings"
    $testArgs += "coverlet.runsettings"
}

if ($Verbose) {
    $testArgs += "--logger"
    $testArgs += "console;verbosity=detailed"
}

# Navigate to test project directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Push-Location $scriptDir

try {
    Write-Host "`nRunning tests..." -ForegroundColor Green
    Write-Host "Command: dotnet test $($testArgs -join ' ')" -ForegroundColor Gray
    Write-Host ""
    
    if ($testArgs.Count -gt 0) {
        dotnet test @testArgs
    } else {
        dotnet test
    }
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "`n? Tests completed successfully!" -ForegroundColor Green
        Write-Host "`nTest outputs are available in: $scriptDir\bin\Debug\net9.0\TestsOutput" -ForegroundColor Cyan
        
        if ($Coverage) {
            Write-Host "`nCoverage reports are available in: $scriptDir\TestResults" -ForegroundColor Cyan
        }
    } else {
        Write-Host "`n? Tests failed!" -ForegroundColor Red
        exit $LASTEXITCODE
    }
} finally {
    Pop-Location
}

Write-Host "`n=== Examples ===" -ForegroundColor Cyan
Write-Host "# Run all tests:"
Write-Host "  .\Run-Tests.ps1"
Write-Host ""
Write-Host "# Run with coverage:"
Write-Host "  .\Run-Tests.ps1 -Coverage"
Write-Host ""
Write-Host "# Run with verbose output:"
Write-Host "  .\Run-Tests.ps1 -Verbose"
Write-Host ""
Write-Host "# Test only Cytoscape format:"
Write-Host "  .\Run-Tests.ps1 -Formats cytoscape"
Write-Host ""
Write-Host "# Use specific executable:"
Write-Host '  .\Run-Tests.ps1 -ExecutablePath "C:\path\to\CdxViz.exe"'
Write-Host ""
Write-Host "# Combine options:"
Write-Host "  .\Run-Tests.ps1 -Coverage -Verbose -Formats cytoscape"
