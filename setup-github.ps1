# GitHub Repository Setup - Automatisierte Befehle
# Führen Sie diese Befehle nach der GitHub Repository-Erstellung aus

Write-Host "================================================================" -ForegroundColor Cyan
Write-Host "    GITHUB REPOSITORY SETUP" -ForegroundColor Cyan
Write-Host "================================================================" -ForegroundColor Cyan
Write-Host ""

# Prüfe Git Status
Write-Host "🔍 Prüfe Git Repository Status..." -ForegroundColor Yellow
git status

Write-Host ""
Write-Host "📡 Verbinde mit GitHub Repository..." -ForegroundColor Yellow

# Entferne eventuell vorhandene Remote-Verbindung
try {
    git remote remove origin 2>$null
    Write-Host "✅ Alte Remote-Verbindung entfernt" -ForegroundColor Green
} catch {
    Write-Host "ℹ️  Keine vorherige Remote-Verbindung gefunden" -ForegroundColor Gray
}

# Füge neue Remote-Verbindung hinzu
git remote add origin https://github.com/Panicgrinder/ConanExilesOptimizer.git
Write-Host "✅ Remote Origin hinzugefügt" -ForegroundColor Green

# Setze Hauptbranch auf main
git branch -M main
Write-Host "✅ Hauptbranch auf 'main' gesetzt" -ForegroundColor Green

# Push zum GitHub Repository
Write-Host ""
Write-Host "🚀 Pushe Code zu GitHub..." -ForegroundColor Yellow
try {
    git push -u origin main
    Write-Host "✅ Code erfolgreich zu GitHub gepusht!" -ForegroundColor Green
    Write-Host ""
    Write-Host "================================================================" -ForegroundColor Green
    Write-Host "         GITHUB REPOSITORY ERFOLGREICH ERSTELLT!" -ForegroundColor Green
    Write-Host "================================================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "🌐 Repository URL: https://github.com/Panicgrinder/ConanExilesOptimizer" -ForegroundColor Cyan
    Write-Host "📦 Releases: https://github.com/Panicgrinder/ConanExilesOptimizer/releases" -ForegroundColor Cyan
    Write-Host "🐛 Issues: https://github.com/Panicgrinder/ConanExilesOptimizer/issues" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "🎯 Nächste Schritte:" -ForegroundColor Yellow
    Write-Host "1. Gehen Sie zu: https://github.com/Panicgrinder/ConanExilesOptimizer" -ForegroundColor White
    Write-Host "2. Erstellen Sie einen Release mit der .exe Datei" -ForegroundColor White
    Write-Host "3. Teilen Sie den Link mit Ihren Freunden!" -ForegroundColor White
    
} catch {
    Write-Host "❌ Push fehlgeschlagen!" -ForegroundColor Red
    Write-Host "Mögliche Ursachen:" -ForegroundColor Yellow
    Write-Host "- GitHub Repository wurde noch nicht erstellt" -ForegroundColor Gray
    Write-Host "- Falscher Repository-Name" -ForegroundColor Gray
    Write-Host "- Authentifizierungsprobleme" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Lösung: Erstellen Sie zuerst das Repository auf GitHub.com" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "📖 Für detaillierte Anweisungen siehe: docs\GitHub-Setup.md" -ForegroundColor Cyan
