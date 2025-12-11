@echo off
REM Script batch pour lancer les deux instances du frontend en parallele
REM Ce script est execute automatiquement par Visual Studio au demarrage

setlocal enabledelayedexpansion

set "FRONTEND_PATH=%~dp0..\..\frontend"
set "LOG_FILE=%TEMP%\vite-multi-launch.log"

echo ======================================== >> "%LOG_FILE%"
echo Demarrage du script %date% %time% >> "%LOG_FILE%"
echo Chemin frontend: %FRONTEND_PATH% >> "%LOG_FILE%"
echo ======================================== >> "%LOG_FILE%"

echo ========================================
echo Lancement des instances frontend...
echo ========================================

REM Verifier que le dossier frontend existe
if not exist "%FRONTEND_PATH%" (
    echo ERREUR: Le dossier frontend n'existe pas a %FRONTEND_PATH%
    echo ERREUR: Le dossier frontend n'existe pas a %FRONTEND_PATH% >> "%LOG_FILE%"
    exit /b 1
)

REM Changer vers le dossier frontend
cd /d "%FRONTEND_PATH%"

REM Verifier que npm est disponible
where npm >nul 2>&1
if errorlevel 1 (
    echo ERREUR: npm n'est pas trouve dans le PATH
    echo ERREUR: npm n'est pas trouve dans le PATH >> "%LOG_FILE%"
    exit /b 1
)

REM Installer les dependances si necessaire
if not exist "node_modules" (
    echo Installation des dependances npm...
    call npm install
    if errorlevel 1 (
        echo ERREUR: Echec de l'installation des dependances
        exit /b 1
    )
)

REM Lancer l'instance 1 sur le port 5173 dans une nouvelle fenetre
echo Lancement de l'instance 1 sur le port 5173...
start "Vite Instance 1 (Port 5173)" cmd /k "cd /d %FRONTEND_PATH% && npm run dev:port1"
echo Instance 1 lancee >> "%LOG_FILE%"

REM Attendre un peu avant de lancer la deuxieme instance
timeout /t 2 /nobreak >nul

REM Lancer l'instance 2 sur le port 5174 dans une nouvelle fenetre
echo Lancement de l'instance 2 sur le port 5174...
start "Vite Instance 2 (Port 5174)" cmd /k "cd /d %FRONTEND_PATH% && npm run dev:port2"
echo Instance 2 lancee >> "%LOG_FILE%"

REM Attendre que le serveur backend soit pret avant d'ouvrir les navigateurs
echo En attente du serveur backend... >> "%LOG_FILE%"
:CHECK_BACKEND
netstat -ano | findstr ":5000" >nul 2>&1
if errorlevel 1 (
    timeout /t 1 /nobreak >nul
    goto CHECK_BACKEND
)
echo Serveur backend detecte sur le port 5000 >> "%LOG_FILE%"

REM Attendre un peu pour que les serveurs frontend demarrent aussi
timeout /t 3 /nobreak >nul

REM Ouvrir les navigateurs
echo Ouverture des navigateurs... >> "%LOG_FILE%"
start http://localhost:5173
timeout /t 1 /nobreak >nul
start http://localhost:5174

echo.
echo Les instances du frontend sont pretes! >> "%LOG_FILE%"
echo   Instance 1 : http://localhost:5173 >> "%LOG_FILE%"
echo   Instance 2 : http://localhost:5174 >> "%LOG_FILE%"
echo Log disponible dans: %LOG_FILE% >> "%LOG_FILE%"

endlocal
exit /b 0
