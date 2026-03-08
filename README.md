
# LLEAP Automation Framework

This project is a Windows desktop UI automation framework built with:

- **C#**
- **.NET 6**
- **NUnit**
- **WinAppDriver**
- **Appium Windows Driver**

It automates Laerdal desktop applications such as:

- **Instructor Application**
- **Laerdal Simulation Home**

The framework is designed so test cases stay simple, while reusable Windows automation logic is kept in helper files.

---

# Project Goal

The goal of this framework is to automate common end-to-end workflows in Laerdal applications, such as:

- launching applications
- clicking buttons and menus
- handling popup windows
- switching between desktop windows
- interacting with controls like combo boxes, buttons, tabs, panes, and sliders
- validating that important workflows complete successfully

---

# Project Structure

```text
LLEAP-AUTOMATION
│
├── Tests
│   ├── Test1_LicenseFreeSession.cs
│   ├── Test2_CollectLogs.cs
│   ├── WinAppDriverHelper.cs
│   └── TestData
│
├── Framework
├── bin
├── obj
├── appsettings.json
├── LLEAP.csproj
├── LLEAP-Automation.sln
├── RunTest.ps1
├── RunTests.ps1
├── QuickTest.ps1
└── README.md
````

---

# Main Files Explained

## `Test1_LicenseFreeSession.cs`

This test automates a full license-free session flow in the Instructor Application.

Example actions:

* click **Add license later**
* select simulator
* select manual mode
* switch to popup windows
* start session
* set eyes to closed
* set compliance
* set heart rate
* play voice
* close application

## `Test2_CollectLogs.cs`

This test automates log collection in Laerdal Simulation Home.

Example actions:

* start the app
* right-click the **Help** tile
* select **Collect client log files**
* handle UAC if needed
* optionally close the log collection process due to long runtime

## `WinAppDriverHelper.cs`

This file contains reusable helper methods used by all tests.

Examples:

* start WinAppDriver
* kill old app processes
* launch application
* switch between Windows popup windows
* clean up drivers

This is the core utility file of the framework.

---

# How the Framework Works

## 1. Setup

Each test starts by:

* making sure **WinAppDriver** is running
* killing old instances of the application
* launching the target application
* creating the main Windows driver session

## 2. Test Steps

The test then performs actions using:

* `FindElement`
* `WebDriverWait`
* `Actions`
* helper methods like `SwitchToWindow(...)`

## 3. Cleanup

At the end, the framework:

* closes all driver sessions
* kills leftover processes

This helps keep test runs clean and prevents old windows from interfering with future runs.

---

# Technologies Used

## NUnit

Used as the test framework.

Important attributes:

* `[TestFixture]` → marks a class as a test class
* `[SetUp]` → runs before each test
* `[Test]` → marks a test method
* `[TearDown]` → runs after each test

## WinAppDriver

Used to automate Windows desktop applications.

It works similarly to Selenium, but for Windows UI.

## Appium Windows Driver

Used to create driver sessions for Windows applications and windows.

---

# Prerequisites

Before running this framework, install the following:

## 1. .NET 6 SDK

Install .NET 6 SDK.

## 2. WinAppDriver

Install **Windows Application Driver**.

Default path:

```text
C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe
```

## 3. Visual Studio Code or Visual Studio

Recommended for editing and running tests.

## 4. Required NuGet Packages

Make sure the project includes packages like:

* `NUnit`
* `NUnit3TestAdapter`
* `Microsoft.NET.Test.Sdk`
* `Appium.WebDriver`
* `Selenium.WebDriver`
* `Selenium.Support`

---

# How to Run the Tests

## Run all tests

```powershell
dotnet test LLEAP.csproj --logger "console;verbosity=detailed"
```

## Run a single test by name

```powershell
dotnet test LLEAP.csproj --filter "FullyQualifiedName~Test1_LicenseFreeSession" --logger "console;verbosity=detailed"
```

or

```powershell
dotnet test LLEAP.csproj --filter "FullyQualifiedName~Test2_CollectClientLogs" --logger "console;verbosity=detailed"
```

## Run using PowerShell script

If your scripts are already set up, you can also use:

```powershell
.\RunTest.ps1
```

or

```powershell
.\RunTests.ps1
```

---

# Important Framework Concepts

## Driver

A `WindowsDriver<WindowsElement>` represents a session connected to a Windows application or a specific window.

Example:

```csharp
driver = WinAppDriverHelper.LaunchApplication(AppPath);
```

## Wait

`WebDriverWait` waits until an element appears before interacting with it.

Example:

```csharp
var button = wait.Until(d => d.FindElement(By.Name("Ok")));
button.Click();
```

This is better than clicking immediately because desktop apps often need time to load.

## Actions

`Actions` is used for advanced interactions like:

* right click
* drag and drop
* keyboard shortcuts

Example:

```csharp
actions.ContextClick(helpTile).Perform();
```

## SwitchToWindow

Some workflows open new popup windows.
For that, the framework uses:

```csharp
WinAppDriverHelper.SwitchToWindow("Select theme", "InstructorApplication");
```

This finds the window by name and attaches a new driver session to it.

---

# Helper Methods

## `EnsureWinAppDriverRunning()`

Checks whether WinAppDriver is running.
If not, it starts it automatically.

## `KillProcessesByName(processName)`

Kills all running processes for the given app name.

Used to prevent old sessions from interfering with the next test.

## `LaunchApplication(appPath)`

Launches the target application and returns the main driver.

## `SwitchToWindow(windowName, processName, timeoutSeconds)`

Attaches to a popup or secondary window by title.

## `CleanupDrivers(...)`

Safely closes all open driver sessions.

---

# Writing a New Test Case

To add a new test:

## Step 1: Create a new file in `Tests`

Example:

```text
Tests\Test3_SomeFeature.cs
```

## Step 2: Create a test class

```csharp
[TestFixture]
public class Test3_SomeFeature
{
}
```

## Step 3: Add Setup, Test, and TearDown

```csharp
[SetUp]
public void Setup()
{
    // launch app
}

[Test]
public void RunTest3()
{
    // test steps
}

[TearDown]
public void Teardown()
{
    // cleanup
}
```

## Step 4: Reuse helper methods

Instead of duplicating launch and window-switch code, use:

```csharp
WinAppDriverHelper.EnsureWinAppDriverRunning();
WinAppDriverHelper.LaunchApplication(AppPath);
WinAppDriverHelper.SwitchToWindow("Window Name", ProcessName);
```

---

# Best Practices Used in This Framework

## 1. Use explicit waits

Avoid clicking elements without waiting for them.

Good:

```csharp
wait.Until(d => d.FindElement(By.Name("OK"))).Click();
```

Bad:

```csharp
driver.FindElement(By.Name("OK")).Click();
```

## 2. Keep reusable code in helpers

Window switching, app launch, and cleanup should stay in helper files.

## 3. Keep test files readable

A test file should show business steps clearly:

```csharp
Console.WriteLine("Step 1: Add license later");
Console.WriteLine("Step 2: Select simulator");
Console.WriteLine("Step 3: Start session");
```

## 4. Add logging

Use `Console.WriteLine(...)` so beginners can follow the test flow.

## 5. Always clean up

Desktop apps can leave windows open if cleanup is skipped.

---

# Common Problems and Fixes

## Problem: Window not found

Example:

```text
Window 'Select theme' not found.
```

### Fix:

* increase timeout
* confirm exact window title in Inspect
* make sure the popup really appeared
* verify process name

## Problem: Element not found

Example:

```text
An element could not be located on the page
```

### Fix:

* use Inspect.exe to inspect the UI tree
* verify `Name`, `AutomationId`, and control type
* switch to the correct window first
* use `WebDriverWait`

## Problem: Test not discovered

Example:

```text
No test matches the given testcase filter
```

### Fix:

* make sure class has `[TestFixture]`
* make sure method has `[Test]`
* use the correct test filter name

## Problem: UAC popup cannot be automated

Windows UAC sometimes appears on the secure desktop, which normal automation tools cannot control.

### Fix:

* allow admin access manually in test environment
* disable UAC in a safe test machine if allowed
* handle it outside normal automation if necessary

---

# How to Inspect Desktop Elements

Use **Inspect.exe** from Windows SDK to inspect UI elements.

Look for:

* `Name`
* `AutomationId`
* `ControlType`
* `BoundingRectangle`
* `NativeWindowHandle`

This information helps create the right locator.

Example locators:

```csharp
By.Name("OK")
MobileBy.AccessibilityId("EyesComboBox")
By.XPath("//Window//Button[@Name='Start session']")
```

---

# Example Locator Types

## By Name

```csharp
driver.FindElement(By.Name("OK"));
```

## By Accessibility ID

```csharp
driver.FindElement(MobileBy.AccessibilityId("PlayButton"));
```

## By XPath

```csharp
driver.FindElement(By.XPath("//Window//Button[@Name='Start session']"));
```

Use `AccessibilityId` first when available, because it is usually more stable.

---

# Current Test Coverage

## Test Case 1

**License-Free Session**

* Launch Instructor Application
* Add license later
* Select simulator
* Select manual mode
* Configure session
* Set vitals and sounds
* Close application

## Test Case 2

**Collect Client Logs**

* Launch Laerdal Simulation Home
* Open Help tile menu
* Trigger client log collection
* Handle UAC if needed
* Stop early if log collection is too long

---

# Future Improvements

Possible improvements for this framework:

* Page Object Model structure
* better logging to file
* screenshots on failure
* test result reports
* reusable control helper methods
* configuration-driven app paths
* CI/CD execution support

---

# Beginner Tips

If you are new to this framework:

1. Start by reading `WinAppDriverHelper.cs`
2. Run one existing test first
3. Use Inspect.exe to understand UI elements
4. Add one step at a time
5. Print logs often with `Console.WriteLine`

---

# Summary

This framework is a reusable Windows desktop automation project for Laerdal applications.

It helps automate:

* application launch
* popup handling
* control interaction
* cleanup

The design goal is simple:

* keep test cases readable
* keep technical logic reusable
* make it easy for beginners to extend

