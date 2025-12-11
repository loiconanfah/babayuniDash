# Script PowerShell pour lancer l'application en mode multijoueur local
# Lance deux instances du frontend sur des ports différents

Write-Host "🚀 Lancement du mode multijoueur local..." -ForegroundColor Green
Write-Host ""

# Vérifier que nous sommes dans le bon répertoire
if (-not (Test-Path "frontend")) {
    Write-Host "❌ Erreur : Le dossier 'frontend' n'existe pas." -ForegroundColor Red
    Write-Host "   Assurez-vous d'exécuter ce script depuis la racine du projet." -ForegroundColor Yellow
    exit 1
}

# Vérifier que npm est installé
try {
    $null = Get-Command npm -ErrorAction Stop
} catch {
    Write-Host "❌ Erreur : npm n'est pas installé ou n'est pas dans le PATH." -ForegroundColor Red
    exit 1
}

Write-Host "📦 Installation des dépendances (si nécessaire)..." -ForegroundColor Cyan
Set-Location frontend
if (-not (Test-Path "node_modules")) {
    npm install
}
Set-Location ..

Write-Host ""
Write-Host "🌐 Lancement de l'instance 1 sur le port 5173..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\frontend'; Write-Host 'Instance 1 - Port 5173' -ForegroundColor Green; npm run dev:port1"

Start-Sleep -Seconds 3

Write-Host "🌐 Lancement de l'instance 2 sur le port 5174..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\frontend'; Write-Host 'Instance 2 - Port 5174' -ForegroundColor Blue; npm run dev:port2"

Write-Host ""
Write-Host "✅ Deux instances du frontend ont été lancées !" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Instructions :" -ForegroundColor Yellow
Write-Host "   1. Instance 1 : http://localhost:5173" -ForegroundColor White
Write-Host "   2. Instance 2 : http://localhost:5174" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  Assurez-vous que le backend est lancé :" -ForegroundColor Yellow
Write-Host "   cd prisonbreak/prisonbreak.Server" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "🎮 Pour tester le multijoueur :" -ForegroundColor Cyan
Write-Host "   - Connectez-vous avec deux comptes différents" -ForegroundColor White
Write-Host "   - Allez dans 'Jeux' > 'Tic-Tac-Toe'" -ForegroundColor White
Write-Host "   - Sélectionnez un joueur en ligne ou créez une partie publique" -ForegroundColor White
Write-Host ""

