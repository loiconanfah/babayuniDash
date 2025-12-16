@echo off
REM Script batch pour lancer ngrok (version simplifiée)
REM Utilise le script PowerShell pour plus de fonctionnalités

echo 🚀 Démarrage avec ngrok...
echo.

REM Vérifier si PowerShell est disponible
powershell -Command "if ($PSVersionTable.PSVersion.Major -lt 5) { exit 1 }" >nul 2>&1
if errorlevel 1 (
    echo ❌ PowerShell 5.0 ou supérieur est requis
    pause
    exit /b 1
)

REM Exécuter le script PowerShell
powershell -ExecutionPolicy Bypass -File "%~dp0start-with-ngrok.ps1" %*

if errorlevel 1 (
    echo.
    echo ❌ Erreur lors de l'exécution du script
    pause
)




