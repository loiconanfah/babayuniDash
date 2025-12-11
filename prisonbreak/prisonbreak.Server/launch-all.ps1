# Script PowerShell pour lancer le backend puis le frontend dans le bon ordre

$ErrorActionPreference = "Continue"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Lancement du backend et du frontend..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$projectPath = $PSScriptRoot
$frontendPath = Join-Path $projectPath "..\..\frontend"

# Verifier que le dossier frontend existe
if (-not (Test-Path $frontendPath)) {
    Write-Host "ERREUR: Le dossier frontend n'existe pas a $frontendPath" -ForegroundColor Red
    exit 1
}

# Verifier que npm est disponible
try {
    $null = Get-Command npm -ErrorAction Stop
} catch {
    Write-Host "ERREUR: npm n'est pas installe ou n'est pas dans le PATH" -ForegroundColor Red
    exit 1
}

# Fonction pour verifier si un port est utilise
function Test-Port {
    param([int]$Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -ne $connection
}

# Lancer le backend via dotnet run en arriere-plan
Write-Host "Lancement du serveur backend..." -ForegroundColor Yellow
$backendProcess = Start-Process -FilePath "dotnet" -ArgumentList "run", "--project", $projectPath -PassThru -WindowStyle Minimized

# Attendre que le serveur backend soit pret (port 5000)
Write-Host "En attente du serveur backend..." -ForegroundColor Yellow
$maxWait = 30 # secondes
$waited = 0
while (-not (Test-Port -Port 5000) -and $waited -lt $maxWait) {
    Start-Sleep -Seconds 1
    $waited++
    Write-Host "." -NoNewline -ForegroundColor Gray
}

Write-Host ""

if (Test-Port -Port 5000) {
    Write-Host "Serveur backend pret sur le port 5000!" -ForegroundColor Green
} else {
    Write-Host "ATTENTION: Le serveur backend n'est pas pret apres $maxWait secondes" -ForegroundColor Yellow
}

# Attendre encore un peu pour que le serveur soit completement initialise
Start-Sleep -Seconds 2

# Lancer les instances frontend
Write-Host "Lancement des instances frontend..." -ForegroundColor Yellow

Set-Location $frontendPath

# Verifier si les ports sont deja utilises
$port1InUse = Test-Port -Port 5173
$port2InUse = Test-Port -Port 5174

# Lancer l'instance 1 si le port n'est pas utilise
if (-not $port1InUse) {
    Write-Host "Lancement de l'instance 1 sur le port 5173..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k", "cd /d `"$frontendPath`" && npm run dev:port1" -WindowStyle Normal
    Start-Sleep -Seconds 2
} else {
    Write-Host "Port 5173 deja utilise, instance 1 deja lancee" -ForegroundColor Yellow
}

# Lancer l'instance 2 si le port n'est pas utilise
if (-not $port2InUse) {
    Write-Host "Lancement de l'instance 2 sur le port 5174..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k", "cd /d `"$frontendPath`" && npm run dev:port2" -WindowStyle Normal
    Start-Sleep -Seconds 2
} else {
    Write-Host "Port 5174 deja utilise, instance 2 deja lancee" -ForegroundColor Yellow
}

# Attendre que les instances frontend demarrent
Write-Host "En attente des instances frontend..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Ouvrir les navigateurs
Write-Host "Ouverture des navigateurs..." -ForegroundColor Yellow
Start-Process "http://localhost:5173"
Start-Sleep -Seconds 1
Start-Process "http://localhost:5174"

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Tous les services sont lances!" -ForegroundColor Green
Write-Host "  Backend: http://localhost:5000" -ForegroundColor Green
Write-Host "  Frontend Instance 1: http://localhost:5173" -ForegroundColor Green
Write-Host "  Frontend Instance 2: http://localhost:5174" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "Appuyez sur Ctrl+C pour arreter tous les services" -ForegroundColor Yellow

# Attendre que l'utilisateur appuie sur Ctrl+C
try {
    while ($true) {
        Start-Sleep -Seconds 1
    }
} catch {
    Write-Host ""
    Write-Host "Arret de tous les services..." -ForegroundColor Yellow
    Stop-Process -Id $backendProcess.Id -Force -ErrorAction SilentlyContinue
    Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Write-Host "Tous les services ont ete arretes" -ForegroundColor Green
}

