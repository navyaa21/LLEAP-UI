Write-Host "🚀 Starting WinAppDriver..." -ForegroundColor Green
Get-Process WinAppDriver -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Process "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
Start-Sleep -Seconds 5

Write-Host "🧪 Running Test #1..." -ForegroundColor Cyan
dotnet test --filter RunTest1

Write-Host "
✅ TEST COMPLETE!" -ForegroundColor Green
Write-Host "Starting LLEAP automation tests..."

dotnet test LLEAP.csproj --logger "console;verbosity=detailed"

Write-Host "Test execution completed."

Write-Host "Cleaning up automation processes..."

Get-Process dotnet,WinAppDriver,vstest.console,LLEAPLogView,InstructorApplication,LaunchPortal -ErrorAction SilentlyContinue | Stop-Process -Force

Write-Host "Cleanup finished."