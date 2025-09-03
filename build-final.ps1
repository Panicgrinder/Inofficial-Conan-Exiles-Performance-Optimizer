$ErrorActionPreference = 'Stop'
Write-Host "BUILD FINAL (Safe)" -ForegroundColor Cyan

$newOut = "releases\final"
if(Test-Path $newOut){ Remove-Item $newOut -Recurse -Force }
New-Item -ItemType Directory -Path $newOut | Out-Null

# Publish die Safe-Projektdatei
& dotnet publish `
  .\src\ConanOptimizerSafe.csproj `
  -c Release -r win-x64 --self-contained true `
  -p:PublishSingleFile=true `
  -o $newOut

Write-Host "Fertig: $newOut" -ForegroundColor Green
