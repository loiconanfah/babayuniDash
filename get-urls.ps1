Start-Sleep -Seconds 5
try {
    $tunnels = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels"
    $frontend = $null
    $backend = $null
    
    foreach ($t in $tunnels.tunnels) {
        if ($t.config.addr -match "5173") {
            $frontend = $t.public_url
        }
        if ($t.config.addr -match "5000") {
            $backend = $t.public_url
        }
    }
    
    Write-Host "Frontend: $frontend"
    Write-Host "Backend: $backend"
    
    if ($backend) {
        Set-Content -Path "frontend\.env" -Value "VITE_API_URL=$backend/api"
        Write-Host "Fichier .env mis a jour avec: $backend/api"
    }
} catch {
    Write-Host "Erreur"
}

