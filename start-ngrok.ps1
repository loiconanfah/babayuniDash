# Script PowerShell pour lancer ngrok
# Usage: .\start-ngrok.ps1 [port]
# Par défaut: port 5173 (frontend)

param(
    [int]$Port = 5173
)

Write-Host "🚀 Lancement de ngrok pour le port $Port..." -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Instructions:" -ForegroundColor Yellow
Write-Host "   1. Assurez-vous que le backend est lancé sur http://localhost:5000" -ForegroundColor Gray
Write-Host "   2. Assurez-vous que le frontend est lancé sur http://localhost:5173" -ForegroundColor Gray
Write-Host "   3. Une fois ngrok lancé, copiez l'URL HTTPS générée" -ForegroundColor Gray
Write-Host "   4. Ouvrez cette URL dans votre navigateur" -ForegroundColor Gray
Write-Host ""
Write-Host "🌐 Interface web ngrok: http://localhost:4040" -ForegroundColor Green
Write-Host ""

# Vérifier si ngrok est installé
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
if (-not $ngrokPath) {
    Write-Host "❌ Erreur: ngrok n'est pas installé ou n'est pas dans le PATH" -ForegroundColor Red
    Write-Host "   Téléchargez ngrok depuis: https://ngrok.com/download" -ForegroundColor Yellow
    exit 1
}

# Lancer ngrok
Write-Host "✅ Lancement de ngrok..." -ForegroundColor Green
ngrok http $Port



