# Script PowerShell pour lancer les instances frontend apres que le serveur backend soit pret

$ErrorActionPreference = "Continue"

$projectPath = $PSScriptRoot
$frontendPath = Join-Path $projectPath "..\..\frontend"
$logFile = Join-Path $env:TEMP "vite-multi-launch.log"

"=== Demarrage du script $(Get-Date) ===" | Out-File -FilePath $logFile -Append
"Chemin frontend: $frontendPath" | Out-File -FilePath $logFile -Append

# Fonction pour verifier si un port est utilise
function Test-Port {
    param([int]$Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -ne $connection
}

# Attendre que le serveur backend soit pret (ports 5000 ou 5001)
Write-Host "En attente du serveur backend..." -ForegroundColor Yellow
$maxWait = 60 # secondes (augmente pour permettre les migrations)
$waited = 0
$backendReady = $false

while (-not $backendReady -and $waited -lt $maxWait) {
    $port5000 = Test-Port -Port 5000
    $port5001 = Test-Port -Port 5001
    
    if ($port5000 -or $port5001) {
        $backendReady = $true
        $portUsed = if ($port5000) { "5000" } else { "5001" }
        "Serveur backend detecte sur le port $portUsed apres $waited secondes" | Out-File -FilePath $logFile -Append
        Write-Host "Serveur backend pret sur le port $portUsed apres $waited secondes!" -ForegroundColor Green
        break
    }
    
    Start-Sleep -Seconds 1
    $waited++
    if ($waited % 5 -eq 0) {
        Write-Host "  En attente... ($waited/$maxWait secondes)" -ForegroundColor Gray
        "En attente du serveur backend... $waited/$maxWait secondes" | Out-File -FilePath $logFile -Append
    }
}

if (-not $backendReady) {
    "ATTENTION: Le serveur backend n'est pas pret apres $maxWait secondes" | Out-File -FilePath $logFile -Append
    Write-Host "ATTENTION: Le serveur backend n'est pas pret apres $maxWait secondes" -ForegroundColor Yellow
    Write-Host "Verification des ports 5000 et 5001..." -ForegroundColor Yellow
    Write-Host "  Port 5000: $(if (Test-Port -Port 5000) { 'OUI' } else { 'NON' })" -ForegroundColor Gray
    Write-Host "  Port 5001: $(if (Test-Port -Port 5001) { 'OUI' } else { 'NON' })" -ForegroundColor Gray
    Write-Host "Lancement des instances frontend quand meme..." -ForegroundColor Yellow
}

# Attendre encore un peu pour que le serveur soit completement initialise
Start-Sleep -Seconds 2

# Verifier que le dossier frontend existe
if (-not (Test-Path $frontendPath)) {
    $errorMsg = "ERREUR: Le dossier frontend n'existe pas a $frontendPath"
    Write-Host $errorMsg -ForegroundColor Red
    $errorMsg | Out-File -FilePath $logFile -Append
    exit 1
}

Set-Location $frontendPath

# Verifier que npm est disponible
try {
    $null = Get-Command npm -ErrorAction Stop
} catch {
    $errorMsg = "ERREUR: npm n'est pas installe ou n'est pas dans le PATH"
    Write-Host $errorMsg -ForegroundColor Red
    $errorMsg | Out-File -FilePath $logFile -Append
    exit 1
}

# Verifier si les ports sont deja utilises
$port1InUse = Test-Port -Port 5173
$port2InUse = Test-Port -Port 5174

"Port 5173 utilise: $port1InUse" | Out-File -FilePath $logFile -Append
"Port 5174 utilise: $port2InUse" | Out-File -FilePath $logFile -Append

# Lancer l'instance 1 si le port n'est pas utilise
if (-not $port1InUse) {
    Write-Host "Lancement de l'instance 1 sur le port 5173..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k", "cd /d `"$frontendPath`" && npm run dev:port1" -WindowStyle Normal
    "Instance 1 lancee $(Get-Date)" | Out-File -FilePath $logFile -Append
    Start-Sleep -Seconds 2
} else {
    "Port 5173 deja utilise, instance 1 deja lancee" | Out-File -FilePath $logFile -Append
}

# Lancer l'instance 2 si le port n'est pas utilise
if (-not $port2InUse) {
    Write-Host "Lancement de l'instance 2 sur le port 5174..." -ForegroundColor Yellow
    Start-Process -FilePath "cmd" -ArgumentList "/k", "cd /d `"$frontendPath`" && npm run dev:port2" -WindowStyle Normal
    "Instance 2 lancee $(Get-Date)" | Out-File -FilePath $logFile -Append
    Start-Sleep -Seconds 2
} else {
    "Port 5174 deja utilise, instance 2 deja lancee" | Out-File -FilePath $logFile -Append
}

# Attendre que les instances frontend demarrent
Start-Sleep -Seconds 3

# Ouvrir les navigateurs
Write-Host "Ouverture des navigateurs..." -ForegroundColor Yellow
Start-Process "http://localhost:5173"
Start-Sleep -Seconds 1
Start-Process "http://localhost:5174"

"Les instances du frontend sont pretes!" | Out-File -FilePath $logFile -Append
"  Instance 1 : http://localhost:5173" | Out-File -FilePath $logFile -Append
"  Instance 2 : http://localhost:5174" | Out-File -FilePath $logFile -Append

Write-Host "Instances frontend lancees avec succes!" -ForegroundColor Green
Write-Host "  Instance 1 : http://localhost:5173" -ForegroundColor Green
Write-Host "  Instance 2 : http://localhost:5174" -ForegroundColor Green

