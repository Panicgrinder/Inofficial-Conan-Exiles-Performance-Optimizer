@echo off
echo ================================================================
echo     GITHUB REPOSITORY ERSTELLEN - BROWSER HELPER
echo ================================================================
echo.

echo 🌐 Öffne GitHub Repository-Erstellungsseite...
echo.
echo 📋 Kopieren Sie diese Daten:
echo Repository name: ConanExilesOptimizer
echo Description: 🗡️ Professional Performance Optimizer for Conan Exiles - Reduce loading times by 50-70%%
echo Visibility: Public
echo Initialize: KEINE Häkchen setzen (wir haben schon alles)
echo.

start https://github.com/new

echo ⏳ Warten Sie 5 Sekunden und drücken Sie dann ENTER wenn das Repository erstellt wurde...
pause

echo.
echo 🚀 Führe GitHub Setup aus...
powershell -ExecutionPolicy Bypass -File "setup-github.ps1"

echo.
echo ================================================================
echo          SETUP ABGESCHLOSSEN!
echo ================================================================
echo.
echo 🌐 Ihr Repository: https://github.com/Panicgrinder/ConanExilesOptimizer
echo 📦 Erstellen Sie jetzt einen Release mit der .exe Datei!
echo.
pause
