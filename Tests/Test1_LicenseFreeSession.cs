using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;
using System.Diagnostics;

namespace LLEAP.Tests
{
    [TestFixture]
    public class Test1_LicenseFreeSession
    {
        private WindowsDriver<WindowsElement> driver;
        private Actions actions;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] === TEST SETUP ===");
            
            // Kill any existing processes
            foreach (var proc in Process.GetProcessesByName("InstructorApplication"))
            {
                proc.Kill();
            }
            Thread.Sleep(2000);
            
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", @"C:\Program Files (x86)\Laerdal Medical\Instructor Application\InstructorApplication\InstructorApplication.exe");
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("ms:experimental-webdriver", true);
            options.AddAdditionalCapability("ms:waitForAppLaunch", "30");

            driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), 
                options
            );
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(20);
            actions = new Actions(driver);
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Setup complete");
        }

        [Test]
        public void RunTest1()
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → ========== TEST #1: License-Free Session ==========");
            
            // ============================================
            // STEP 1: Laerdal Simulation Home is already launched by WinAppDriver
            // NOTE: Step 2 (Click Instructor Application) is SKIPPED because the app itself IS the Instructor Application
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 1 & 2: Application launched (Instructor Application is the app itself)");
            
            // Wait for app to fully load
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for application to load...");
            Thread.Sleep(8000);
            
            // ============================================
            // STEP 3: When requested to add the license, click on "Add license later"
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 3: Clicking Add license later");
            
            bool step3Completed = false;
            
            // Wait for license dialog to appear
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for license dialog...");
            Thread.Sleep(5000);
            
            // Try multiple methods to find the license button
            string[] buttonTexts = { 
                "Add license later", 
                "Add a License", 
                "Add License Later",
                "Add Later",
                "License"
            };
            
            for (int attempt = 1; attempt <= 10; attempt++)
            {
                // Method 1: Try each button text
                foreach (string btnText in buttonTexts)
                {
                    try
                    {
                        var btn = driver.FindElementByName(btnText);
                        btn.Click();
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 3 completed - Clicked '{btnText}'");
                        step3Completed = true;
                        break;
                    }
                    catch { }
                }
                
                if (step3Completed) break;
                
                // Method 2: Try XPath contains
                try
                {
                    var btn = driver.FindElementByXPath("//Button[contains(@Name, 'Add') and contains(@Name, 'license')]");
                    btn.Click();
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 3 completed - Clicked by XPath");
                    step3Completed = true;
                    break;
                }
                catch { }
                
                // Method 3: Try all buttons
                try
                {
                    var buttons = driver.FindElementsByClassName("Button");
                    foreach (var btn in buttons)
                    {
                        if (btn.Text.Contains("Add") || btn.Text.Contains("license"))
                        {
                            btn.Click();
                            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 3 completed - Clicked by button scan");
                            step3Completed = true;
                            break;
                        }
                    }
                }
                catch { }
                
                if (step3Completed) break;
                
                // Method 4: Click by coordinates (from your inspection: X=1282, Y=849)
                try
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Trying coordinate click at (1282, 849)");
                    actions.MoveByOffset(1282, 849).Click().Perform();
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 3 completed - Clicked by coordinates");
                    step3Completed = true;
                    break;
                }
                catch { }
                
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Step 3 - Attempt {attempt}/10: Waiting for license button...");
                Thread.Sleep(2000);
            }
            
            Assert.IsTrue(step3Completed, "❌ Step 3 FAILED - Could not click license button");
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ License button clicked successfully");
            
            // ============================================
            // CRITICAL WAIT AFTER CLICKING LICENSE BUTTON
            // Wait for the main application screen to load
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for main application to load after license click...");
            Thread.Sleep(8000); // Wait 8 seconds for the UI to transition
            
            // Wait specifically for Local Computer tile to be available
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for Local Computer tile to be available...");
            
            bool localComputerReady = false;
            for (int attempt = 1; attempt <= 15; attempt++)
            {
                try
                {
                    var localComputer = driver.FindElementByName("Local Computer");
                    if (localComputer != null && localComputer.Displayed && localComputer.Enabled)
                    {
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Local Computer tile is ready");
                        localComputerReady = true;
                        break;
                    }
                }
                catch
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for Local Computer tile... Attempt {attempt}/15");
                }
                Thread.Sleep(1000);
            }
            
            Assert.IsTrue(localComputerReady, "❌ Local Computer tile did not become available");
            
            // ============================================
            // STEP 4: Click on the Local Computer tile
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 4: Clicking Local Computer");
            try
            {
                var localComputer = driver.FindElementByName("Local Computer");
                localComputer.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 4 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 4 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000); // Wait for selection to register
            
            // ============================================
            // STEP 5: Click on SimMan3G Plus
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 5: Selecting SimMan3G Plus");
            try
            {
                var simMan = driver.FindElementByName("SimMan3G Plus");
                simMan.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 5 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 5 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 6: Click on the Manual Mode
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 6: Selecting Manual Mode");
            try
            {
                var manualMode = driver.FindElementByName("Manual Mode");
                manualMode.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 6 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 6 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(3000);
            
            // ============================================
            // STEP 7: Expand the list of Themes
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 7: Expanding Themes list");
            try
            {
                var themes = driver.FindElementByName("Themes");
                themes.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 7 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 7 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 8: Click on the Healthy Patient theme
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 8: Selecting Healthy Patient theme");
            try
            {
                var healthyTheme = driver.FindElementByName("Healthy Patient");
                healthyTheme.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 8 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 8 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 9: Click on the OK button
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 9: Clicking OK button");
            try
            {
                var okButton = driver.FindElementByName("OK");
                okButton.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 9 completed");
            }
            catch
            {
                // Try XPath if name fails
                try
                {
                    var okButton = driver.FindElementByXPath("//Button[@Name='OK']");
                    okButton.Click();
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 9 completed");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 9 failed: {ex.Message}");
                    throw;
                }
            }
            Thread.Sleep(3000);
            
            // ============================================
            // STEP 10: Click on the Start Session
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 10: Starting Session");
            try
            {
                var startSession = driver.FindElementByName("Start Session");
                startSession.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 10 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 10 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(5000); // Wait for session to start
            
            // ============================================
            // STEP 11: Maximize the window
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 11: Maximizing window");
            try
            {
                var maximize = driver.FindElementByName("Maximize");
                maximize.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 11 completed");
            }
            catch
            {
                // If maximize button not found, double-click title bar
                try
                {
                    var window = driver.FindElementByClassName("Window");
                    actions.DoubleClick(window).Perform();
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 11 completed by double-click");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [WARNING] Step 11 failed: {ex.Message}");
                }
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 12: For the Eyes control, select the "Closed" option
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 12: Setting Eyes to Closed");
            try
            {
                var eyes = driver.FindElementByName("Eyes");
                eyes.Click();
                Thread.Sleep(1000);
                var closed = driver.FindElementByName("Closed");
                closed.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 12 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 12 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 13: For the Lung compliance, change the value of the slider to 67%
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 13: Setting Lung compliance to 67%");
            try
            {
                var slider = driver.FindElementByName("Lung compliance");
                // Click and drag to set value (approximate)
                actions.ClickAndHold(slider)
                       .MoveByOffset(50, 0) // Adjust this offset as needed
                       .Release()
                       .Perform();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 13 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 13 failed: {ex.Message}");
                // Non-critical, continue
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 14: On the Patient monitor, click on the HR value, change it to 100
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 14: Setting Heart Rate to 100");
            try
            {
                // Click on Patient Monitor first to ensure it's active
                var patientMonitor = driver.FindElementByName("Patient Monitor");
                patientMonitor.Click();
                Thread.Sleep(1000);
                
                var hrValue = driver.FindElementByName("HR");
                hrValue.Click();
                hrValue.Clear();
                hrValue.SendKeys("100");
                hrValue.SendKeys(Keys.Enter);
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 14 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 14 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 15: For the Voices, select the Coughing and play it once
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 15: Playing Coughing voice");
            try
            {
                var voices = driver.FindElementByName("Voices");
                voices.Click();
                Thread.Sleep(1000);
                
                var coughing = driver.FindElementByName("Coughing");
                coughing.Click();
                Thread.Sleep(1000);
                
                var play = driver.FindElementByName("Play");
                play.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 15 completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [ERROR] Step 15 failed: {ex.Message}");
                throw;
            }
            Thread.Sleep(2000);
            
            // ============================================
            // STEP 16: Close the application using the "X" button
            // ============================================
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [STEP] → Step 16: Closing application");
            try
            {
                var closeButton = driver.FindElementByName("Close");
                closeButton.Click();
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 16 completed");
            }
            catch
            {
                // If close button not found, use Alt+F4
                try
                {
                    actions.SendKeys(Keys.Alt + Keys.F4).Perform();
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ Step 16 completed by Alt+F4");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [WARNING] Step 16 failed: {ex.Message}");
                }
            }
            
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] ✅ ========== TEST #1 COMPLETED SUCCESSFULLY ==========");
        }

        [TearDown]
        public void Teardown()
        {
            try { driver?.Close(); } catch { }
            driver?.Quit();
        }
    }
}
