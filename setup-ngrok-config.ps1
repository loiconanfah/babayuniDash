# Copier ngrok.yml vers le repertoire de configuration ngrok
$ngrokConfigPath = "$env:USERPROFILE\AppData\Local\ngrok\ngrok.yml"
$ngrokDir = Split-Path $ngrokConfigPath

# Creer le repertoire si necessaire
if (-not (Test-Path $ngrokDir)) {
    New-Item -ItemType Directory -Path $ngrokDir -Force | Out-Null
}

# Copier le fichier
Copy-Item -Path "ngrok.yml" -Destination $ngrokConfigPath -Force

Write-Host "Fichier ngrok.yml copie vers:" -ForegroundColor Green
Write-Host $ngrokConfigPath -ForegroundColor White
Write-Host ""
Write-Host "Vous pouvez maintenant lancer: ngrok start --all" -ForegroundColor Yellow

