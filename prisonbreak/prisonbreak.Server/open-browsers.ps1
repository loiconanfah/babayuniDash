# Script PowerShell pour ouvrir les deux fenêtres du navigateur
# S'exécute après le démarrage des serveurs

# Fonction pour vérifier si un port est utilisé
function Test-Port {
    param([int]$Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -ne $connection
}

# Attendre que les serveurs soient prêts (maximum 30 secondes)
$maxAttempts = 15
$attempts = 0

Write-Host "⏳ Attente du démarrage des serveurs frontend..." -ForegroundColor Yellow

# Vérifier le port 5173
while (-not (Test-Port -Port 5173)) {
    $attempts++
    if ($attempts -gt $maxAttempts) {
        Write-Host "⚠️  Timeout : Le serveur sur le port 5173 n'a pas démarré" -ForegroundColor Yellow
        break
    }
    Start-Sleep -Seconds 2
}

$attempts = 0
# Vérifier le port 5174
while (-not (Test-Port -Port 5174)) {
    $attempts++
    if ($attempts -gt $maxAttempts) {
        Write-Host "⚠️  Timeout : Le serveur sur le port 5174 n'a pas démarré" -ForegroundColor Yellow
        break
    }
    Start-Sleep -Seconds 2
}

# Ouvrir les deux fenêtres du navigateur (même si Vite les ouvre déjà, on s'assure qu'elles sont ouvertes)
Write-Host "🌐 Vérification et ouverture des fenêtres du navigateur..." -ForegroundColor Cyan

# Attendre un peu plus pour que Vite ouvre les navigateurs
Start-Sleep -Seconds 3

# Ouvrir les deux fenêtres du navigateur (au cas où Vite ne les aurait pas ouvertes)
Start-Process "http://localhost:5173"
Start-Sleep -Seconds 1
Start-Process "http://localhost:5174"

Write-Host "✅ Les deux fenêtres ont été ouvertes !" -ForegroundColor Green
Write-Host "   Instance 1 : http://localhost:5173" -ForegroundColor Cyan
Write-Host "   Instance 2 : http://localhost:5174" -ForegroundColor Cyan

