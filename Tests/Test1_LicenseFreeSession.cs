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
    public class Test1_LicenseFreeSession
    {
        private WindowsDriver<WindowsElement> driver;
        private WindowsDriver<WindowsElement> themeDriver;
        private WindowsDriver<WindowsElement> pauseDriver;
        private WindowsDriver<WindowsElement> patientDriver;

        private Actions actions;
        private WebDriverWait wait;
        private Process instructorProc;

        private const string InstructorProcessName = "InstructorApplication";
        private const string AppPath =
            @"C:\Program Files (x86)\Laerdal Medical\Instructor Application\InstructorApplication\InstructorApplication.exe";

        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] === TEST SETUP ===");

            WinAppDriverHelper.EnsureWinAppDriverRunning();
            WinAppDriverHelper.KillProcessesByName(InstructorProcessName);

            Thread.Sleep(2000);

            driver = WinAppDriverHelper.LaunchApplication(AppPath);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);

            actions = new Actions(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] Setup complete");
        }

        [Test]
        public void RunTest1()
        {
            instructorProc = Process.GetProcessesByName(InstructorProcessName)
                .FirstOrDefault(p => p.MainWindowHandle != IntPtr.Zero);

            Console.WriteLine("Step 1: Add license later");
            wait.Until(d => d.FindElement(By.Name("Add license later"))).Click();

            Thread.Sleep(3000);

            Console.WriteLine("Step 2: Local computer");
            wait.Until(d => d.FindElement(By.Name("Local computer"))).Click();

            Console.WriteLine("Step 3: SimMan 3G PLUS");
            wait.Until(d => d.FindElement(By.Name("SimMan 3G PLUS"))).Click();

            Thread.Sleep(3000);

            Console.WriteLine("Step 4: Manual Mode");
            wait.Until(d => d.FindElement(By.Name("Manual Mode"))).Click();

            Thread.Sleep(2000);

            Console.WriteLine("Step 5: Switch to Select theme");
            themeDriver = WinAppDriverHelper.SwitchToWindow("Select theme", InstructorProcessName);

            new WebDriverWait(themeDriver, TimeSpan.FromSeconds(20))
                .Until(d => d.FindElement(By.Name("Ok")))
                .Click();

            Thread.Sleep(5000);

            Console.WriteLine("Step 6: Switch to PAUSE");
            pauseDriver = WinAppDriverHelper.SwitchToWindow("PAUSE", InstructorProcessName);

            Console.WriteLine("Step 7: Start session");
            var pauseWait = new WebDriverWait(pauseDriver, TimeSpan.FromSeconds(20));
            pauseWait.Until(d =>
                d.FindElement(By.XPath("//Window//Button[@Name='Start session']")))
                .Click();

            Thread.Sleep(3000);

            Console.WriteLine("Step 8: Switch to Healthy patient");
            patientDriver = WinAppDriverHelper.SwitchToWindow("Healthy patient", InstructorProcessName);
            var patientWait = new WebDriverWait(patientDriver, TimeSpan.FromSeconds(30));

            try
            {
                patientDriver.Manage().Window.Maximize();
            }
            catch
            {
                var patientActions = new Actions(patientDriver);
                patientActions.SendKeys(Keys.Alt + Keys.Space).Perform();
                Thread.Sleep(500);
                patientActions.SendKeys("x").Perform();
            }

            Thread.Sleep(1000);

            Console.WriteLine("Step 9: Set Eyes to Closed");
            var eyesCombo = patientWait.Until(d =>
                d.FindElement(MobileBy.AccessibilityId("EyesComboBox")));

            eyesCombo.Click();
            Thread.Sleep(500);
            patientWait.Until(d => d.FindElement(By.Name("Closed"))).Click();

            Thread.Sleep(1000);

            Console.WriteLine("Step 10: Set Lung compliance to 67%");

            var complianceSlider = patientWait.Until(d =>
                d.FindElement(MobileBy.AccessibilityId("compliance")));

            var sliderSize = complianceSlider.Size;
            int startX = 5;
            int y = sliderSize.Height / 2;
            int moveTo67 = (int)(sliderSize.Width * 0.33);

            var complianceActions = new Actions(patientDriver);
            complianceActions
                .MoveToElement(complianceSlider, startX, y)
                .ClickAndHold()
                .MoveByOffset(moveTo67, 0)
                .Release()
                .Perform();

            Console.WriteLine("Step 11: Set HR to 100");

            try
            {
                Console.WriteLine("Step 11: Click HR pane by coordinates");

                int clickX = 1560;
                int clickY = 159;

                new Actions(patientDriver)
                    .MoveByOffset(clickX, clickY)
                    .Click()
                    .Perform();

                Thread.Sleep(1000);



                Console.WriteLine("Switching to Set Heart Rate dialog...");
                var hrDialog = WinAppDriverHelper.SwitchToWindow("Set Heart Rate", InstructorProcessName);
                var hrDialogWait = new WebDriverWait(hrDialog, TimeSpan.FromSeconds(20));

                Console.WriteLine("Finding Edit boxes...");
                var editBoxes = hrDialogWait.Until(d => d.FindElements(By.ClassName("Edit")));
                Console.WriteLine($"Found {editBoxes.Count} Edit box(es)");

                if (editBoxes.Count < 2)
                {
                    throw new Exception("Expected at least 2 Edit controls in Set Heart Rate dialog.");
                }

                var newValueBox = editBoxes[1];

                Console.WriteLine("Entering new HR value 100...");
                newValueBox.Click();
                newValueBox.SendKeys(Keys.Control + "a");
                newValueBox.SendKeys(Keys.Delete);
                newValueBox.SendKeys("100");

                Console.WriteLine("Clicking OK...");
                hrDialogWait.Until(d => d.FindElement(By.Name("OK"))).Click();

                Thread.Sleep(1000);

                Console.WriteLine("Switching back to Healthy patient window...");
                patientDriver = WinAppDriverHelper.SwitchToWindow("Healthy patient", InstructorProcessName);
                patientWait = new WebDriverWait(patientDriver, TimeSpan.FromSeconds(30));

                Console.WriteLine("HR step completed successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Step 11 failed: {ex}");
                throw;
            }



            Console.WriteLine("Step 12: Play Coughing once");

            var coughingItem = patientWait.Until(d =>
                d.FindElement(By.Name("Coughing")));
            coughingItem.Click();

            Thread.Sleep(500);

            var playButton = patientWait.Until(d =>
                d.FindElement(MobileBy.AccessibilityId("PlayButton")));
            playButton.Click();

            Thread.Sleep(1000);

            Console.WriteLine("Step 13: Close application");

            var closeButton = patientWait.Until(d =>
                d.FindElement(MobileBy.AccessibilityId("Close")));
            closeButton.Click();
        }

        [TearDown]
        public void Teardown()
        {
            WinAppDriverHelper.CleanupDrivers(patientDriver, pauseDriver, themeDriver, driver);
            WinAppDriverHelper.KillProcessesByName(InstructorProcessName);
        }
    }
}
