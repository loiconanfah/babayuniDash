@echo off
REM Script batch pour lancer deux instances du frontend
REM Appelé par Visual Studio via le SPA Proxy

echo 🚀 Lancement de deux instances du frontend...
echo.

cd ..\..\frontend

REM Lancer la première instance
echo 🌐 Instance 1 - Port 5173...
start "Instance 1 - Port 5173" powershell -NoExit -Command "cd '%CD%'; Write-Host '=== INSTANCE 1 - PORT 5173 ===' -ForegroundColor Green; npm run dev:port1"

timeout /t 3 /nobreak >nul

REM Lancer la deuxième instance
echo 🌐 Instance 2 - Port 5174...
start "Instance 2 - Port 5174" powershell -NoExit -Command "cd '%CD%'; Write-Host '=== INSTANCE 2 - PORT 5174 ===' -ForegroundColor Blue; npm run dev:port2"

echo.
echo ✅ Deux instances lancées !
echo    Instance 1 : http://localhost:5173
echo    Instance 2 : http://localhost:5174
echo.

REM Attendre indéfiniment pour que le script ne se termine pas
REM (Visual Studio attend que ce script se termine)
:loop
timeout /t 60 /nobreak >nul
goto loop

