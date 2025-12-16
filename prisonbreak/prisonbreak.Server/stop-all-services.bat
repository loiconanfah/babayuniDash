@echo off
REM Script pour arreter tous les services du projet (Vite et backend)

echo ========================================
echo Arret de tous les services...
echo ========================================

REM Arreter les instances Vite sur les ports 5173 et 5174
echo Arret des instances Vite...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5173" ^| findstr "LISTENING"') do (
    echo Arret du processus Vite (port 5173) PID: %%a
    taskkill /F /PID %%a >nul 2>&1
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5174" ^| findstr "LISTENING"') do (
    echo Arret du processus Vite (port 5174) PID: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM Arreter le serveur backend sur les ports 5000 et 5001
echo Arret du serveur backend...
for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5000" ^| findstr "LISTENING"') do (
    echo Arret du processus backend (port 5000) PID: %%a
    taskkill /F /PID %%a >nul 2>&1
)

for /f "tokens=5" %%a in ('netstat -ano ^| findstr ":5001" ^| findstr "LISTENING"') do (
    echo Arret du processus backend (port 5001) PID: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM Arreter tous les processus dotnet en cours (sauf si c'est Visual Studio lui-meme)
echo Arret des processus dotnet (serveur backend)...
for /f "tokens=2" %%a in ('tasklist /FI "IMAGENAME eq dotnet.exe" /FO LIST ^| findstr "PID"') do (
    echo Arret du processus dotnet PID: %%a
    taskkill /F /PID %%a >nul 2>&1
)

REM Arreter tous les processus node (Vite utilise Node.js)
echo Arret des processus node (Vite)...
taskkill /F /IM node.exe >nul 2>&1

REM Arreter les fenetres de commande Vite
echo Arret des fenetres de commande Vite...
taskkill /F /FI "WINDOWTITLE eq Vite Instance 1*" >nul 2>&1
taskkill /F /FI "WINDOWTITLE eq Vite Instance 2*" >nul 2>&1

echo.
echo Tous les services ont ete arretes!
echo.
pause

