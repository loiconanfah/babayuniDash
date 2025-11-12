# =====================================================
# Script de démarrage pour le projet Hashi
# Lance automatiquement le backend ET le client Vue.js
# =====================================================

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🚀 Démarrage du projet Hashi..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Vérifier si Node.js est installé
Write-Host "🔍 Vérification de Node.js..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js installé: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js n'est pas installé !" -ForegroundColor Red
    Write-Host "Téléchargez-le sur: https://nodejs.org/" -ForegroundColor Yellow
    exit 1
}

# Vérifier si les dépendances npm sont installées
Write-Host ""
Write-Host "🔍 Vérification des dépendances npm..." -ForegroundColor Yellow
$nodeModulesPath = ".\prisonbreak.client\node_modules"
if (-not (Test-Path $nodeModulesPath)) {
    Write-Host "📦 Installation des dépendances npm..." -ForegroundColor Yellow
    Set-Location ".\prisonbreak.client"
    npm install
    Set-Location ".."
    Write-Host "✅ Dépendances installées" -ForegroundColor Green
} else {
    Write-Host "✅ Dépendances déjà installées" -ForegroundColor Green
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "🚀 Lancement des serveurs..." -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Lancer le backend en arrière-plan
Write-Host "📡 Démarrage du backend ASP.NET Core..." -ForegroundColor Cyan
$backendJob = Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$PWD\prisonbreak.Server'; dotnet run --launch-profile https"
) -PassThru

Write-Host "✅ Backend démarré (PID: $($backendJob.Id))" -ForegroundColor Green

# Attendre que le backend soit prêt
Write-Host "⏳ Attente du démarrage du backend (5 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Lancer le client Vue.js en arrière-plan
Write-Host "🎨 Démarrage du client Vue.js (Vite)..." -ForegroundColor Cyan
$clientJob = Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$PWD\prisonbreak.client'; npm run dev"
) -PassThru

Write-Host "✅ Client démarré (PID: $($clientJob.Id))" -ForegroundColor Green

# Attendre que Vite soit prêt
Write-Host "⏳ Attente du démarrage de Vite (5 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "✅ Projet Hashi démarré avec succès !" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📊 Informations importantes:" -ForegroundColor Yellow
Write-Host "   • Backend:     https://localhost:5001" -ForegroundColor White
Write-Host "   • Client:      http://localhost:5173" -ForegroundColor White
Write-Host "   • Swagger:     https://localhost:5001/swagger" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Ouverture du navigateur..." -ForegroundColor Cyan
Start-Sleep -Seconds 2
Start-Process "http://localhost:5173"

Write-Host ""
Write-Host "ℹ️  Deux fenêtres PowerShell sont ouvertes:" -ForegroundColor Yellow
Write-Host "   1. Backend (ASP.NET Core)" -ForegroundColor White
Write-Host "   2. Client (Vue.js/Vite)" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  Pour arrêter les serveurs:" -ForegroundColor Yellow
Write-Host "   Fermez les deux fenêtres PowerShell" -ForegroundColor White
Write-Host "   OU appuyez sur Ctrl+C dans chaque fenêtre" -ForegroundColor White
Write-Host ""
Write-Host "🎉 Bon développement !" -ForegroundColor Green
Write-Host ""

