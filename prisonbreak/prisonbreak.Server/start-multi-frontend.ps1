# Script pour lancer deux instances du frontend
# Appelé automatiquement par Visual Studio via le SPA Proxy

$ErrorActionPreference = "Continue"

# Chemin vers le dossier frontend
$frontendPath = Join-Path $PSScriptRoot "..\..\frontend"
$frontendPath = (Resolve-Path $frontendPath).Path

Write-Host "🚀 Lancement de deux instances du frontend..." -ForegroundColor Green
Write-Host "📁 Dossier : $frontendPath" -ForegroundColor Cyan
Write-Host ""

# Vérifier que le dossier existe
if (-not (Test-Path $frontendPath)) {
    Write-Host "❌ Erreur : Le dossier frontend n'existe pas : $frontendPath" -ForegroundColor Red
    exit 1
}

# Lancer la première instance sur le port 5173
Write-Host "🌐 Instance 1 - Port 5173..." -ForegroundColor Cyan
$cmd1 = "cd '$frontendPath'; Write-Host '=== INSTANCE 1 - PORT 5173 ===' -ForegroundColor Green; npm run dev:port1"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $cmd1

# Attendre un peu avant de lancer la deuxième instance
Start-Sleep -Seconds 3

# Lancer la deuxième instance sur le port 5174
Write-Host "🌐 Instance 2 - Port 5174..." -ForegroundColor Cyan
$cmd2 = "cd '$frontendPath'; Write-Host '=== INSTANCE 2 - PORT 5174 ===' -ForegroundColor Blue; npm run dev:port2"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $cmd2

Write-Host ""
Write-Host "✅ Deux instances lancées !" -ForegroundColor Green
Write-Host "   Instance 1 : http://localhost:5173" -ForegroundColor White
Write-Host "   Instance 2 : http://localhost:5174" -ForegroundColor White
Write-Host ""

# Lancer la première instance en arrière-plan et la garder active
# On utilise Start-Job pour lancer en arrière-plan, mais on garde aussi une fenêtre visible
# Le script se termine mais les instances continuent dans leurs propres fenêtres PowerShell

