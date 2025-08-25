@echo off
echo ================================================================
echo     CONAN EXILES OPTIMIZER - EINFACHER BUILD
echo ================================================================
echo.

echo Wechsle in src Verzeichnis...
cd /d "F:\ConanExilesOptimizer\src"

echo.
echo Erstelle Release Build...
dotnet publish ConanOptimizer.csproj --configuration Release --output "..\releases\v3.0" --self-contained true --runtime win-x64 /p:PublishSingleFile=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ================================================================
    echo          BUILD ERFOLGREICH!
    echo ================================================================
    echo.
    echo Ausgabe: F:\ConanExilesOptimizer\releases\v3.0\ConanOptimizer.exe
    echo.
    explorer "..\releases\v3.0"
) else (
    echo.
    echo ================================================================
    echo          BUILD FEHLGESCHLAGEN!
    echo ================================================================
    pause
)

echo.
pause
