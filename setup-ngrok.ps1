# Script pour configurer ngrok avec frontend + backend
# Usage: .\setup-ngrok.ps1

Write-Host "🚀 Configuration ngrok pour frontend + backend" -ForegroundColor Cyan
Write-Host ""

# Vérifier si ngrok.yml existe
if (-not (Test-Path "ngrok.yml")) {
    Write-Host "❌ Fichier ngrok.yml non trouvé" -ForegroundColor Red
    Write-Host "   Le fichier ngrok.yml doit être dans le dossier racine du projet" -ForegroundColor Yellow
    exit 1
}

# Demander le token ngrok si pas présent
$config = Get-Content "ngrok.yml" -Raw
if ($config -match "authtoken:\s*#") {
    Write-Host "⚠️  Token ngrok manquant dans ngrok.yml" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "1. Récupérez votre token depuis: https://dashboard.ngrok.com/get-started/your-authtoken" -ForegroundColor Cyan
    Write-Host "2. Éditez ngrok.yml et remplacez la ligne 'authtoken: # ...' par 'authtoken: VOTRE_TOKEN'" -ForegroundColor Cyan
    Write-Host ""
    $continue = Read-Host "Appuyez sur Entrée après avoir ajouté le token"
}

Write-Host ""
Write-Host "📋 Étapes suivantes:" -ForegroundColor Yellow
Write-Host "   1. Assurez-vous que le backend est lancé sur http://localhost:5000" -ForegroundColor Gray
Write-Host "   2. Assurez-vous que le frontend est lancé sur http://localhost:5173" -ForegroundColor Gray
Write-Host "   3. Lancez: ngrok start --all" -ForegroundColor Green
Write-Host ""
Write-Host "💡 Après le lancement, notez les URLs ngrok:" -ForegroundColor Cyan
Write-Host "   - Frontend: https://xxxxx.ngrok-free.app" -ForegroundColor White
Write-Host "   - Backend:  https://yyyyy.ngrok-free.app" -ForegroundColor White
Write-Host ""
Write-Host "   4. Créez un fichier .env dans frontend/ avec:" -ForegroundColor Gray
Write-Host "      VITE_API_URL=https://yyyyy.ngrok-free.app/api" -ForegroundColor White
Write-Host ""
Write-Host "   5. Redémarrez le frontend" -ForegroundColor Gray
Write-Host ""

