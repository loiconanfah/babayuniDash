# Script pour démarrer plusieurs tunnels ngrok (une instance par tunnel)
# Compatible avec ngrok 3.x

param(
    [int]$BackendPort = 5000,
    [int]$FrontendPort1 = 5173,
    [int]$FrontendPort2 = 5174
)

Write-Host "🚀 Démarrage de ngrok (plusieurs tunnels)..." -ForegroundColor Cyan
Write-Host ""

# Vérifier si ngrok est installé
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
if (-not $ngrokPath) {
    Write-Host "❌ ngrok n'est pas installé ou n'est pas dans le PATH" -ForegroundColor Red
    Write-Host "   Téléchargez ngrok depuis: https://ngrok.com/download" -ForegroundColor Yellow
    exit 1
}

Write-Host "✅ ngrok trouvé: $($ngrokPath.Source)" -ForegroundColor Green
Write-Host ""

# Vérifier le token ngrok
$ngrokConfigPath = "$env:USERPROFILE\.ngrok2\ngrok.yml"
if (-not (Test-Path $ngrokConfigPath)) {
    Write-Host "⚠️  Configuration ngrok non trouvée" -ForegroundColor Yellow
    $token = Read-Host "Entrez votre token ngrok (trouvé sur https://dashboard.ngrok.com/get-started/your-authtoken)"
    if ($token) {
        ngrok config add-authtoken $token
        Write-Host "✅ Token ngrok configuré" -ForegroundColor Green
    } else {
        Write-Host "❌ Token requis pour continuer" -ForegroundColor Red
        exit 1
    }
}

# Tuer les processus ngrok existants
Get-Process -Name ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Fonction pour tester si un port est utilisé
function Test-Port {
    param([int]$Port)
    try {
        $connection = Test-NetConnection -ComputerName localhost -Port $Port -WarningAction SilentlyContinue -InformationLevel Quiet
        return $connection
    } catch {
        return $false
    }
}

# Vérifier que les services sont en cours d'exécution
Write-Host "🔍 Vérification des services..." -ForegroundColor Cyan
$backendRunning = Test-Port -Port $BackendPort
$frontend1Running = Test-Port -Port $FrontendPort1
$frontend2Running = Test-Port -Port $FrontendPort2

if (-not $backendRunning) {
    Write-Host "⚠️  Backend non détecté sur le port $BackendPort" -ForegroundColor Yellow
    Write-Host "   Démarrez d'abord le backend" -ForegroundColor Yellow
}

if (-not $frontend1Running) {
    Write-Host "⚠️  Frontend instance 1 non détecté sur le port $FrontendPort1" -ForegroundColor Yellow
}

if (-not $frontend2Running) {
    Write-Host "⚠️  Frontend instance 2 non détecté sur le port $FrontendPort2" -ForegroundColor Yellow
}

Write-Host ""

# Démarrer les tunnels ngrok
$processes = @()

if ($backendRunning) {
    Write-Host "🌐 Démarrage du tunnel ngrok pour le backend (port $BackendPort)..." -ForegroundColor Cyan
    $backendNgrok = Start-Process -FilePath "ngrok" -ArgumentList "http $BackendPort" -PassThru -WindowStyle Hidden
    $processes += @{ Name = "Backend"; Process = $backendNgrok; Port = $BackendPort }
    Start-Sleep -Seconds 2
}

if ($frontend1Running) {
    Write-Host "🌐 Démarrage du tunnel ngrok pour le frontend instance 1 (port $FrontendPort1)..." -ForegroundColor Cyan
    $frontend1Ngrok = Start-Process -FilePath "ngrok" -ArgumentList "http $FrontendPort1" -PassThru -WindowStyle Hidden
    $processes += @{ Name = "Frontend 1"; Process = $frontend1Ngrok; Port = $FrontendPort1 }
    Start-Sleep -Seconds 2
}

if ($frontend2Running) {
    Write-Host "🌐 Démarrage du tunnel ngrok pour le frontend instance 2 (port $FrontendPort2)..." -ForegroundColor Cyan
    $frontend2Ngrok = Start-Process -FilePath "ngrok" -ArgumentList "http $FrontendPort2" -PassThru -WindowStyle Hidden
    $processes += @{ Name = "Frontend 2"; Process = $frontend2Ngrok; Port = $FrontendPort2 }
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "⏳ Attente de la configuration des tunnels (5 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Récupérer les URLs ngrok
Write-Host ""
Write-Host "🌍 URLs ngrok disponibles:" -ForegroundColor Green
Write-Host ""

# Note: Avec plusieurs instances de ngrok, chaque instance utilise un port API différent
# Le premier tunnel utilise 4040, les suivants utilisent 4041, 4042, etc.
$apiPorts = @(4040, 4041, 4042)

for ($i = 0; $i -lt $processes.Count; $i++) {
    $apiPort = $apiPorts[$i]
    $process = $processes[$i]
    
    try {
        $ngrokApiResponse = Invoke-RestMethod -Uri "http://localhost:$apiPort/api/tunnels" -ErrorAction Stop
        
        if ($ngrokApiResponse.tunnels -and $ngrokApiResponse.tunnels.Count -gt 0) {
            $tunnel = $ngrokApiResponse.tunnels[0]
            $url = $tunnel.public_url
            
            Write-Host "   $($process.Name) (port $($process.Port)):" -ForegroundColor Cyan
            Write-Host "      $url" -ForegroundColor White
            Write-Host "      Dashboard: http://localhost:$apiPort" -ForegroundColor Gray
            Write-Host ""
        }
    } catch {
        Write-Host "   $($process.Name): Impossible de récupérer l'URL (API port $apiPort)" -ForegroundColor Yellow
        Write-Host "      Consultez le dashboard ngrok sur: http://localhost:$apiPort" -ForegroundColor Gray
        Write-Host ""
    }
}

Write-Host "📋 Informations:" -ForegroundColor Yellow
Write-Host "   • Chaque tunnel ngrok a son propre dashboard" -ForegroundColor White
Write-Host "   • Dashboard principal: http://localhost:4040" -ForegroundColor White
Write-Host "   • Partagez l'URL du frontend avec vos utilisateurs" -ForegroundColor White
Write-Host "   • Appuyez sur Ctrl+C pour arrêter tous les tunnels" -ForegroundColor White
Write-Host ""

# Attendre que l'utilisateur arrête ngrok
Write-Host "Appuyez sur Ctrl+C pour arrêter ngrok..." -ForegroundColor Gray
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} finally {
    Write-Host ""
    Write-Host "🛑 Arrêt de tous les tunnels ngrok..." -ForegroundColor Yellow
    Get-Process -Name ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
    Write-Host "✅ Tous les tunnels ngrok arrêtés" -ForegroundColor Green
}




