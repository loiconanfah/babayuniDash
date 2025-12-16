# Arrêter les sessions existantes
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Lancer ngrok
Start-Process ngrok -ArgumentList "http","5173" -WindowStyle Hidden

# Attendre
Start-Sleep -Seconds 6

# Récupérer l'URL
try {
    $tunnels = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    foreach ($tunnel in $tunnels.tunnels) {
        if ($tunnel.config.addr -eq "localhost:5173") {
            Write-Host $tunnel.public_url
            exit 0
        }
    }
} catch {
    Write-Host "Erreur: ngrok non accessible"
}

