using OpenQA.Selenium;
using LLEAP.Core;
using LLEAP.Helpers;
using System.Threading;

namespace LLEAP.Pages
{
    public class LicenseDialogPage : BasePage
    {
        public LicenseDialogPage(IWebDriver driver) : base(driver) { }

        public PatientSimulationPage ClickLicenseButton()
        {
            Logger.Step("Looking for license button");
            
            // Get locator from config
            var licenseConfig = Config.ElementLocators.LicenseButton;
            
            // Try multiple strategies in order of reliability
            By[] locators = {
                // 1. By AutomationId (most reliable if available)
                !string.IsNullOrEmpty(licenseConfig.AutomationId) 
                    ? By.XPath($"//*[@AutomationId='{licenseConfig.AutomationId}']") 
                    : null,
                
                // 2. By exact Name
                !string.IsNullOrEmpty(licenseConfig.Name) 
                    ? By.Name(licenseConfig.Name) 
                    : null,
                
                // 3. By XPath from config
                !string.IsNullOrEmpty(licenseConfig.XPath) 
                    ? By.XPath(licenseConfig.XPath) 
                    : null,
                
                // 4. By ClassName with text contains
                By.XPath($"//{licenseConfig.ClassName}[contains(@Name, 'Add')]"),
                
                // 5. Generic fallback
                By.XPath("//Button[contains(@Name, 'Add') or contains(@Name, 'license')]")
            };

            for (int attempt = 1; attempt <= 5; attempt++)
            {
                Logger.Info($"Attempt {attempt}/5 to find license button");
                
                foreach (var locator in locators)
                {
                    if (locator == null) continue;
                    
                    try
                    {
                        var button = WaitHelper.WaitForClickable(Driver, locator, 3);
                        if (button != null)
                        {
                            button.Click();
                            Logger.Success("✅ License button clicked!");
                            Thread.Sleep(2000);
                            return new PatientSimulationPage(Driver);
                        }
                    }
                    catch { }
                }
                Thread.Sleep(2000);
            }
            
            Logger.Error("❌ Could not find license button");
            return null;
        }
    }
}
