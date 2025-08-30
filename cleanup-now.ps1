param([switch]$Execute)

# Muster für direkte Einmal-Bereinigung
$patterns = @(
    "build.ps1",
    "docs\GitHub-Setup.md.backup",
    "ALLE-VERSIONEN-ÜBERSICHT.md",
    "BENUTZER-ANLEITUNG.md",
    "ERWEITERTE-EINSTELLUNGEN.md",
    "releases\*\*.md"
)

$targets = @()
foreach ($pat in $patterns) {
    $targets += Get-ChildItem $pat -ErrorAction SilentlyContinue
}
$targets = $targets | Sort-Object FullName -Unique

if (-not $targets) {
    Write-Host "Nichts zu löschen gefunden." -ForegroundColor Cyan
    return
}

$deleted = @()
foreach ($t in $targets) {
    if ($Execute) {
        Remove-Item $t.FullName -Force
        $deleted += $t.FullName
        Write-Host "Gelöscht: $($t.FullName)" -ForegroundColor Red
    } else {
        Write-Host "SIMULATE DELETE: $($t.FullName)" -ForegroundColor Yellow
    }
}

if ($Execute) {
    Write-Host "Gesamt gelöscht: $($deleted.Count) Dateien" -ForegroundColor Green
} else {
    Write-Host "Simulation beendet. Für echte Bereinigung: .\cleanup-now.ps1 -Execute" -ForegroundColor Cyan
}
