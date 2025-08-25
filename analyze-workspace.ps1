# SICHERE WORKSPACE-ANALYSE
# Nur Analyse, keine Loeschung

Write-Host "CONAN EXILES OPTIMIZER - WORKSPACE ANALYSE" -ForegroundColor Cyan
Write-Host "===========================================" -ForegroundColor Cyan

Write-Host ""
Write-Host "BUILD-SCRIPTS ANALYSE:" -ForegroundColor Yellow

$buildFiles = Get-ChildItem "build-*.ps1" | Sort-Object Name -Unique
Write-Host "Gefundene Build-Scripts: $($buildFiles.Count)" -ForegroundColor Cyan

foreach ($file in $buildFiles) {
    $sizeKB = [math]::Round($file.Length / 1KB, 1)
    
    if ($file.Name -eq "build-modular.ps1") {
        Write-Host "  BEHALTEN: $($file.Name) ($sizeKB KB) - Funktionsfaehig" -ForegroundColor Green
    } else {
        Write-Host "  REDUNDANT: $($file.Name) ($sizeKB KB)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "TEST-SCRIPTS ANALYSE:" -ForegroundColor Yellow

$testFiles = Get-ChildItem "test-*.ps1" | Sort-Object Name -Unique  
Write-Host "Gefundene Test-Scripts: $($testFiles.Count)" -ForegroundColor Cyan

foreach ($file in $testFiles) {
    $sizeKB = [math]::Round($file.Length / 1KB, 1)
    Write-Host "  REDUNDANT: $($file.Name) ($sizeKB KB) - Development only" -ForegroundColor Red
}

Write-Host ""
Write-Host "DOKUMENTATIONS-ANALYSE:" -ForegroundColor Yellow

$docDuplicates = @()
if (Test-Path "ERWEITERTE-EINSTELLUNGEN.md") {
    $docDuplicates += Get-Item "ERWEITERTE-EINSTELLUNGEN.md"
}

$releasesDocs = Get-ChildItem "releases\*\*.md" -ErrorAction SilentlyContinue
$docDuplicates += $releasesDocs

Write-Host "Master-Dokumentation: modules\documentation\" -ForegroundColor Green
Write-Host "Gefundene Duplikate: $($docDuplicates.Count)" -ForegroundColor Cyan

foreach ($file in $docDuplicates) {
    $sizeKB = [math]::Round($file.Length / 1KB, 1)
    Write-Host "  DUPLIKAT: $($file.FullName) ($sizeKB KB)" -ForegroundColor Red
}

Write-Host ""
Write-Host "ZUSAMMENFASSUNG:" -ForegroundColor Cyan

$redundantBuilds = ($buildFiles | Where-Object { $_.Name -ne "build-modular.ps1" }).Count
$redundantTests = $testFiles.Count
$redundantDocs = $docDuplicates.Count

Write-Host "Redundante Build-Scripts: $redundantBuilds" -ForegroundColor Yellow
Write-Host "Redundante Test-Scripts: $redundantTests" -ForegroundColor Yellow
Write-Host "Redundante Dokumentation: $redundantDocs" -ForegroundColor Yellow
Write-Host "GESAMT ZU BEREINIGEN: $($redundantBuilds + $redundantTests + $redundantDocs) Dateien" -ForegroundColor Cyan

Write-Host ""
Write-Host "NAECHSTE SCHRITTE:" -ForegroundColor Green
Write-Host "1. Pruefen Sie die Analyse-Ergebnisse" -ForegroundColor White
Write-Host "2. Bei Bestaetigung: Manuelle Bereinigung" -ForegroundColor White
Write-Host "3. Behalten Sie build-modular.ps1 und modules\documentation\" -ForegroundColor White
