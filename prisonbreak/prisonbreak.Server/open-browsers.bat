@echo off
REM Script pour ouvrir les deux fenêtres du navigateur
REM S'exécute après le démarrage des serveurs

REM Attendre que les serveurs soient prêts
timeout /t 10 /nobreak >nul 2>&1

REM Vérifier si les serveurs répondent (maximum 30 secondes)
set /a attempts=0
:check_port1
netstat -an | findstr ":5173" >nul 2>&1
if %errorlevel% neq 0 (
    set /a attempts+=1
    if %attempts% gtr 15 goto timeout_error
    timeout /t 2 /nobreak >nul 2>&1
    goto check_port1
)

set /a attempts=0
:check_port2
netstat -an | findstr ":5174" >nul 2>&1
if %errorlevel% neq 0 (
    set /a attempts+=1
    if %attempts% gtr 15 goto timeout_error
    timeout /t 2 /nobreak >nul 2>&1
    goto check_port2
)

REM Ouvrir les deux fenêtres du navigateur
start "" "http://localhost:5173"
timeout /t 1 /nobreak >nul 2>&1
start "" "http://localhost:5174"
exit /b 0

:timeout_error
REM Ouvrir quand même les fenêtres même si les serveurs ne répondent pas encore
start "" "http://localhost:5173"
timeout /t 1 /nobreak >nul 2>&1
start "" "http://localhost:5174"
exit /b 0

