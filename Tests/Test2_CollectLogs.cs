using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using LLEAP.Core;
using LLEAP.Helpers;
using System.Threading;

namespace LLEAP.Tests
{
    [TestFixture]
    public class Test2_CollectLogs : BaseTest
    {
        [Test]
        public void RunCollectLogs()
        {
            Logger.Step("=== TEST #2: Collect Client Logs ===");
            
            // Find Help tile
            Logger.Step("Right-clicking Help tile");
            var helpTile = WaitHelper.WaitForElement(Driver, By.Name("Help"), 30);
            
            // Right click
            var actions = new Actions(Driver);
            actions.ContextClick(helpTile).Perform();
            Thread.Sleep(2000);
            
            // Click Collect client log files
            Logger.Step("Clicking Collect client log files");
            var collectOption = WaitHelper.WaitForClickable(Driver, By.Name("Collect client log files"), 10);
            collectOption?.Click();
            Thread.Sleep(3000);
            
            // Handle UAC if appears
            try
            {
                var uacYes = Driver.FindElement(By.Name("Yes"));
                uacYes?.Click();
                Logger.Info("UAC handled");
            }
            catch { }
            
            // Verify success
            try
            {
                var success = WaitHelper.WaitForElement(Driver, By.Name("Logs collected successfully"), 30);
                Assert.IsNotNull(success);
                Logger.Success("✅ Logs collected successfully");
                
                // Click OK
                Driver.FindElement(By.Name("OK")).Click();
            }
            catch
            {
                Logger.Error("Success message not found");
                Assert.Fail("Logs were not collected");
            }
            
            Logger.Success("✅ TEST #2 COMPLETED");
        }
    }
}
