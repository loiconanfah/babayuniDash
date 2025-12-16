# Script simplifié pour démarrer ngrok
# Utilise le fichier de configuration ngrok-config.yml

Write-Host "🚀 Démarrage de ngrok..." -ForegroundColor Cyan
Write-Host ""

# Vérifier si ngrok est installé
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
if (-not $ngrokPath) {
    Write-Host "❌ ngrok n'est pas installé ou n'est pas dans le PATH" -ForegroundColor Red
    Write-Host "   Téléchargez ngrok depuis: https://ngrok.com/download" -ForegroundColor Yellow
    exit 1
}

# Vérifier si le fichier de configuration existe
$configFile = Join-Path $PSScriptRoot "ngrok-config.yml"
if (-not (Test-Path $configFile)) {
    Write-Host "❌ Fichier de configuration introuvable: $configFile" -ForegroundColor Red
    Write-Host "   Créez le fichier ngrok-config.yml avec votre configuration" -ForegroundColor Yellow
    exit 1
}

# Vérifier si le token est configuré
$configContent = Get-Content $configFile -Raw
if ($configContent -match "YOUR_AUTH_TOKEN") {
    Write-Host "⚠️  Token ngrok non configuré dans ngrok-config.yml" -ForegroundColor Yellow
    $token = Read-Host "Entrez votre token ngrok (trouvé sur https://dashboard.ngrok.com/get-started/your-authtoken)"
    if ($token) {
        (Get-Content $configFile) -replace "YOUR_AUTH_TOKEN", $token | Set-Content $configFile
        Write-Host "✅ Token configuré dans ngrok-config.yml" -ForegroundColor Green
    } else {
        Write-Host "❌ Token requis pour continuer" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ Configuration trouvée: $configFile" -ForegroundColor Green
Write-Host ""

# Tuer les processus ngrok existants
Get-Process -Name ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Démarrer ngrok avec la configuration
Write-Host "🌐 Démarrage de ngrok..." -ForegroundColor Cyan
Write-Host "⚠️  Note: ngrok 3.x ne supporte pas plusieurs tunnels avec un seul fichier de config" -ForegroundColor Yellow
Write-Host "   Utilisez start-ngrok-multiple.ps1 pour démarrer plusieurs tunnels" -ForegroundColor Yellow
Write-Host ""

# Pour ngrok 3.x, on démarre un seul tunnel (backend par défaut)
Write-Host "🌐 Démarrage du tunnel ngrok pour le backend (port 5000)..." -ForegroundColor Cyan
$ngrokProcess = Start-Process -FilePath "ngrok" -ArgumentList "http 5000" -PassThru -NoNewWindow

Start-Sleep -Seconds 5

# Récupérer les URLs ngrok
try {
    $ngrokApiResponse = Invoke-RestMethod -Uri "http://localhost:4040/api/tunnels" -ErrorAction Stop
    
    Write-Host ""
    Write-Host "🌍 URLs ngrok disponibles:" -ForegroundColor Green
    Write-Host ""
    
    foreach ($tunnel in $ngrokApiResponse.tunnels) {
        $name = $tunnel.name
        $url = $tunnel.public_url
        $addr = $tunnel.config.addr
        
        # Déterminer le type de service
        $serviceType = switch ($addr) {
            "5000" { "Backend API" }
            "5173" { "Frontend Instance 1" }
            "5174" { "Frontend Instance 2" }
            default { "Service" }
        }
        
        Write-Host "   $serviceType ($name):" -ForegroundColor Cyan
        Write-Host "      $url" -ForegroundColor White
        Write-Host ""
    }
    
    Write-Host "📋 Informations:" -ForegroundColor Yellow
    Write-Host "   • Dashboard ngrok: http://localhost:4040" -ForegroundColor White
    Write-Host "   • Partagez l'URL du frontend avec vos utilisateurs" -ForegroundColor White
    Write-Host "   • Appuyez sur Ctrl+C pour arrêter ngrok" -ForegroundColor White
    Write-Host ""
    
} catch {
    Write-Host "⚠️  Impossible de récupérer les URLs ngrok automatiquement" -ForegroundColor Yellow
    Write-Host "   Consultez le dashboard ngrok sur: http://localhost:4040" -ForegroundColor Yellow
    Write-Host ""
}

# Attendre que l'utilisateur arrête ngrok
Write-Host "Appuyez sur Ctrl+C pour arrêter ngrok..." -ForegroundColor Gray
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} finally {
    Write-Host ""
    Write-Host "🛑 Arrêt de ngrok..." -ForegroundColor Yellow
    Get-Process -Name ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "✅ ngrok arrêté" -ForegroundColor Green
}

