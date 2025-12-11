# Script de test pour vérifier que les deux ports fonctionnent
$frontendPath = Join-Path $PSScriptRoot "..\..\frontend"

Write-Host "🧪 Test des ports frontend..." -ForegroundColor Cyan
Write-Host "Chemin frontend: $frontendPath" -ForegroundColor Gray

Set-Location $frontendPath

# Test du port 5173
Write-Host "`n📌 Test du port 5173..." -ForegroundColor Yellow
$job1 = Start-Job -ScriptBlock {
    Set-Location $using:frontendPath
    npm run dev:port1
}

Start-Sleep -Seconds 8

$port1Check = Get-NetTCPConnection -LocalPort 5173 -ErrorAction SilentlyContinue
if ($port1Check) {
    Write-Host "✅ Port 5173 : ACTIF" -ForegroundColor Green
} else {
    Write-Host "❌ Port 5173 : INACTIF" -ForegroundColor Red
}

Stop-Job $job1
Remove-Job $job1

Start-Sleep -Seconds 2

# Test du port 5174
Write-Host "`n📌 Test du port 5174..." -ForegroundColor Yellow
$job2 = Start-Job -ScriptBlock {
    Set-Location $using:frontendPath
    npm run dev:port2
}

Start-Sleep -Seconds 8

$port2Check = Get-NetTCPConnection -LocalPort 5174 -ErrorAction SilentlyContinue
if ($port2Check) {
    Write-Host "✅ Port 5174 : ACTIF" -ForegroundColor Green
} else {
    Write-Host "❌ Port 5174 : INACTIF" -ForegroundColor Red
}

Stop-Job $job2
Remove-Job $job2

Write-Host "`n✅ Test terminé" -ForegroundColor Cyan

