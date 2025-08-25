# Build Script für SICHERE Conan Exiles Optimizer Version

Write-Host "CONAN EXILES OPTIMIZER - SICHERE VERSION"
Write-Host "100% Ban-sicher für Online-Server"

# Prüfe .NET Installation
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET gefunden: Version $dotnetVersion"
} catch {
    Write-Host "❌ .NET SDK nicht gefunden!"
    exit 1
}

# Erstelle Output-Verzeichnis für sichere Version
$outputDir = "releases\v3.1.0-safe"
if (Test-Path $outputDir) { 
    Remove-Item $outputDir -Recurse -Force 
}
New-Item -ItemType Directory -Path $outputDir -Force

Write-Host "Baue SICHERE Version..."

# Build sichere Version
Set-Location src
dotnet publish ConanOptimizerSafe.csproj `
    --configuration Release `
    --output "..\$outputDir" `
    --self-contained true `
    --runtime win-x64 `
    /p:PublishSingleFile=true `
    /p:PublishReadyToRun=true `
    /p:IncludeNativeLibrariesForSelfExtract=true

Set-Location ..

if (Test-Path "$outputDir\ConanOptimizerSafe.exe") {
    $fileSize = (Get-Item "$outputDir\ConanOptimizerSafe.exe").Length / 1MB
    Write-Host ""
    Write-Host "✅ SICHERE VERSION erfolgreich erstellt!"
    Write-Host "Pfad: $outputDir\ConanOptimizerSafe.exe"
    Write-Host "Größe: $([math]::Round($fileSize, 1)) MB"
    Write-Host ""
    Write-Host "SICHERHEITSGARANTIE:"
    Write-Host "- KEINE Config-Datei Änderungen"
    Write-Host "- KEINE Spieldatei-Modifikationen" 
    Write-Host "- Nur Windows-Optimierungen"
} else {
    Write-Host "❌ Build fehlgeschlagen!"
    exit 1
}
