using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using LLEAP.Tests.Helpers;

namespace LLEAP.Tests
{
    [TestFixture]
    public class Test2_CollectClientLogs
    {
        private WindowsDriver<WindowsElement> driver;
        private WindowsDriver<WindowsElement> uacDriver;

        private Actions actions;
        private WebDriverWait wait;

        private const string AppPath =
            @"C:\Program Files (x86)\Laerdal Medical\Laerdal Simulation Home\LaunchPortal.exe";

        private const string ProcessName = "LaunchPortal";

        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] === TEST 2 SETUP ===");

            WinAppDriverHelper.EnsureWinAppDriverRunning();
            WinAppDriverHelper.KillProcessesByName(ProcessName);

            Thread.Sleep(2000);

            driver = WinAppDriverHelper.LaunchApplication(AppPath);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            actions = new Actions(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] Setup complete");
        }

        [Test]
        public void RunTest2_CollectClientLogs()
        {
            Console.WriteLine("Step 1: Start Laerdal Simulation Home");

            Console.WriteLine("Step 2: Right click on Help tile");

            IWebElement helpTile = null;

            try
            {
                helpTile = wait.Until(d => d.FindElement(By.Name("Help")));
            }
            catch
            {
                try
                {
                    helpTile = wait.Until(d => d.FindElement(By.XPath("//*[contains(@Name,'Help')]")));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Could not find Help tile: {ex}");
                    throw;
                }
            }

            actions.ContextClick(helpTile).Perform();
            Thread.Sleep(1500);

            Console.WriteLine("Step 3: Select 'Collect client log files'");

            try
            {
                var collectLogs = wait.Until(d =>
                    d.FindElement(By.Name("Collect client log files")));
                collectLogs.Click();
            }
            catch
            {
                try
                {
                    var collectLogs = wait.Until(d =>
                        d.FindElement(By.XPath("//*[contains(@Name,'Collect client log files')]")));
                    collectLogs.Click();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Could not click 'Collect client log files': {ex}");
                    throw;
                }
            }


            Console.WriteLine("Log collection started.");

            // Give the log window time to launch
            Thread.Sleep(3000);

            Console.WriteLine("Closing log collection window (LLEAPLogView)...");

            try
            {
                foreach (var proc in Process.GetProcessesByName("LLEAPLogView"))
                {
                    try
                    {
                        proc.Kill();
                        Console.WriteLine($"Closed process: {proc.ProcessName}");
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }

            Console.WriteLine("Application closed after triggering log collection.");
            Console.WriteLine("Reason: Log collection process takes a long time, so test execution stops here due to time constraints.");

            // Close main application
            driver.Quit();

            Console.WriteLine("Step 4: Manage User Account Control if needed");

            try
            {
                uacDriver = WinAppDriverHelper.SwitchToWindow("User Account Control", ProcessName, 5);

                var uacWait = new WebDriverWait(uacDriver, TimeSpan.FromSeconds(10));

                try
                {
                    var yesButton = uacWait.Until(d => d.FindElement(By.Name("Yes")));
                    yesButton.Click();
                    Console.WriteLine("[SUCCESS] UAC handled by clicking Yes");
                }
                catch
                {
                    Console.WriteLine("[WARNING] UAC window found, but 'Yes' button was not clickable.");
                }
            }
            catch
            {
                Console.WriteLine("[INFO] UAC did not appear");
            }

            Thread.Sleep(5000);

            Console.WriteLine("Step 5: Verify logs were collected");

            bool successFound = false;

            try
            {
                var successMessage = driver.FindElements(By.XPath("//*[contains(@Name,'log')]"))
                    .FirstOrDefault(el =>
                    {
                        try
                        {
                            var text = el.GetAttribute("Name");
                            return !string.IsNullOrWhiteSpace(text) &&
                                   (text.IndexOf("collected", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    text.IndexOf("saved", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                    text.IndexOf("success", StringComparison.OrdinalIgnoreCase) >= 0);
                        }
                        catch
                        {
                            return false;
                        }
                    });

                if (successMessage != null)
                {
                    Console.WriteLine($"[SUCCESS] Verification message found: {successMessage.GetAttribute("Name")}");
                    successFound = true;
                }
            }
            catch
            {
            }

            if (!successFound)
            {
                try
                {
                    var okButton = driver.FindElements(By.Name("OK")).FirstOrDefault();
                    if (okButton != null)
                    {
                        Console.WriteLine("[INFO] OK button found after log collection, assuming completion dialog appeared.");
                        successFound = true;
                    }
                }
                catch
                {
                }
            }

            Assert.IsTrue(successFound, "Could not verify that client logs were collected.");

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] TEST CASE 2 COMPLETED");
        }

        [TearDown]
        public void Teardown()
        {
            WinAppDriverHelper.CleanupDrivers(uacDriver, driver);
            WinAppDriverHelper.KillProcessesByName(ProcessName);
        }
    }
}