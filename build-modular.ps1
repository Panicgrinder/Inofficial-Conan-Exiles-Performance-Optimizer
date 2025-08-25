# Einfacher Build Script für modulare Versionen

param([string]$Version = "safe")

Write-Host "CONAN EXILES OPTIMIZER - MODULARER BUILD" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Erstelle Output-Verzeichnis
$outputDir = "releases\$Version"
if (Test-Path $outputDir) { 
    Remove-Item $outputDir -Recurse -Force 
}
New-Item -ItemType Directory -Path $outputDir -Force

Write-Host ""
Write-Host "Building $Version Version..." -ForegroundColor Yellow

# Wechsle in Versionsordner
Set-Location "versions\$Version"

# Bestimme Projekt-Datei
$projectFile = ""
if ($Version -eq "safe") { $projectFile = "SafeOptimizer.csproj" }
elseif ($Version -eq "original") { $projectFile = "OriginalOptimizer.csproj" }
elseif ($Version -eq "advanced") { $projectFile = "AdvancedOptimizer.csproj" }
else {
    Write-Host "Unbekannte Version: $Version" -ForegroundColor Red
    exit 1
}

# Build
dotnet publish $projectFile `
    --configuration Release `
    --output "..\..\$outputDir" `
    --self-contained true `
    --runtime win-x64 `
    /p:PublishSingleFile=true `
    /p:PublishReadyToRun=true `
    /p:IncludeNativeLibrariesForSelfExtract=true

Set-Location ..\..

# Prüfe Ergebnis
$exeFiles = Get-ChildItem "$outputDir\*.exe"
if ($exeFiles.Count -gt 0) {
    $fileSize = ($exeFiles[0].Length / 1MB)
    Write-Host ""
    Write-Host "Build erfolgreich!" -ForegroundColor Green
    Write-Host "Datei: $($exeFiles[0].Name)" -ForegroundColor Cyan
    Write-Host "Groesse: $([math]::Round($fileSize, 1)) MB" -ForegroundColor Cyan
    
    # Kopiere Dokumentation
    Copy-Item "modules\documentation\*" $outputDir -Force -ErrorAction SilentlyContinue
    Write-Host "Dokumentation kopiert" -ForegroundColor Green
} else {
    Write-Host "Build fehlgeschlagen!" -ForegroundColor Red
    exit 1
}
