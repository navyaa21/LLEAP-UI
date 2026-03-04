using NUnit.Framework;
using OpenQA.Selenium.Appium.Windows;
using LLEAP.Helpers;
using LLEAP.Config;
using System;

namespace LLEAP.Core
{
    [TestFixture]
    public abstract class BaseTest
    {
        protected WindowsDriver<WindowsElement> Driver;
        protected AppConfig Config;
        protected string TestName;

        [SetUp]
        public void Setup()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            Config = AppConfig.Load();
            
            Logger.Info($"=== STARTING TEST: {TestName} ===");
            
            try
            {
                Driver = DriverFactory.CreateDriver();
                Logger.Success("Driver initialized successfully");
            }
            catch (Exception ex)
            {
                Logger.Error($"Setup failed: {ex.Message}");
                throw;
            }
        }

        [TearDown]
        public void Teardown()
        {
            try
            {
                if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
                {
                    ScreenshotHelper.TakeScreenshot(Driver, TestName);
                }
                
                Driver?.Quit();
                Logger.Info($"=== TEST COMPLETED: {TestName} ===");
            }
            catch (Exception ex)
            {
                Logger.Error($"Teardown error: {ex.Message}");
            }
        }
    }
}
