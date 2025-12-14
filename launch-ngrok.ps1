# Arrêter les sessions existantes
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

Write-Host "🚀 Lancement de ngrok..." -ForegroundColor Cyan

# Lancer ngrok
Start-Process ngrok -ArgumentList "http","5173" -WindowStyle Hidden

# Attendre que ngrok démarre
Start-Sleep -Seconds 8

# Récupérer l'URL
try {
    $tunnels = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    foreach ($tunnel in $tunnels.tunnels) {
        $addr = $tunnel.config.addr
        if ($addr -match "5173") {
            Write-Host ""
            Write-Host "✅ URL ngrok:" -ForegroundColor Green
            Write-Host $tunnel.public_url -ForegroundColor White
            Write-Host ""
            exit 0
        }
    }
    Write-Host "❌ Tunnel non trouve" -ForegroundColor Red
} catch {
    Write-Host "❌ Erreur: ngrok non accessible" -ForegroundColor Red
    Write-Host "   Lancez manuellement: ngrok http 5173" -ForegroundColor Yellow
}



