# Script pour lancer ngrok et récupérer l'URL
Write-Host "🚀 Lancement de ngrok..." -ForegroundColor Cyan

# Arrêter les sessions existantes
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Lancer ngrok en arrière-plan
Start-Process ngrok -ArgumentList "http","5173" -WindowStyle Hidden

# Attendre que ngrok démarre
Write-Host "⏳ Attente du démarrage de ngrok..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Récupérer l'URL
try {
    $tunnels = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    $frontendUrl = ($tunnels.tunnels | Where-Object { $_.config.addr -eq "localhost:5173" }).public_url
    
    if ($frontendUrl) {
        Write-Host ""
        Write-Host "✅ URL ngrok frontend:" -ForegroundColor Green
        Write-Host $frontendUrl -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host "❌ URL non trouvée" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Erreur: ngrok n'est pas accessible" -ForegroundColor Red
    Write-Host "   Vérifiez que ngrok est bien lancé" -ForegroundColor Yellow
}

