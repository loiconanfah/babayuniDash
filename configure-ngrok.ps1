# Script pour configurer le fichier .env avec l'URL ngrok du backend
# Usage: .\configure-ngrok.ps1

Write-Host "🔧 Configuration du fichier .env pour ngrok" -ForegroundColor Cyan
Write-Host ""

# Demander l'URL ngrok du backend
Write-Host "📋 Étapes:" -ForegroundColor Yellow
Write-Host "   1. Lancez: ngrok start --all" -ForegroundColor Gray
Write-Host "   2. Notez l'URL du backend (ex: https://yyyyy.ngrok-free.app)" -ForegroundColor Gray
Write-Host ""
$backendUrl = Read-Host "Entrez l'URL ngrok du backend (sans /api à la fin)"

if ([string]::IsNullOrWhiteSpace($backendUrl)) {
    Write-Host "❌ URL vide, annulation" -ForegroundColor Red
    exit 1
}

# Nettoyer l'URL (enlever /api si présent)
$backendUrl = $backendUrl.TrimEnd('/')
$backendUrl = $backendUrl.TrimEnd('/api')

# Ajouter /api à la fin
$apiUrl = "$backendUrl/api"

Write-Host ""
Write-Host "📝 Configuration:" -ForegroundColor Yellow
Write-Host "   VITE_API_URL=$apiUrl" -ForegroundColor White
Write-Host ""

# Créer ou mettre à jour le fichier .env
$envContent = "VITE_API_URL=$apiUrl"
Set-Content -Path "frontend\.env" -Value $envContent

Write-Host "✅ Fichier .env mis à jour dans frontend/" -ForegroundColor Green
Write-Host ""
Write-Host "⚠️  IMPORTANT: Redémarrez le serveur frontend pour appliquer les changements" -ForegroundColor Yellow
Write-Host "   Arrêtez (Ctrl+C) et relancez: npm run dev" -ForegroundColor Gray
Write-Host ""

