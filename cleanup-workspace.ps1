# VORSICHTIGE WORKSPACE-BEREINIGUNG
# Analysiert zuerst, dann fragt nach Bestätigung

param(
    [switch]$Execute
)

Write-Host "🔍 CONAN EXILES OPTIMIZER - WORKSPACE ANALYSE" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# Standardverhalten: Simulation (außer -Execute ist gesetzt)
$Simulate = -not $Execute

if ($Simulate) {
    Write-Host "⚠️  SIMULATIONS-MODUS - Keine Dateien werden gelöscht!" -ForegroundColor Yellow
    Write-Host "   Verwende -Execute für echte Bereinigung" -ForegroundColor Gray
}

# PHASE 1: IDENTIFIZIERE REDUNDANTE BUILD-SCRIPTS
Write-Host ""
Write-Host "📋 ANALYSE: Build-Scripts" -ForegroundColor Yellow

$allBuilds = Get-ChildItem "build-*.ps1" | Sort-Object Name
$keepBuild = "build-modular.ps1"  # Der funktionsfähige

Write-Host "   Gefunden: $($allBuilds.Count) Build-Scripts" -ForegroundColor Cyan

foreach ($build in $allBuilds) {
    if ($build.Name -eq $keepBuild) {
        Write-Host "   ✅ BEHALTEN: $($build.Name) (funktionsfähig)" -ForegroundColor Green
    } else {
        $sizeKB = [math]::Round($build.Length / 1KB, 1)
        Write-Host "   ❌ REDUNDANT: $($build.Name) ($sizeKB KB)" -ForegroundColor Red
        
        if (-not $Simulate) {
            # Backup erstellen vor Löschung
            $backupName = "backup_$($build.Name)"
            Copy-Item $build.FullName $backupName
            Write-Host "     📁 Backup: $backupName" -ForegroundColor Gray
        }
    }
}

# PHASE 2: IDENTIFIZIERE REDUNDANTE TEST-SCRIPTS  
Write-Host ""
Write-Host "📋 ANALYSE: Test-Scripts" -ForegroundColor Yellow

$allTests = Get-ChildItem "test-*.ps1" | Sort-Object Name
Write-Host "   Gefunden: $($allTests.Count) Test-Scripts" -ForegroundColor Cyan

foreach ($test in $allTests) {
    $sizeKB = [math]::Round($test.Length / 1KB, 1)
    Write-Host "   ❌ REDUNDANT: $($test.Name) ($sizeKB KB) - Nur für Development" -ForegroundColor Red
    
    if (-not $Simulate) {
        $backupName = "backup_$($test.Name)"
        Copy-Item $test.FullName $backupName
        Write-Host "     📁 Backup: $backupName" -ForegroundColor Gray
    }
}

# PHASE 3: IDENTIFIZIERE DOKUMENTATIONS-DUPLIKATE
Write-Host ""
Write-Host "📋 ANALYSE: Dokumentation" -ForegroundColor Yellow

$docFiles = @()
$docFiles += Get-ChildItem "ERWEITERTE-EINSTELLUNGEN.md" -ErrorAction SilentlyContinue
$docFiles += Get-ChildItem "releases\*\ERWEITERTE-EINSTELLUNGEN.md" -ErrorAction SilentlyContinue
$docFiles += Get-ChildItem "ALLE-VERSIONEN-ÜBERSICHT.md" -ErrorAction SilentlyContinue
$docFiles += Get-ChildItem "releases\*\ALLE-VERSIONEN-ÜBERSICHT.md" -ErrorAction SilentlyContinue

$masterDoc = "modules\documentation\"
Write-Host "   Master-Quelle: $masterDoc" -ForegroundColor Green

foreach ($doc in $docFiles) {
    if ($doc.FullName -notlike "*modules\documentation*") {
        $sizeKB = [math]::Round($doc.Length / 1KB, 1)
        Write-Host "   ❌ DUPLIKAT: $($doc.FullName) ($sizeKB KB)" -ForegroundColor Red
        
        if (-not $Simulate) {
            $backupName = "backup_doc_$([System.IO.Path]::GetFileNameWithoutExtension($doc.Name))_$(Get-Date -Format 'HHmm').md"
            Copy-Item $doc.FullName $backupName
            Write-Host "     📁 Backup: $backupName" -ForegroundColor Gray
        }
    }
}

# PHASE 4: ZUSAMMENFASSUNG
Write-Host ""
Write-Host "📊 ZUSAMMENFASSUNG:" -ForegroundColor Cyan

$redundantBuilds = ($allBuilds | Where-Object { $_.Name -ne $keepBuild }).Count
$redundantTests = $allTests.Count
$redundantDocs = ($docFiles | Where-Object { $_.FullName -notlike "*modules\documentation*" }).Count

Write-Host "   🔧 Redundante Build-Scripts: $redundantBuilds" -ForegroundColor Yellow
Write-Host "   🧪 Redundante Test-Scripts: $redundantTests" -ForegroundColor Yellow  
Write-Host "   📝 Redundante Dokumentation: $redundantDocs" -ForegroundColor Yellow
Write-Host "   💾 Gesamt zu bereinigen: $($redundantBuilds + $redundantTests + $redundantDocs) Dateien" -ForegroundColor Cyan

if ($Simulate) {
    Write-Host ""
    Write-Host "⚠️  SIMULATION ABGESCHLOSSEN" -ForegroundColor Yellow
    Write-Host "   Für echte Bereinigung: .\cleanup-workspace.ps1 -Execute" -ForegroundColor White
} else {
    Write-Host ""
    Write-Host "⚠️  BEREINIGUNG WÜRDE JETZT STARTEN..." -ForegroundColor Yellow
    $confirmation = Read-Host "Fortsetzten? (ja/nein)"
    
    if ($confirmation -eq "ja") {
        Write-Host "🗑️  BEREINIGUNG STARTET..." -ForegroundColor Red
        
        # Lösche redundante Builds (außer dem funktionsfähigen)
        foreach ($build in $allBuilds) {
            if ($build.Name -ne $keepBuild) {
                Remove-Item $build.FullName -Force
                Write-Host "   ❌ Gelöscht: $($build.Name)" -ForegroundColor Red
            }
        }
        
        # Lösche redundante Tests
        foreach ($test in $allTests) {
            Remove-Item $test.FullName -Force
            Write-Host "   ❌ Gelöscht: $($test.Name)" -ForegroundColor Red
        }
        
        # Lösche redundante Dokumentation
        foreach ($doc in $docFiles) {
            if ($doc.FullName -notlike "*modules\documentation*") {
                Remove-Item $doc.FullName -Force
                Write-Host "   ❌ Gelöscht: $($doc.FullName)" -ForegroundColor Red
            }
        }
        
        Write-Host ""
        Write-Host "✅ BEREINIGUNG ABGESCHLOSSEN!" -ForegroundColor Green
        Write-Host "📁 Backups erstellt für alle gelöschten Dateien" -ForegroundColor Cyan
    } else {
        Write-Host "❌ Bereinigung abgebrochen" -ForegroundColor Red
    }
}
