@echo off
REM =====================================================
REM Script de démarrage pour le projet Hashi (Windows)
REM Lance automatiquement le backend ET le client Vue.js
REM =====================================================

echo ========================================
echo 🚀 Demarrage du projet Hashi...
echo ========================================
echo.

REM Lancer le backend dans un nouveau terminal
echo 📡 Demarrage du backend ASP.NET Core...
start "Hashi Backend" cmd /k "cd prisonbreak.Server && dotnet run --launch-profile https"

REM Attendre 5 secondes
timeout /t 5 /nobreak > nul

REM Lancer le client dans un nouveau terminal
echo 🎨 Demarrage du client Vue.js...
start "Hashi Client" cmd /k "cd prisonbreak.client && npm run dev"

REM Attendre 5 secondes
timeout /t 5 /nobreak > nul

echo.
echo ========================================
echo ✅ Projet Hashi demarre avec succes !
echo ========================================
echo.
echo 📊 Informations importantes:
echo    • Backend:     https://localhost:5001
echo    • Client:      http://localhost:5173
echo    • Swagger:     https://localhost:5001/swagger
echo.
echo 🌐 Ouverture du navigateur...
timeout /t 2 /nobreak > nul
start http://localhost:5173

echo.
echo ℹ️  Deux fenetres de terminal sont ouvertes:
echo    1. Hashi Backend (ASP.NET Core)
echo    2. Hashi Client (Vue.js/Vite)
echo.
echo ⚠️  Pour arreter les serveurs:
echo    Fermez les deux fenetres de terminal
echo.
echo 🎉 Bon developpement !
echo.
pause

