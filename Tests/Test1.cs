using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using LLEAP.Pages;
using LLEAP.Helpers;
using System;
using System.Threading;

namespace LLEAP.Tests
{
    [TestFixture]
    public class Test1
    {
        private WindowsDriver<WindowsElement> driver;
        private SimulationHomePage homePage;
        private readonly string appPath = @"C:\Program Files (x86)\Laerdal Medical\Instructor Application\InstructorApplication\InstructorApplication.exe";

        [SetUp]
        public void Setup()
        {
            Logger.Info("=== TEST SETUP ===");
            
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", appPath);
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("ms:experimental-webdriver", true);

            driver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), 
                options
            );
            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            homePage = new SimulationHomePage(driver);
            
            Logger.Success("Setup complete");
        }

        [Test]
        public void RunTest1()
        {
            Logger.Step("=== TEST #1: License-Free Session ===");
            
            homePage.ClickInstructorApp();
            
            bool licenseClicked = homePage.ClickLicenseButton();
            Assert.IsTrue(licenseClicked, "Failed to click license button");
            
            homePage.ClickLocalComputer();
            homePage.ClickSimMan3GPlus();
            homePage.ClickManualMode();
            homePage.ClickThemesList();
            homePage.ClickHealthyTheme();
            homePage.ClickOkButton();
            homePage.ClickStartSession();
            homePage.ClickMaximize();
            homePage.ClickEyes();
            homePage.ClickEyesClosed();
            homePage.SetHeartRateTo100();
            homePage.PlayCoughing();
            homePage.CloseApp();
            
            Logger.Success("✅ TEST #1 COMPLETED");
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}
