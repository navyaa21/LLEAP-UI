# QuickTest.ps1 - Just run Test #1 with auto-server-start
param(
    [string]$TestNumber = "1"
)

Write-Host "🚀 Quick Test Runner - Auto-starting WinAppDriver if needed" -ForegroundColor Cyan
Write-Host "========================================================"

# Just call the main runner with appropriate parameter
if ($TestNumber -eq "1") {
    & ".\RunTests.ps1" -Test1
} elseif ($TestNumber -eq "2") {
    & ".\RunTests.ps1" -Test2
} else {
    & ".\RunTests.ps1" -All
}
