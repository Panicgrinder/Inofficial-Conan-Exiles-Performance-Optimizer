# VORSICHTIGE WORKSPACE-BEREINIGUNG
# Analysiert zuerst, dann fragt nach Bestätigung

param(
    [switch]$Execute,
    [switch]$Force,
    [switch]$PurgeUIBackups
)

# Zentrales Backup-Verzeichnis fuer Bereinigungen
$backupDir = ".cleanup-backups"

Write-Host "CONAN EXILES OPTIMIZER - WORKSPACE ANALYSE" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan

# Standardverhalten: Simulation (ausser -Execute ist gesetzt)
$Simulate = -not $Execute

if ($Simulate) {
    Write-Host "SIMULATIONS-MODUS - Keine Dateien werden geloescht!" -ForegroundColor Yellow
    Write-Host "   Verwende -Execute fuer echte Bereinigung" -ForegroundColor Gray
}

# Stelle sicher, dass das Backup-Verzeichnis existiert (nur bei echter Ausfuehrung)
if (-not $Simulate) {
    if (-not (Test-Path -LiteralPath $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
        Write-Host ("Backup-Ordner erstellt: {0}" -f $backupDir) -ForegroundColor DarkGray
    }
}

# Migriere vorhandene Backups im Root in den Backup-Ordner
$existingBackups = Get-ChildItem -Path . -File -Filter 'backup_*' -ErrorAction SilentlyContinue
if ($existingBackups -and $existingBackups.Count -gt 0) {
    Write-Host ""; Write-Host "MIGRATION: vorhandene Backups verschieben" -ForegroundColor Yellow
    foreach ($b in $existingBackups) {
        $target = Join-Path $backupDir $b.Name
        if ($Simulate) {
            Write-Host ("   Wuerde verschieben: {0} -> {1}" -f $b.FullName, $target) -ForegroundColor Yellow
        } else {
            Move-Item -LiteralPath $b.FullName -Destination $target -Force
            Write-Host ("   Verschoben: {0} -> {1}" -f $b.Name, $target) -ForegroundColor DarkYellow
        }
    }
}

# PHASE 1: IDENTIFIZIERE REDUNDANTE BUILD-SCRIPTS
Write-Host ""
Write-Host "ANALYSE: Build-Scripts" -ForegroundColor Yellow

$allBuilds = Get-ChildItem "build-*.ps1" | Sort-Object Name
$keepBuild = "build-modular.ps1"  # Der funktionsfähige

Write-Host ("   Gefunden: {0} Build-Scripts" -f $allBuilds.Count) -ForegroundColor Cyan

foreach ($build in $allBuilds) {
    if ($build.Name -eq $keepBuild) {
    Write-Host ("   BEHALTEN: {0} (funktionsfaehig)" -f $build.Name) -ForegroundColor Green
    } else {
        $sizeKB = [math]::Round($build.Length / 1KB, 1)
    Write-Host ("   REDUNDANT: {0} ({1} KB)" -f $build.Name, $sizeKB) -ForegroundColor Red
        
        if (-not $Simulate) {
            # Backup erstellen vor Löschung
            $backupName = "backup_$($build.Name)"
            $backupPath = Join-Path $backupDir $backupName
            Copy-Item $build.FullName $backupPath
            Write-Host ("     Backup: {0}" -f $backupPath) -ForegroundColor Gray
        }
    }
}

# Zusätzlich: historisches Root-Build-Skript separat erfassen
$rootBuild = $null
if (Test-Path "build.ps1") {
    $rootBuild = Get-Item "build.ps1"
    Write-Host "   REDUNDANT: build.ps1" -ForegroundColor Red
    if (-not $Simulate) {
    $backupName = "backup_build.ps1"
    $backupPath = Join-Path $backupDir $backupName
    Copy-Item $rootBuild.FullName $backupPath
    Write-Host ("     Backup: {0}" -f $backupPath) -ForegroundColor Gray
    }
}

# PHASE 2: IDENTIFIZIERE REDUNDANTE TEST-SCRIPTS  
Write-Host ""
Write-Host "ANALYSE: Test-Scripts" -ForegroundColor Yellow

$allTests = Get-ChildItem "test-*.ps1" | Sort-Object Name
Write-Host ("   Gefunden: {0} Test-Scripts" -f $allTests.Count) -ForegroundColor Cyan

foreach ($test in $allTests) {
    $sizeKB = [math]::Round($test.Length / 1KB, 1)
    Write-Host ("   REDUNDANT: {0} ({1} KB) - Nur fuer Development" -f $test.Name, $sizeKB) -ForegroundColor Red
    
    if (-not $Simulate) {
    $backupName = "backup_$($test.Name)"
    $backupPath = Join-Path $backupDir $backupName
    Copy-Item $test.FullName $backupPath
    Write-Host ("     Backup: {0}" -f $backupPath) -ForegroundColor Gray
    }
}

# PHASE 3: IDENTIFIZIERE DOKUMENTATIONS-DUPLIKATE
Write-Host ""
Write-Host "ANALYSE: Dokumentation" -ForegroundColor Yellow

$docFiles = @()
# Root-Duplikate bekannter Dateien
$docFiles += Get-ChildItem "ERWEITERTE-EINSTELLUNGEN.md" -ErrorAction SilentlyContinue
$docFiles += Get-ChildItem "ALLE-VERSIONEN-ÜBERSICHT.md" -ErrorAction SilentlyContinue
$docFiles += Get-ChildItem "BENUTZER-ANLEITUNG.md" -ErrorAction SilentlyContinue
# Alle Markdown-Dateien in Releases (jegliche README/Anleitungen)
$docFiles += Get-ChildItem "releases\*\*.md" -ErrorAction SilentlyContinue

$masterDoc = 'modules\documentation\'
Write-Host ("   Master-Quelle: {0}" -f $masterDoc) -ForegroundColor Green

foreach ($doc in $docFiles) {
    if ($doc.FullName -notlike '*modules\documentation*') {
        $sizeKB = [math]::Round($doc.Length / 1KB, 1)
        Write-Host ("   DUPLIKAT: {0} ({1} KB)" -f $doc.FullName, $sizeKB) -ForegroundColor Red
        
        if (-not $Simulate) {
            $backupName = "backup_doc_$([System.IO.Path]::GetFileNameWithoutExtension($doc.Name))_$(Get-Date -Format 'HHmm').md"
            $backupPath = Join-Path $backupDir $backupName
            Copy-Item $doc.FullName $backupPath
            Write-Host ("     Backup: {0}" -f $backupPath) -ForegroundColor Gray
        }
    }
}

# PHASE 3b: UI-DOKU-BACKUPS IM BACKUP-ORDNER
Write-Host ""
Write-Host "ANALYSE: UI-Dokumentations-Backups" -ForegroundColor Yellow

$uiBackupDocs = @()
if (Test-Path -LiteralPath $backupDir) {
    $uiBackupDocs = Get-ChildItem -Path $backupDir -File -Filter 'backup_doc_*.md' -ErrorAction SilentlyContinue
}
Write-Host ("   Gefunden: {0} UI-Backup-Dokumente" -f ($uiBackupDocs.Count)) -ForegroundColor Cyan

# PHASE 4: ZUSAMMENFASSUNG
Write-Host ""
Write-Host "ZUSAMMENFASSUNG:" -ForegroundColor Cyan

$redundantBuilds = ($allBuilds | Where-Object { $_.Name -ne $keepBuild }).Count + ([int]([bool]$rootBuild))
$redundantTests = $allTests.Count
$redundantDocs = ($docFiles | Where-Object { $_.FullName -notlike '*modules\documentation*' }).Count
$uiBackupCount = $uiBackupDocs.Count

Write-Host ("   Redundante Build-Scripts: {0}" -f $redundantBuilds) -ForegroundColor Yellow
Write-Host ("   Redundante Test-Scripts: {0}" -f $redundantTests) -ForegroundColor Yellow  
Write-Host ("   Redundante Dokumentation: {0}" -f $redundantDocs) -ForegroundColor Yellow
Write-Host ("   UI-Backup-Dokumente: {0}" -f $uiBackupCount) -ForegroundColor Yellow
Write-Host ("   Gesamt zu bereinigen: {0} Dateien" -f ($redundantBuilds + $redundantTests + $redundantDocs + $uiBackupCount)) -ForegroundColor Cyan

if ($Simulate) {
    Write-Host ""
    Write-Host "SIMULATION ABGESCHLOSSEN" -ForegroundColor Yellow
    Write-Host "   Fuer echte Bereinigung: .\cleanup-workspace.ps1 -Execute" -ForegroundColor White
} else {
    Write-Host ""
    if (-not $Force) {
        Write-Host "BEREINIGUNG WUERDE JETZT STARTEN..." -ForegroundColor Yellow
        $confirmation = Read-Host "Fortsetzen? (ja/nein)"
        if ($confirmation -ne "ja") {
            Write-Host "Bereinigung abgebrochen" -ForegroundColor Red
            return
        }
    } else {
        Write-Host "-Force aktiv: Bestaetigungsabfrage uebersprungen" -ForegroundColor Yellow
    }

    if ($Force -or $confirmation -eq "ja") {
        Write-Host "BEREINIGUNG STARTET..." -ForegroundColor Red
        
        # Lösche redundante Builds (außer dem funktionsfähigen)
        foreach ($build in $allBuilds) {
            if ($build.Name -ne $keepBuild) {
                Remove-Item $build.FullName -Force
                Write-Host ("   Geloescht: {0}" -f $build.Name) -ForegroundColor Red
            }
        }
        if ($rootBuild) {
            Remove-Item $rootBuild.FullName -Force
            Write-Host "   Geloescht: build.ps1" -ForegroundColor Red
        }
        
        # Lösche redundante Tests
        foreach ($test in $allTests) {
            Remove-Item $test.FullName -Force
            Write-Host ("   Geloescht: {0}" -f $test.Name) -ForegroundColor Red
        }
        
        # Lösche redundante Dokumentation
        foreach ($doc in $docFiles) {
            if ($doc.FullName -notlike '*modules\documentation*') {
                Remove-Item $doc.FullName -Force
                Write-Host ("   Geloescht: {0}" -f $doc.FullName) -ForegroundColor Red
            }
        }

        # Lösche UI-Backup-Dokumente im Backup-Ordner (nur wenn explizit gewuenscht)
        if ($PurgeUIBackups -and $uiBackupDocs -and $uiBackupDocs.Count -gt 0) {
            foreach ($b in $uiBackupDocs) {
                Remove-Item -LiteralPath $b.FullName -Force
                Write-Host ("   Geloescht (UI-Backup): {0}" -f $b.Name) -ForegroundColor Red
            }
        } elseif ($uiBackupDocs.Count -gt 0) {
            Write-Host "   Hinweis: UI-Backups nicht geloescht (verwende -PurgeUIBackups)" -ForegroundColor DarkYellow
        }
        
        Write-Host ""
        Write-Host "BEREINIGUNG ABGESCHLOSSEN!" -ForegroundColor Green
        Write-Host "Backups erstellt fuer alle geloeschten Dateien" -ForegroundColor Cyan
    } else {
        Write-Host "Bereinigung abgebrochen" -ForegroundColor Red
    }
}
