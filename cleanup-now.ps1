param([switch]$Execute)

$toDelete = @(
    "build.ps1",
    "docs\GitHub-Setup.md.backup",
    "ALLE-VERSIONEN-ÜBERSICHT.md",
    "releases\safe\ALLE-VERSIONEN-ÜBERSICHT.md",
    "releases\safe\BENUTZER-ANLEITUNG.md",
    "releases\safe\ERWEITERTE-EINSTELLUNGEN.md"
)

$deleted = @()
foreach ($p in $toDelete) {
    if (Test-Path $p) {
        if ($Execute) {
            Remove-Item $p -Force
            $deleted += $p
        } else {
            Write-Host "SIMULATE DELETE: $p" -ForegroundColor Yellow
        }
    }
}

if ($Execute) {
    Write-Host "Gelöscht: $($deleted.Count) Dateien" -ForegroundColor Green
} else {
    Write-Host "Simulation beendet. Für echte Bereinigung: .\cleanup-now.ps1 -Execute" -ForegroundColor Cyan
}
