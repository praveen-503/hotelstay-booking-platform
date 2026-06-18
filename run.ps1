# HotelStay Booking Platform Launcher

Write-Host "=============================================" -ForegroundColor Cyan
Write-Host "   HotelStay Booking Platform Local Launcher" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

# 1. Check if node_modules exists in the UI directory, run npm install if missing
$uiDir = Join-Path $PSScriptRoot "hotelstay-ui"
$nodeModules = Join-Path $uiDir "node_modules"

if (-not (Test-Path $nodeModules)) {
    Write-Host "`n[1/2] Installing frontend dependencies..." -ForegroundColor Yellow
    Push-Location $uiDir
    npm install
    Pop-Location
    Write-Host "Frontend dependencies installed successfully!" -ForegroundColor Green
} else {
    Write-Host "`n[1/2] Frontend dependencies already installed. Skipping npm install." -ForegroundColor Green
}

# 2. Start the Backend API in a new PowerShell window
Write-Host "`n[2/2] Launching applications..." -ForegroundColor Yellow
Write-Host "Starting Backend API on https://localhost:7252 in a new window..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'Starting ASP.NET Core Backend API...'; dotnet run --project HotelStay.Api --launch-profile https"

# 3. Start the Frontend UI in a new PowerShell window
Write-Host "Starting Frontend UI on http://localhost:4200 in a new window..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "Write-Host 'Starting Angular Frontend...'; cd hotelstay-ui; npm start"

Write-Host "`nApplications started! Check the newly opened PowerShell windows for logs." -ForegroundColor Green
Write-Host "=============================================" -ForegroundColor Cyan
