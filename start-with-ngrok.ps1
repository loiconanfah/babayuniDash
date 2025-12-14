# Script PowerShell pour lancer ngrok avec plusieurs tunnels
# Compatible avec ngrok 3.x (une instance par tunnel)

param(
    [int]$BackendPort = 5000,
    [int]$FrontendPort1 = 5173,
    [int]$FrontendPort2 = 5174
)

$ErrorActionPreference = "Stop"

Write-Host "🚀 Configuration de ngrok pour l'accès en ligne" -ForegroundColor Cyan
Write-Host ""

# Vérifier si ngrok est installé
$ngrokPath = Get-Command ngrok -ErrorAction SilentlyContinue
if (-not $ngrokPath) {
    Write-Host "❌ ngrok n'est pas installé ou n'est pas dans le PATH" -ForegroundColor Red
    Write-Host "   Téléchargez ngrok depuis: https://ngrok.com/download" -ForegroundColor Yellow
    Write-Host "   Ou installez-le via: choco install ngrok" -ForegroundColor Yellow
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

# Vérifier les ports
Write-Host "🔍 Vérification des services..." -ForegroundColor Cyan
$backendRunning = Test-Port -Port $BackendPort
$frontend1Running = Test-Port -Port $FrontendPort1
$frontend2Running = Test-Port -Port $FrontendPort2

if ($backendRunning) {
    Write-Host "   ✅ Backend en cours sur le port $BackendPort" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Backend non démarré sur le port $BackendPort" -ForegroundColor Yellow
    Write-Host "      Démarrez d'abord le backend avec: dotnet run" -ForegroundColor Yellow
}

if ($frontend1Running) {
    Write-Host "   ✅ Frontend instance 1 en cours sur le port $FrontendPort1" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Frontend instance 1 non démarré sur le port $FrontendPort1" -ForegroundColor Yellow
}

if ($frontend2Running) {
    Write-Host "   ✅ Frontend instance 2 en cours sur le port $FrontendPort2" -ForegroundColor Green
} else {
    Write-Host "   ⚠️  Frontend instance 2 non démarré sur le port $FrontendPort2" -ForegroundColor Yellow
}

Write-Host ""

# Tuer les processus ngrok existants
Get-Process -Name ngrok -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Démarrer ngrok pour chaque service (ngrok 3.x nécessite une instance par tunnel)
Write-Host "🌐 Démarrage des tunnels ngrok..." -ForegroundColor Cyan
Write-Host ""

$ngrokProcesses = @()

# Backend
if ($backendRunning) {
    Write-Host "   🌐 Tunnel backend (port $BackendPort)..." -ForegroundColor Cyan
    $backendNgrok = Start-Process -FilePath "ngrok" -ArgumentList "http $BackendPort" -PassThru -WindowStyle Hidden
    $ngrokProcesses += @{ Name = "Backend"; Port = $BackendPort; Process = $backendNgrok; ApiPort = 4040 }
    Start-Sleep -Seconds 2
}

# Frontend 1
if ($frontend1Running) {
    Write-Host "   🌐 Tunnel frontend 1 (port $FrontendPort1)..." -ForegroundColor Cyan
    $frontend1Ngrok = Start-Process -FilePath "ngrok" -ArgumentList "http $FrontendPort1" -PassThru -WindowStyle Hidden
    $ngrokProcesses += @{ Name = "Frontend 1"; Port = $FrontendPort1; Process = $frontend1Ngrok; ApiPort = 4041 }
    Start-Sleep -Seconds 2
}

# Frontend 2
if ($frontend2Running) {
    Write-Host "   🌐 Tunnel frontend 2 (port $FrontendPort2)..." -ForegroundColor Cyan
    $frontend2Ngrok = Start-Process -FilePath "ngrok" -ArgumentList "http $FrontendPort2" -PassThru -WindowStyle Hidden
    $ngrokProcesses += @{ Name = "Frontend 2"; Port = $FrontendPort2; Process = $frontend2Ngrok; ApiPort = 4042 }
    Start-Sleep -Seconds 2
}

if ($ngrokProcesses.Count -eq 0) {
    Write-Host "❌ Aucun service en cours d'exécution. Démarrez au moins un service avant de lancer ngrok." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "⏳ Attente de la configuration des tunnels (5 secondes)..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Récupérer toutes les URLs ngrok
# Note: Chaque instance ngrok utilise un port API différent (4040, 4041, 4042, etc.)
Write-Host ""
Write-Host "🌍 URLs ngrok disponibles:" -ForegroundColor Green
Write-Host ""

foreach ($service in $ngrokProcesses) {
    try {
        $ngrokApiResponse = Invoke-RestMethod -Uri "http://localhost:$($service.ApiPort)/api/tunnels" -ErrorAction Stop
        
        if ($ngrokApiResponse.tunnels -and $ngrokApiResponse.tunnels.Count -gt 0) {
            $tunnel = $ngrokApiResponse.tunnels[0]
            $url = $tunnel.public_url
            Write-Host "   $($service.Name) (port $($service.Port)):" -ForegroundColor Cyan
            Write-Host "      $url" -ForegroundColor White
            Write-Host "      Dashboard: http://localhost:$($service.ApiPort)" -ForegroundColor Gray
            Write-Host ""
        }
    } catch {
        Write-Host "   $($service.Name): Consultez http://localhost:$($service.ApiPort) pour l'URL" -ForegroundColor Yellow
        Write-Host ""
    }
}

Write-Host "📋 Informations importantes:" -ForegroundColor Yellow
Write-Host "   1. Partagez l'URL du frontend avec vos utilisateurs" -ForegroundColor White
Write-Host "   2. L'URL ngrok change à chaque redémarrage (sauf avec un plan payant)" -ForegroundColor White
Write-Host "   3. Chaque tunnel a son propre dashboard ngrok" -ForegroundColor White
Write-Host "   4. Appuyez sur Ctrl+C pour arrêter tous les tunnels" -ForegroundColor White
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
