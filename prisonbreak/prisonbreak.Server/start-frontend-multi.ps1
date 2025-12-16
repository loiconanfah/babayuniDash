# Script PowerShell pour lancer les deux instances du frontend en parallele
# Ce script est execute automatiquement par Visual Studio au demarrage

# Rediriger la sortie vers un fichier de log pour le diagnostic
$logFile = Join-Path $env:TEMP "vite-multi-launch.log"
"=== Demarrage du script $(Get-Date) ===" | Out-File -FilePath $logFile -Append
"PSScriptRoot: $PSScriptRoot" | Out-File -FilePath $logFile -Append
"Working Directory: $(Get-Location)" | Out-File -FilePath $logFile -Append

# Afficher un message visible dans Visual Studio
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Script de lancement frontend demarre" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$frontendPath = Join-Path $PSScriptRoot "..\..\frontend"
"Chemin frontend: $frontendPath" | Out-File -FilePath $logFile -Append

if (-not (Test-Path $frontendPath)) {
    $errorMsg = "Erreur : Le dossier frontend n'existe pas a $frontendPath"
    Write-Host $errorMsg -ForegroundColor Red
    $errorMsg | Out-File -FilePath $logFile -Append
    exit 1
}

Set-Location $frontendPath

# Verifier si npm est disponible
try {
    $null = Get-Command npm -ErrorAction Stop
    "npm trouve" | Out-File -FilePath $logFile -Append
} catch {
    $errorMsg = "Erreur : npm n'est pas installe ou n'est pas dans le PATH"
    Write-Host $errorMsg -ForegroundColor Red
    $errorMsg | Out-File -FilePath $logFile -Append
    exit 1
}

# Fonction pour verifier si un port est utilise
function Test-Port {
    param([int]$Port)
    $connection = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue
    return $null -ne $connection
}

# Verifier si les instances sont deja en cours d'execution
$port1InUse = Test-Port -Port 5173
$port2InUse = Test-Port -Port 5174

"Port 5173 utilise: $port1InUse" | Out-File -FilePath $logFile -Append
"Port 5174 utilise: $port2InUse" | Out-File -FilePath $logFile -Append

if ($port1InUse -and $port2InUse) {
    Write-Host "Les deux instances du frontend sont deja en cours d'execution" -ForegroundColor Yellow
    Write-Host "   Instance 1 : http://localhost:5173" -ForegroundColor Cyan
    Write-Host "   Instance 2 : http://localhost:5174" -ForegroundColor Cyan
    exit 0
}

# Verifier si les dependances sont installees
if (-not (Test-Path "node_modules")) {
    Write-Host "Installation des dependances..." -ForegroundColor Cyan
    npm install
}

# Lancer les instances manquantes dans des fenetres PowerShell separees
# Creer des fichiers de script temporaires pour eviter les problemes d'echappement
$script1Path = Join-Path $env:TEMP "vite-instance-1.ps1"
$script2Path = Join-Path $env:TEMP "vite-instance-2.ps1"

$script1Content = @"
Set-Location '$frontendPath'
Write-Host 'Frontend Instance 1 - Port 5173' -ForegroundColor Green
npm run dev:port1
"@

$script2Content = @"
Set-Location '$frontendPath'
Write-Host 'Frontend Instance 2 - Port 5174' -ForegroundColor Blue
npm run dev:port2
"@

# Ecrire les scripts temporaires
$script1Content | Out-File -FilePath $script1Path -Encoding UTF8 -Force
$script2Content | Out-File -FilePath $script2Path -Encoding UTF8 -Force

"Scripts temporaires crees:" | Out-File -FilePath $logFile -Append
"  Instance 1: $script1Path" | Out-File -FilePath $logFile -Append
"  Instance 2: $script2Path" | Out-File -FilePath $logFile -Append

# Lancer les deux instances - FORCER le lancement des deux
$processes = @()

# Lancer l'instance 1
if (-not $port1InUse) {
    Write-Host "Lancement de l'instance 1 sur le port 5173..." -ForegroundColor Green
    "Lancement instance 1..." | Out-File -FilePath $logFile -Append
    try {
        $argList1 = @("-NoExit", "-ExecutionPolicy", "Bypass", "-File", $script1Path)
        $process1 = Start-Process powershell.exe -ArgumentList $argList1 -WindowStyle Normal -PassThru -ErrorAction Stop
        if ($process1) {
            Write-Host "   Processus lance (PID: $($process1.Id))" -ForegroundColor Gray
            "Instance 1 lancee (PID: $($process1.Id))" | Out-File -FilePath $logFile -Append
            $processes += $process1
        } else {
            "ERREUR: Processus 1 retourne null" | Out-File -FilePath $logFile -Append
        }
    } catch {
        $errorMsg = "Erreur lors du lancement de l'instance 1 : $_"
        Write-Host $errorMsg -ForegroundColor Red
        Write-Host "   Details: $($_.Exception.Message)" -ForegroundColor Red
        $errorMsg | Out-File -FilePath $logFile -Append
        $_.Exception | Out-File -FilePath $logFile -Append
    }
} else {
    Write-Host "Instance 1 deja en cours sur le port 5173" -ForegroundColor Yellow
    "Instance 1 deja en cours" | Out-File -FilePath $logFile -Append
}

# Attendre avant de lancer la deuxieme instance
Write-Host "Attente de 3 secondes avant le lancement de l'instance 2..." -ForegroundColor Gray
Start-Sleep -Seconds 3

# Lancer l'instance 2 - TOUJOURS essayer de la lancer (meme si le port semble utilise)
Write-Host "Lancement de l'instance 2 sur le port 5174..." -ForegroundColor Green
"Lancement instance 2..." | Out-File -FilePath $logFile -Append
"Port 5174 utilise avant lancement: $port2InUse" | Out-File -FilePath $logFile -Append

# Verifier a nouveau le port juste avant le lancement
$port2Check = Test-Port -Port 5174
"Port 5174 verifie juste avant lancement: $port2Check" | Out-File -FilePath $logFile -Append

# Lancer l'instance 2 meme si le port semble utilise (Vite gerera l'erreur si necessaire)
try {
    $argList2 = @("-NoExit", "-ExecutionPolicy", "Bypass", "-File", $script2Path)
    "Arguments pour instance 2: $($argList2 -join ' ')" | Out-File -FilePath $logFile -Append
    "Chemin script 2: $script2Path" | Out-File -FilePath $logFile -Append
    "Script 2 existe: $(Test-Path $script2Path)" | Out-File -FilePath $logFile -Append
    
    $process2 = Start-Process powershell.exe -ArgumentList $argList2 -WindowStyle Normal -PassThru -ErrorAction Stop
    if ($process2) {
        Write-Host "   Processus lance (PID: $($process2.Id))" -ForegroundColor Gray
        "Instance 2 lancee (PID: $($process2.Id))" | Out-File -FilePath $logFile -Append
        $processes += $process2
        
        # Verifier que le processus est toujours actif apres 3 secondes
        Start-Sleep -Seconds 3
        $process2StillRunning = $false
        try {
            $null = Get-Process -Id $process2.Id -ErrorAction Stop
            $process2StillRunning = $true
            Write-Host "   Processus 2 toujours actif" -ForegroundColor Green
        } catch {
            $process2StillRunning = $false
            Write-Host "   ATTENTION: Processus 2 s'est arrete!" -ForegroundColor Yellow
        }
        "Processus 2 toujours actif apres 3 secondes: $process2StillRunning" | Out-File -FilePath $logFile -Append
    } else {
        "ERREUR: Processus 2 retourne null" | Out-File -FilePath $logFile -Append
        Write-Host "   ERREUR: Le processus 2 n'a pas ete cree" -ForegroundColor Red
    }
} catch {
    $errorMsg = "Erreur lors du lancement de l'instance 2 : $_"
    Write-Host $errorMsg -ForegroundColor Red
    Write-Host "   Details: $($_.Exception.Message)" -ForegroundColor Red
    $errorMsg | Out-File -FilePath $logFile -Append
    $_.Exception | Out-File -FilePath $logFile -Append
    $_.Exception.StackTrace | Out-File -FilePath $logFile -Append
}

Start-Sleep -Seconds 2

Write-Host "Nombre de processus lances: $($processes.Count)" -ForegroundColor Gray
"Nombre de processus lances: $($processes.Count)" | Out-File -FilePath $logFile -Append

# Verifier que les ports sont bien utilises apres le lancement
Start-Sleep -Seconds 5
$port1Final = Test-Port -Port 5173
$port2Final = Test-Port -Port 5174

Write-Host ""
Write-Host "Etat des ports apres lancement:" -ForegroundColor Cyan
if ($port1Final) {
    Write-Host "   Port 5173 : ACTIF" -ForegroundColor Green
} else {
    Write-Host "   Port 5173 : INACTIF" -ForegroundColor Red
}

if ($port2Final) {
    Write-Host "   Port 5174 : ACTIF" -ForegroundColor Green
} else {
    Write-Host "   Port 5174 : INACTIF" -ForegroundColor Red
}

"Port 5173 final: $port1Final" | Out-File -FilePath $logFile -Append
"Port 5174 final: $port2Final" | Out-File -FilePath $logFile -Append

Write-Host ""
Write-Host "Les instances du frontend sont pretes !" -ForegroundColor Green
Write-Host "   Instance 1 : http://localhost:5173" -ForegroundColor Cyan
Write-Host "   Instance 2 : http://localhost:5174" -ForegroundColor Cyan
Write-Host ""
Write-Host "Log disponible dans: $logFile" -ForegroundColor Gray

# Lancer le script d'ouverture des navigateurs en arriere-plan
$browserScript = Join-Path $PSScriptRoot "open-browsers.ps1"
if (Test-Path $browserScript) {
    $browserArgs = @("-ExecutionPolicy", "Bypass", "-File", $browserScript)
    Start-Process powershell -ArgumentList $browserArgs -WindowStyle Hidden
    "Script navigateurs lance" | Out-File -FilePath $logFile -Append
}

"=== Fin du script $(Get-Date) ===" | Out-File -FilePath $logFile -Append
