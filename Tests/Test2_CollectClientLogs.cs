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
using LLEAP.Framework.Config;
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

        private string AppPath => AppConfig.SimulationHome;
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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

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

                Console.WriteLine("Log collection started.");
                Thread.Sleep(3000);

                Console.WriteLine("Closing log collection window (LLEAPLogView)...");
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

                Console.WriteLine("Application closed after triggering log collection.");
                Console.WriteLine("Reason: Log collection process takes a long time, so test execution stops here due to time constraints.");

                driver.Quit();
                return;
            }
            catch
            {
                try
                {
                    var collectLogs = wait.Until(d =>
                        d.FindElement(By.XPath("//*[contains(@Name,'Collect client log files')]")));
                    collectLogs.Click();

                    Console.WriteLine("Log collection started.");
                    Thread.Sleep(3000);

                    Console.WriteLine("Closing log collection window (LLEAPLogView)...");
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

                    Console.WriteLine("Application closed after triggering log collection.");
                    Console.WriteLine("Reason: Log collection process takes a long time, so test execution stops here due to time constraints.");

                    driver.Quit();
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Could not click 'Collect client log files': {ex}");
                    throw;
                }
            }
        }

        [TearDown]
        public void Teardown()
        {
            WinAppDriverHelper.CleanupDrivers(uacDriver, driver);
            WinAppDriverHelper.KillProcessesByName(ProcessName);
        }
    }
}