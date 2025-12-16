# Script PowerShell pour arrêter toutes les sessions ngrok
# Usage: .\stop-ngrok.ps1

Write-Host "🛑 Arrêt de toutes les sessions ngrok..." -ForegroundColor Yellow
Write-Host ""

$processes = Get-Process ngrok -ErrorAction SilentlyContinue

if ($processes) {
    $count = $processes.Count
    Write-Host "   Trouvé $count processus ngrok actif(s)" -ForegroundColor Gray
    
    foreach ($process in $processes) {
        Write-Host "   Arrêt du processus PID $($process.Id)..." -ForegroundColor Gray
        Stop-Process -Id $process.Id -Force -ErrorAction SilentlyContinue
    }
    
    Start-Sleep -Milliseconds 500
    
    # Vérifier que tout est arrêté
    $remaining = Get-Process ngrok -ErrorAction SilentlyContinue
    if (-not $remaining) {
        Write-Host ""
        Write-Host "✅ Toutes les sessions ngrok ont été arrêtées" -ForegroundColor Green
    } else {
        Write-Host ""
        Write-Host "⚠️  Certains processus n'ont pas pu être arrêtés" -ForegroundColor Yellow
        Write-Host "   Essayez de les arrêter manuellement depuis le dashboard:" -ForegroundColor Gray
        Write-Host "   https://dashboard.ngrok.com/agents" -ForegroundColor Cyan
    }
} else {
    Write-Host "ℹ️  Aucune session ngrok active" -ForegroundColor Gray
}

Write-Host ""
Write-Host "💡 Vous pouvez maintenant relancer ngrok avec:" -ForegroundColor Cyan
Write-Host "   ngrok http 5173" -ForegroundColor White
Write-Host ""



