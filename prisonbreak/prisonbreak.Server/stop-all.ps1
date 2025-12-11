# Script PowerShell pour arreter tous les builds et services en cours

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Arret de tous les builds et services..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Arreter tous les processus dotnet (backend)
Write-Host "Arret des processus dotnet..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Arreter tous les processus node (Vite)
Write-Host "Arret des processus node..." -ForegroundColor Yellow
Get-Process -Name "node" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Arreter tous les processus msbuild (builds Visual Studio)
Write-Host "Arret des processus msbuild..." -ForegroundColor Yellow
Get-Process -Name "msbuild" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# Arreter les processus Vite sur les ports specifiques
Write-Host "Arret des processus sur les ports 5173 et 5174..." -ForegroundColor Yellow
$ports = @(5173, 5174, 5000, 5001)
foreach ($port in $ports) {
    $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
    foreach ($conn in $connections) {
        if ($conn.OwningProcess) {
            Write-Host "Arret du processus sur le port $port (PID: $($conn.OwningProcess))" -ForegroundColor Yellow
            Stop-Process -Id $conn.OwningProcess -Force -ErrorAction SilentlyContinue
        }
    }
}

Write-Host ""
Write-Host "Tous les builds et services ont ete arretes!" -ForegroundColor Green
Write-Host ""

