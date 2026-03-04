# RunTests.ps1 - AUTO-STARTS WinAppDriver
param(
    [switch]$Test1,
    [switch]$Test2,
    [switch]$All,
    [switch]$NoServerRestart
)

# Function to ensure WinAppDriver is running
function Ensure-WinAppDriverRunning {
    Write-Host "🔍 Checking WinAppDriver server..." -ForegroundColor Yellow
    
    # Check if WinAppDriver is already running and responding
    $process = Get-Process WinAppDriver -ErrorAction SilentlyContinue
    $serverRunning = $false
    
    if ($process) {
        Write-Host "   WinAppDriver process found (PID: $($process.Id))" -ForegroundColor Gray
        
        # Test if server is actually responding
        try {
            $tcpClient = New-Object System.Net.Sockets.TcpClient
            $connection = $tcpClient.BeginConnect("127.0.0.1", 4723, $null, $null)
            $wait = $connection.AsyncWaitHandle.WaitOne(2000, $false)
            if ($wait) {
                $tcpClient.EndConnect($connection)
                $tcpClient.Close()
                $serverRunning = $true
                Write-Host "   ✅ WinAppDriver server is responding!" -ForegroundColor Green
            } else {
                $tcpClient.Close()
                Write-Host "   ⚠️  WinAppDriver process found but not responding" -ForegroundColor Yellow
            }
        } catch {
            Write-Host "   ⚠️  WinAppDriver process found but not responding: $_" -ForegroundColor Yellow
        }
    }
    
    # If server is not running or not responding, start it
    if (-not $serverRunning) {
        Write-Host "   🚀 Starting WinAppDriver server..." -ForegroundColor Green
        
        # Kill any existing processes
        Get-Process WinAppDriver -ErrorAction SilentlyContinue | Stop-Process -Force
        Start-Sleep -Seconds 2
        
        # Start WinAppDriver
        $winAppDriverPath = "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
        if (-not (Test-Path $winAppDriverPath)) {
            Write-Host "   ❌ WinAppDriver not found at: $winAppDriverPath" -ForegroundColor Red
            exit 1
        }
        
        Start-Process -FilePath $winAppDriverPath -WindowStyle Normal
        
        # Wait for server to start
        Write-Host "   ⏳ Waiting for server to initialize..." -ForegroundColor Yellow
        $maxWait = 15
        $connected = $false
        
        for ($i = 1; $i -le $maxWait; $i++) {
            Start-Sleep -Seconds 1
            try {
                $tcpClient = New-Object System.Net.Sockets.TcpClient
                $connection = $tcpClient.BeginConnect("127.0.0.1", 4723, $null, $null)
                $wait = $connection.AsyncWaitHandle.WaitOne(1000, $false)
                if ($wait) {
                    $tcpClient.EndConnect($connection)
                    $tcpClient.Close()
                    $connected = $true
                    break
                }
                $tcpClient.Close()
            } catch {}
        }
        
        if ($connected) {
            Write-Host "   ✅ WinAppDriver server started successfully!" -ForegroundColor Green
        } else {
            Write-Host "   ❌ Failed to start WinAppDriver server!" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "   ✅ Using existing WinAppDriver server" -ForegroundColor Green
    }
}

# Main script
Clear-Host
Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║              LLEAP TEST RUNNER                             ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Check admin
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
if (-not $isAdmin) {
    Write-Host "❌ This script must be run as Administrator!" -ForegroundColor Red
    exit 1
}

# Ensure WinAppDriver is running
Ensure-WinAppDriverRunning

# Navigate to project
Set-Location $PSScriptRoot

# Build project
Write-Host "`n🔨 Building project..." -ForegroundColor Yellow
dotnet build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Build successful!" -ForegroundColor Green

# Run tests based on parameters
Write-Host "`n🧪 Running tests..." -ForegroundColor Cyan
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Cyan

if ($Test1) {
    Write-Host "Running Test #1 only..." -ForegroundColor White
    dotnet test --filter Test1_LicenseFreeSession
}
elseif ($Test2) {
    Write-Host "Running Test #2 only..." -ForegroundColor White
    dotnet test --filter Test2_CollectLogs
}
elseif ($All) {
    Write-Host "Running ALL tests..." -ForegroundColor White
    dotnet test
}
else {
    # Default: show menu
    Write-Host "Select tests to run:" -ForegroundColor Yellow
    Write-Host "1. Test #1 (License-Free Session)"
    Write-Host "2. Test #2 (Collect Logs)"
    Write-Host "3. All tests"
    $choice = Read-Host "`nEnter choice (1-3)"
    
    switch ($choice) {
        "1" { dotnet test --filter Test1_LicenseFreeSession }
        "2" { dotnet test --filter Test2_CollectLogs }
        "3" { dotnet test }
        default { dotnet test --filter Test1_LicenseFreeSession }
    }
}

Write-Host "`n════════════════════════════════════════════════════════════" -ForegroundColor Green
Write-Host "✅ Test execution complete!" -ForegroundColor Green
Write-Host "════════════════════════════════════════════════════════════" -ForegroundColor Green

# Keep window open if double-clicked
if ($host.Name -like "*ISE*" -or $host.Name -like "*Console*") {
    Write-Host "`nPress any key to exit..." -ForegroundColor Gray
    $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
}
