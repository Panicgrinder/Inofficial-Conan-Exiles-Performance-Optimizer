# Simple Build Script für Conan Exiles Optimizer

Write-Host "Building Conan Exiles Optimizer..."

# Prüfe .NET Installation
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET gefunden: Version $dotnetVersion"
} catch {
    Write-Host "❌ .NET SDK nicht gefunden!"
    exit 1
}

# Erstelle Output-Verzeichnis
$outputDir = "releases\v3.0.0"
if (Test-Path $outputDir) { 
    Remove-Item $outputDir -Recurse -Force 
}
New-Item -ItemType Directory -Path $outputDir -Force

# Build
Set-Location src
dotnet publish ConanOptimizer.csproj `
    --configuration Release `
    --output "..\$outputDir" `
    --self-contained true `
    --runtime win-x64 `
    /p:PublishSingleFile=true `
    /p:PublishReadyToRun=true `
    /p:IncludeNativeLibrariesForSelfExtract=true

Set-Location ..

if (Test-Path "$outputDir\ConanOptimizer.exe") {
    $fileSize = (Get-Item "$outputDir\ConanOptimizer.exe").Length / 1MB
    Write-Host "✅ Build erfolgreich!"
    Write-Host "📁 Pfad: $outputDir\ConanOptimizer.exe"
    Write-Host "📏 Größe: $([math]::Round($fileSize, 1)) MB"
} else {
    Write-Host "❌ Build fehlgeschlagen!"
    exit 1
}
