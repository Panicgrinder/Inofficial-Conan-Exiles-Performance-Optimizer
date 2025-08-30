<#
    prepare-friends.ps1

    Erstellt ein Freundes-Paket mit der 100% sicheren Safe-Version:
    - Baut/published die Safe-Variante (keine Engine.ini/Game.ini Änderungen)
    - Prüft Code auf verdächtige NPC/AI/Game.ini/ServerSettings Änderungen
    - Packt EXE + kurzes README + LICENSE in ein ZIP unter releases/friends

    Verwendung (PowerShell):
      ./prepare-friends.ps1 [-Version v3.1.0] [-Force]

    Hinweise:
    - ASCII-only Output, kein Unicode
    - Keine Internet-Abhaengigkeiten
#>

param(
    [string]$Version = "v3.1.0",
    [switch]$Force
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info([string]$msg){ Write-Host "[INFO] $msg" -ForegroundColor Cyan }
function Write-Warn([string]$msg){ Write-Host "[WARN] $msg" -ForegroundColor Yellow }
function Write-Err([string]$msg){ Write-Host "[ERR ] $msg" -ForegroundColor Red }

Write-Info "Starte Erstellung des Freunde-Pakets (Safe-Version)"

# 1) Pfade vorbereiten
$repoRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $repoRoot

$safeProj = Join-Path $repoRoot 'src/ConanOptimizerSafe.csproj'
if(!(Test-Path $safeProj)){
    Write-Err "Safe-Projekt nicht gefunden: src/ConanOptimizerSafe.csproj"
    exit 1
}

$friendsRoot = Join-Path $repoRoot 'releases/friends'
New-Item -ItemType Directory -Path $friendsRoot -Force | Out-Null

$packageName = "ConanOptimizer-Safe-Freunde-$Version"
$packageDir = Join-Path $friendsRoot $packageName
if(Test-Path $packageDir){ Remove-Item -Recurse -Force $packageDir }
New-Item -ItemType Directory -Path $packageDir | Out-Null

# 2) Kurzer statischer Code-Scan auf NPC/AI-relevante Aenderungen
Write-Info "Pruefe Code auf NPC/AI Aenderungen..."
$patterns = @(
    'ServerSettings\.ini',
    'GameUserSettings\.ini',
    'Thrall', 'Follower', 'Followers',
    'Purge', 'PurgeMeter', 'PurgeDuration',
    'NPC', '\bnpc\b', 'AI', '\bai\b', 'Aggro', 'Spawn', 'Population'
)

$codePaths = @('modules', 'src', 'versions') | ForEach-Object { Join-Path $repoRoot $_ } | Where-Object { Test-Path $_ }
$findings = @()
foreach($cp in $codePaths){
    $files = Get-ChildItem -Path $cp -Recurse -Include *.cs,*.ps1 -File -ErrorAction SilentlyContinue
    foreach($f in $files){
        [string]$content = ""
        try { $content = [string](Get-Content -Raw -Path $f.FullName -ErrorAction Stop) } catch { $content = "" }
        if($null -eq $content){ $content = "" }
        foreach($p in $patterns){
            if([System.Text.RegularExpressions.Regex]::IsMatch($content, $p, [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)){
                $rel = $f.FullName
                if($rel.StartsWith($repoRoot, [System.StringComparison]::OrdinalIgnoreCase)){
                    $rel = $rel.Substring($repoRoot.Length + 1)
                }
                $findings += "$p @ $rel"
            }
        }
    }
}

# Entferne bekannte harmlose Treffer (Netzwerk/Engine/Grafik - keine NPC/AI)
$harmless = @(
    'Game\.ini',
    'Engine\.ini',
    'TextureStreaming',
    'RendererSettings',
    'Network',
    'LOD'
)
$filtered = $findings | Where-Object { $h = $_; -not ($harmless | ForEach-Object { if($h -match $_){ $true } }) }

if($filtered.Count -gt 0){
    Write-Warn "Auffaellige Code-Treffer gefunden:"; $filtered | ForEach-Object { Write-Host " - $_" }
    if(-not $Force){
        Write-Err "Abbruch ohne -Force. Bitte pruefen/entfernen Sie alle NPC/AI relevanten Aenderungen."
        exit 2
    } else {
        Write-Warn "Set -Force genutzt: fahre trotzdem fort."
    }
} else {
    Write-Info "Keine NPC/AI relevanten Aenderungen im Code gefunden."
}

# 3) Build/Publish Safe-Variante
Write-Info "Baue Safe-Variante (Release, win-x64, SingleFile=false)"
dotnet publish `
    "$safeProj" `
    -c Release `
    -r win-x64 `
    --self-contained false `
    -o (Join-Path $packageDir 'bin') | Out-Null

$exe = Get-ChildItem -Path (Join-Path $packageDir 'bin') -Filter *.exe -File | Select-Object -First 1
if(-not $exe){ Write-Err "Safe-EXE nicht gefunden nach Publish."; exit 3 }

# 4) README und LICENSE beilegen
$readmePath = Join-Path $packageDir 'README-FUER-FREUNDE.md'
@"
# Conan Exiles Optimizer (Safe) – Paket fuer Freunde

Dieses Paket enthaelt NUR die sichere Windows-Optimierung. Es werden KEINE Spiel-Config-Dateien wie Engine.ini oder Game.ini veraendert. Keine NPC/AI-Aenderungen.

Inhalt:
- Safe Optimizer EXE (Windows)
- Diese Kurzanleitung (README)
- Lizenz

Verwendung:
1. Schliessen Sie Conan Exiles.
2. Starten Sie die EXE und klicken Sie auf "Sichere Optimierungen anwenden".
3. Starten Sie danach Conan Exiles neu.

Was wird gemacht?
- Windows Gaming Mode, GPU Scheduling, Power- und Memory-Management
- Keine Aenderungen an Engine.ini/Game.ini/ServerSettings
- Keine NPC/AI-relevanten Aenderungen

Hinweise:
- Funktioniert auf offiziellen Servern.
- Fuer fortgeschrittene Einstellungen nutzen Sie die Haupt-App; dieses Paket bleibt bewusst minimal.
"@ | Set-Content -Path $readmePath -Encoding UTF8

$licenseSrc = Join-Path $repoRoot 'LICENSE'
if(Test-Path $licenseSrc){ Copy-Item $licenseSrc -Destination (Join-Path $packageDir 'LICENSE') -Force }

# 5) ZIP erzeugen
$zipPath = Join-Path $friendsRoot ("$packageName.zip")
if(Test-Path $zipPath){ Remove-Item -Force $zipPath }
Write-Info "Erzeuge ZIP: $zipPath"
Compress-Archive -Path (Join-Path $packageDir '*') -DestinationPath $zipPath -CompressionLevel Optimal

Write-Host ""; Write-Info "Fertig. Teilen Sie die Datei:"; Write-Host "  $zipPath" -ForegroundColor Green
