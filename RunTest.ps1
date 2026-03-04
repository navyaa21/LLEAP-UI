Write-Host "🚀 Starting WinAppDriver..." -ForegroundColor Green
Get-Process WinAppDriver -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
Start-Sleep -Seconds 5

Write-Host "🧪 Running Test #1..." -ForegroundColor Cyan
dotnet test --filter RunTest1

Write-Host "
✅ TEST COMPLETE!" -ForegroundColor Green
