# Arreter les sessions existantes
Get-Process ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

Write-Host "Lancement de ngrok (frontend + backend)..." -ForegroundColor Cyan

# Verifier si ngrok.yml existe
if (-not (Test-Path "ngrok.yml")) {
    Write-Host "Fichier ngrok.yml non trouve" -ForegroundColor Red
    exit 1
}

# Lancer ngrok avec tous les tunnels
Start-Process ngrok -ArgumentList "start","--all" -WindowStyle Hidden

# Attendre
Start-Sleep -Seconds 8

# Recuperer les URLs
try {
    $tunnels = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    
    $frontendUrl = $null
    $backendUrl = $null
    
    foreach ($tunnel in $tunnels.tunnels) {
        $addr = $tunnel.config.addr
        if ($addr -match "5173") {
            $frontendUrl = $tunnel.public_url
        }
        if ($addr -match "5000") {
            $backendUrl = $tunnel.public_url
        }
    }
    
    Write-Host ""
    if ($frontendUrl) {
        Write-Host "Frontend:" -ForegroundColor Green
        Write-Host "   $frontendUrl" -ForegroundColor White
    }
    
    if ($backendUrl) {
        Write-Host ""
        Write-Host "Backend:" -ForegroundColor Green
        Write-Host "   $backendUrl" -ForegroundColor White
        Write-Host ""
        Write-Host "Mise a jour du fichier .env..." -ForegroundColor Yellow
        
        # Mettre a jour le .env
        $envContent = "VITE_API_URL=$backendUrl/api"
        Set-Content -Path "frontend\.env" -Value $envContent
        
        Write-Host "Fichier .env mis a jour" -ForegroundColor Green
        Write-Host ""
        Write-Host "Redemarrez le frontend pour appliquer les changements" -ForegroundColor Yellow
    } else {
        Write-Host "Backend non trouve" -ForegroundColor Red
    }
    
} catch {
    Write-Host "Erreur: ngrok non accessible" -ForegroundColor Red
}
