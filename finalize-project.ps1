param([switch]$Execute)

$ErrorActionPreference = 'Stop'

$timestamp = Get-Date -Format yyyyMMdd_HHmmss
$backupRoot = Join-Path ".finalize-backups" $timestamp

function BackupThenRemove($path){
  if(Test-Path $path){
    if($Execute){
      New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
      $rel = $path
      $dest = Join-Path $backupRoot ($rel -replace '[:\\\\/]', '_')
      if(Test-Path $path -PathType Container){
        Copy-Item $path $dest -Recurse -Force -ErrorAction SilentlyContinue
        Remove-Item $path -Recurse -Force
      } else {
        Copy-Item $path $dest -Force -ErrorAction SilentlyContinue
        Remove-Item $path -Force
      }
      Write-Host "Geloescht: $path" -ForegroundColor Red
    } else {
      Write-Host "SIMULATE DELETE: $path" -ForegroundColor Yellow
    }
  }
}

Write-Host "=== FINALIZE PROJECT: nur SAFE-Version behalten ===" -ForegroundColor Cyan
Write-Host ("Modus: {0}" -f ($(if($Execute){"EXECUTE"}else{"SIMULATE"}))) -ForegroundColor Gray

# 1) Versions-Ordner und Beta
@(
  "versions",
  "official-beta"
) | ForEach-Object { BackupThenRemove $_ }

# 2) Releases anderer Versionen
@(
  "releases\\advanced","releases\\original",
  "releases\\v3.0.0","releases\\v3.2.0-advanced","releases\\v3.1.0-safe"
) | ForEach-Object { BackupThenRemove $_ }

# 3) Redundante/alte Quell-Dateien und Projekte in src (Safe bleibt)
@(
  "src\\ConanOptimizer.cs",
  "src\\ConanOptimizer_New.cs",
  "src\\ConanOptimizerAdvancedSimple.cs",
  "src\\MainForm.cs",
  "src\\OptimizationApplier.cs",
  "src\\OptimizationSettings.cs",
  "src\\ConanOptimizerAdvanced.csproj",
  "src\\ConanOptimizerAdvancedSimple.csproj",
  "src\\ConanOptimizer.csproj"
) | ForEach-Object { BackupThenRemove $_ }

# 4) Build-Skripte bereinigen (behalte build-final.ps1)
Get-ChildItem -Path . -Filter "build-*.ps1" | Where-Object {
  $_.Name -ne "build-final.ps1"
} | ForEach-Object { BackupThenRemove $_.FullName }

Write-Host ""
if($Execute){
  Write-Host "FINALIZE ABGESCHLOSSEN. Backups: $backupRoot" -ForegroundColor Green
}else{
  Write-Host "Simulation beendet. Zur Ausfuehrung: .\\finalize-project.ps1 -Execute" -ForegroundColor Yellow
}
