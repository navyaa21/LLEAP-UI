# Update-Locator.ps1
# Run this script to update the license button selector

param(
    [string]C:\Users\navee\Desktop\LLEAP-Automation = "C:\Users\navee\Desktop\LLEAP-Automation",
    [string] = "",
    [string] = "",
    [string] = ""
)

 = Join-Path C:\Users\navee\Desktop\LLEAP-Automation "appsettings.json"

if (Test-Path ) {
     = Get-Content  -Raw | ConvertFrom-Json
    
    if () {
        .ElementLocators.LicenseButton.Name = 
        Write-Host "✅ Updated button Name to: " -ForegroundColor Green
    }
    
    if () {
        .ElementLocators.LicenseButton.AutomationId = 
        Write-Host "✅ Updated AutomationId to: " -ForegroundColor Green
    }
    
    if () {
        .ElementLocators.LicenseButton.XPath = 
        Write-Host "✅ Updated XPath to: " -ForegroundColor Green
    }
    
     | ConvertTo-Json -Depth 10 | Set-Content 
    Write-Host "
✅ Configuration updated successfully!" -ForegroundColor Green
} else {
    Write-Host "❌ Config file not found!" -ForegroundColor Red
}
