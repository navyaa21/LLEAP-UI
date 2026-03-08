

# LLEAP UI Automation Framework

Windows UI Automation framework for **Laerdal LLEAP applications** using:

* **WinAppDriver**
* **Appium .NET client**
* **Selenium**
* **NUnit**
* **.NET 6**

This framework automates desktop workflows such as:

* Launching Instructor Application
* Starting simulation sessions
* Modifying patient parameters
* Collecting logs from Laerdal Simulation Home

---

# Framework Architecture

```
LLEAP-Automation
│
├── Framework
│   ├── Core
│   │   ├── BaseTest.cs
│   │   └── DriverFactory.cs
│   │
│   └── Utilities
│       └── ConfigManager.cs
│
├── Tests
│   ├── Test1_LicenseFreeSession.cs
│   ├── Test2_CollectClientLogs.cs
│   └── WinAppDriverHelper.cs
│
├── RunTests.ps1
├── appsettings.json
├── LLEAP.csproj
└── README.md
```

---

# Technologies Used

| Tool             | Purpose                      |
| ---------------- | ---------------------------- |
| WinAppDriver     | Windows UI automation server |
| Appium.WebDriver | Client library               |
| Selenium         | UI interaction               |
| NUnit            | Test framework               |
| .NET 6           | Runtime                      |
| PowerShell       | Test runner script           |
| GitHub Actions   | CI/CD                        |

---

# Prerequisites

Install the following before running tests.

### 1 Install .NET 6 SDK

Download
[https://dotnet.microsoft.com/download/dotnet/6.0](https://dotnet.microsoft.com/download/dotnet/6.0)

Verify

```powershell
dotnet --version
```

---

### 2 Install WinAppDriver

Download

```
https://github.com/microsoft/WinAppDriver/releases
```

Install to

```
C:\Program Files (x86)\Windows Application Driver
```

Verify

```
WinAppDriver.exe
```

---

### 3 Install Required Applications

These applications must exist on the machine running automation.

Instructor Application

```
C:\Program Files (x86)\Laerdal Medical\Instructor Application\InstructorApplication\InstructorApplication.exe
```

Laerdal Simulation Home

```
C:\Program Files (x86)\Laerdal Medical\Laerdal Simulation Home\LaunchPortal.exe
```

---

### 4 Run PowerShell as Administrator

WinAppDriver and UI automation require **administrator privileges**.

---

# Configuration

The framework uses **appsettings.json** to manage paths.

Example:

```json
{
  "Applications": {
    "InstructorApp": "C:\\Program Files (x86)\\Laerdal Medical\\Instructor Application\\InstructorApplication\\InstructorApplication.exe",
    "SimulationHome": "C:\\Program Files (x86)\\Laerdal Medical\\Laerdal Simulation Home\\LaunchPortal.exe"
  },
  "WinAppDriver": {
    "Url": "http://127.0.0.1:4723"
  }
}
```

Update paths if applications are installed elsewhere.

---

# Running Tests

Tests are executed using the **RunTests.ps1** script.

---

## Run Test 1

License Free Session automation.

```powershell
.\RunTests.ps1 -Test1
```

Test steps:

1 Launch Instructor Application
2 Add license later
3 Select local computer
4 Start manual simulation
5 Change eye state
6 Adjust lung compliance
7 Modify heart rate
8 Play coughing sound

---

## Run Test 2

Collect client logs from Laerdal Simulation Home.

```powershell
.\RunTests.ps1 -Test2
```

Test steps:

1 Launch Simulation Home
2 Right click Help tile
3 Select **Collect client log files**
4 Handle UAC popup
5 Verify log collection

Note

Log collection can take a **very long time**.
The test intentionally closes the application early due to time constraints.

---

## Run All Tests

```powershell
.\RunTests.ps1 -All
```

---

# Running Tests Using dotnet

You can also run tests directly using the .NET CLI.

Run specific test

```powershell
dotnet test --filter Test1_LicenseFreeSession
```

Run log collection test

```powershell
dotnet test --filter Test2_CollectClientLogs
```

Run all tests

```powershell
dotnet test
```

---

# Test Cleanup

After execution, the script automatically cleans up processes.

Example PowerShell cleanup

```powershell
Stop-Process -Name dotnet -Force -ErrorAction SilentlyContinue
Stop-Process -Name WinAppDriver -Force -ErrorAction SilentlyContinue
```

This prevents leftover processes consuming memory.

---

# Continuous Integration

The framework supports **GitHub Actions**.

Automation runs on a **self hosted Windows runner** because UI automation requires:

* Installed applications
* Desktop session
* WinAppDriver
* Local UI interaction

---

## CI Workflow Location

```
.github/workflows/ui-automation.yml
```

---

## Example Workflow

```yaml
name: LLEAP UI Automation

on:
  workflow_dispatch:

jobs:
  run-tests:
    runs-on: [self-hosted, windows]

    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: 6.0.x

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build

      - name: Run Tests
        run: powershell -ExecutionPolicy Bypass -File RunTests.ps1 -All
```

---

# Best Practices

### Always run tests

* On a clean machine
* With administrator privileges
* With WinAppDriver running

---

### Avoid committing build artifacts

Add to `.gitignore`

```
bin/
obj/
TestResults/
.vs/
```

---

# Troubleshooting

## WinAppDriver not starting

Run manually

```
WinAppDriver.exe
```

---

## Element not found

Use **Inspect.exe** from Windows SDK to locate UI elements.

---

## UAC blocking automation

Run PowerShell as **Administrator**.

---

## Tests fail due to timing

Increase waits in code or use:

```
WebDriverWait
```

instead of `Thread.Sleep`.

---

# Future Improvements

Possible enhancements:

* Page Object Model
* Screenshot capture on failure
* Reporting integration
* Retry logic for unstable UI steps
* Parallel test execution
* Docker based Windows runner

---

# Author

Navya

GitHub
[https://github.com/navyaa21/LLEAP-UI](https://github.com/navyaa21/LLEAP-UI)

