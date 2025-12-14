# Script pour lancer deux instances du frontend en mode multijoueur
# À exécuter depuis le dossier frontend

Write-Host "🚀 Lancement du mode multijoueur..." -ForegroundColor Green
Write-Host ""

# Vérifier que npm est installé
try {
    $null = Get-Command npm -ErrorAction Stop
} catch {
    Write-Host "❌ Erreur : npm n'est pas installé." -ForegroundColor Red
    exit 1
}

# Vérifier que les ports sont libres
$port1 = 5173
$port2 = 5174

function Test-Port {
    param([int]$Port)
    try {
        $listener = [System.Net.Sockets.TcpListener]::new([System.Net.IPAddress]::Any, $Port)
        $listener.Start()
        $listener.Stop()
        return $true
    } catch {
        return $false
    }
}

if (-not (Test-Port -Port $port1)) {
    Write-Host "⚠️  Le port $port1 est déjà utilisé. Arrêtez l'application qui l'utilise." -ForegroundColor Yellow
}

if (-not (Test-Port -Port $port2)) {
    Write-Host "⚠️  Le port $port2 est déjà utilisé. Arrêtez l'application qui l'utilise." -ForegroundColor Yellow
}

Write-Host "🌐 Lancement de l'instance 1 sur le port $port1..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; Write-Host '=== Instance 1 - Port $port1 ===' -ForegroundColor Green; Write-Host ''; npm run dev:port1"

Start-Sleep -Seconds 3

Write-Host "🌐 Lancement de l'instance 2 sur le port $port2..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD'; Write-Host '=== Instance 2 - Port $port2 ===' -ForegroundColor Blue; Write-Host ''; npm run dev:port2"

Write-Host ""
Write-Host "✅ Deux instances ont été lancées !" -ForegroundColor Green
Write-Host ""
Write-Host "📋 URLs :" -ForegroundColor Yellow
Write-Host "   Instance 1 : http://localhost:$port1" -ForegroundColor White
Write-Host "   Instance 2 : http://localhost:$port2" -ForegroundColor White
Write-Host ""
Write-Host "💡 Astuce : Ouvrez chaque URL dans un navigateur différent ou en mode navigation privée" -ForegroundColor Cyan
Write-Host ""



