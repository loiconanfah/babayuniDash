# Script simple pour lancer deux instances du frontend
# À exécuter depuis la racine du projet

$ErrorActionPreference = "Stop"

Write-Host ""
Write-Host "🚀 Lancement du mode multijoueur..." -ForegroundColor Green
Write-Host ""

# Vérifier que nous sommes dans le bon répertoire
if (-not (Test-Path "frontend")) {
    Write-Host "❌ Erreur : Le dossier 'frontend' n'existe pas." -ForegroundColor Red
    Write-Host "   Assurez-vous d'exécuter ce script depuis la racine du projet." -ForegroundColor Yellow
    exit 1
}

# Obtenir le chemin absolu
$rootPath = $PWD.Path
$frontendPath = Join-Path $rootPath "frontend"

Write-Host "📁 Dossier frontend : $frontendPath" -ForegroundColor Cyan
Write-Host ""

# Vérifier que npm est installé
try {
    $npmVersion = npm --version
    Write-Host "✅ npm détecté (version $npmVersion)" -ForegroundColor Green
} catch {
    Write-Host "❌ Erreur : npm n'est pas installé." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "🌐 Lancement de l'instance 1 sur le port 5173..." -ForegroundColor Cyan
$cmd1 = "cd '$frontendPath'; Write-Host '========================================' -ForegroundColor Green; Write-Host '  INSTANCE 1 - PORT 5173' -ForegroundColor Green; Write-Host '========================================' -ForegroundColor Green; Write-Host ''; npm run dev:port1"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $cmd1

Write-Host "   ⏳ Attente de 5 secondes..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host ""
Write-Host "🌐 Lancement de l'instance 2 sur le port 5174..." -ForegroundColor Cyan
$cmd2 = "cd '$frontendPath'; Write-Host '========================================' -ForegroundColor Blue; Write-Host '  INSTANCE 2 - PORT 5174' -ForegroundColor Blue; Write-Host '========================================' -ForegroundColor Blue; Write-Host ''; npm run dev:port2"
Start-Process powershell -ArgumentList "-NoExit", "-Command", $cmd2

Write-Host ""
Write-Host "✅ Deux instances ont été lancées !" -ForegroundColor Green
Write-Host ""
Write-Host "📋 URLs disponibles :" -ForegroundColor Yellow
Write-Host "   🟢 Instance 1 : http://localhost:5173" -ForegroundColor White
Write-Host "   🔵 Instance 2 : http://localhost:5174" -ForegroundColor White
Write-Host ""
Write-Host "💡 Astuce : Ouvrez chaque URL dans un navigateur différent" -ForegroundColor Cyan
Write-Host "   ou utilisez le mode navigation privée pour la deuxième instance" -ForegroundColor Cyan
Write-Host ""
Write-Host "⚠️  N'oubliez pas de lancer le backend :" -ForegroundColor Yellow
Write-Host "   cd prisonbreak\prisonbreak.Server" -ForegroundColor White
Write-Host "   dotnet run" -ForegroundColor White
Write-Host ""


