# MARKDOWN LINT FIXER - Behebt automatisch alle Markdown-Formatierungsfehler

param(
    [string]$FilePath = "."
)

Write-Host "MARKDOWN LINT FIXER" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

# Finde alle Markdown-Dateien
$markdownFiles = Get-ChildItem -Path $FilePath -Recurse -Filter "*.md"

Write-Host "Gefundene Markdown-Dateien: $($markdownFiles.Count)" -ForegroundColor Yellow

foreach ($file in $markdownFiles) {
    Write-Host "Bearbeite: $($file.Name)" -ForegroundColor Green
    
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Fix 1: MD022 - Leerzeilen um Überschriften
    # Füge Leerzeile vor Überschriften hinzu (außer am Anfang)
    $content = $content -replace '(?<!^|\n\n)(#{1,6}\s)', "`n`n`$1"
    
    # Füge Leerzeile nach Überschriften hinzu (außer vor Listen oder anderen Überschriften)
    $content = $content -replace '(#{1,6}\s[^\n]+)(?!\n\n)(?!\n#{1,6})(?!\n[-*+])', "`$1`n"
    
    # Fix 2: MD032 - Leerzeilen um Listen
    # Füge Leerzeile vor Listen hinzu
    $content = $content -replace '(?<!^|\n\n)([-*+]\s)', "`n`n`$1"
    
    # Füge Leerzeile nach Listen hinzu
    $content = $content -replace '([-*+]\s[^\n]+(?:\n[-*+]\s[^\n]+)*)(?!\n\n)(?!\n[-*+])(?!\n#{1,6})', "`$1`n"
    
    # Fix 3: MD026 - Entferne Doppelpunkte aus Überschriften
    $content = $content -replace '(#{1,6}\s[^:\n]+):(\s*)$', '$1$2'
    
    # Fix 4: MD040 - Füge Sprach-Tags zu Code-Blöcken hinzu
    $content = $content -replace '```\n(?![\w-]+\n)', "```text`n"
    
    # Fix 5: MD031 - Leerzeilen um Code-Blöcke
    $content = $content -replace '(?<!^|\n\n)(```)', "`n`n`$1"
    $content = $content -replace '(```[^\n]*(?:\n(?!```)[^\n]*)*\n```)(?!\n\n)', "`$1`n"
    
    # Bereinige mehrfache Leerzeilen (max 2 aufeinander)
    $content = $content -replace '\n{3,}', "`n`n"
    
    # Entferne Leerzeilen am Ende der Datei
    $content = $content.TrimEnd()
    
    # Speichere nur wenn Änderungen vorgenommen wurden
    if ($content -ne $originalContent) {
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "  ✅ Behoben" -ForegroundColor Green
    } else {
        Write-Host "  ℹ️ Keine Änderungen nötig" -ForegroundColor Gray
    }
}

Write-Host "`nAlle Markdown-Dateien bearbeitet!" -ForegroundColor Cyan
