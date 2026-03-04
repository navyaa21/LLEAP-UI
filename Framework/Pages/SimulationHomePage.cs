using OpenQA.Selenium;
using LLEAP.Core;
using LLEAP.Helpers;
using System.Threading;

namespace LLEAP.Pages
{
    public class SimulationHomePage : BasePage
    {
        private readonly By _instructorApp = By.Name("Instructor Application");
        private readonly By _localComputer = By.Name("Local Computer");
        private readonly By _simMan3GPlus = By.Name("SimMan3G Plus");
        private readonly By _manualMode = By.Name("Manual Mode");
        private readonly By _themesList = By.Name("Themes");
        private readonly By _healthyTheme = By.Name("Healthy Patient");
        private readonly By _okButton = By.XPath("//Button[@Name='OK']");
        private readonly By _startSession = By.Name("Start Session");
        private readonly By _maximize = By.Name("Maximize");
        private readonly By _eyes = By.Name("Eyes");
        private readonly By _eyesClosed = By.Name("Closed");
        private readonly By _lungSlider = By.Name("Lung compliance");
        private readonly By _hrValue = By.Name("HR");
        private readonly By _voices = By.Name("Voices");
        private readonly By _coughing = By.Name("Coughing");
        private readonly By _play = By.Name("Play");

        public SimulationHomePage(IWebDriver driver) : base(driver) { }

        public void ClickInstructorApp()
        {
            Logger.Step("Clicking Instructor Application");
            Click(_instructorApp, 30);
            Thread.Sleep(3000);
        }

        public bool ClickLicenseButton()
        {
            Logger.Step("Looking for license button");
            
            string[] strategies = {
                "//Button[contains(@Name, 'Add license later')]",
                "//Button[contains(@Name, 'Add') and contains(@Name, 'license')]",
                "//*[contains(@Name, 'Add license later')]",
                "//Window//Button[contains(@Name, 'Add')]",
                "//Button[contains(@Name, 'License')]"
            };

            for (int attempt = 1; attempt <= 5; attempt++)
            {
                Logger.Info($"Attempt {attempt}/5");
                
                foreach (string xpath in strategies)
                {
                    try
                    {
                        var button = WaitHelper.WaitForClickable(Driver, By.XPath(xpath), 3);
                        if (button != null)
                        {
                            button.Click();
                            Logger.Success("✅ License button clicked!");
                            Thread.Sleep(2000);
                            return true;
                        }
                    }
                    catch { }
                }
                Thread.Sleep(2000);
            }
            
            Logger.Error("❌ Could not find license button");
            return false;
        }

        public void ClickLocalComputer() { Click(_localComputer, 30); Thread.Sleep(1000); }
        public void ClickSimMan3GPlus() { Click(_simMan3GPlus, 30); Thread.Sleep(1000); }
        public void ClickManualMode() { Click(_manualMode, 30); Thread.Sleep(2000); }
        public void ClickThemesList() { Click(_themesList, 30); Thread.Sleep(1000); }
        public void ClickHealthyTheme() { Click(_healthyTheme, 30); Thread.Sleep(1000); }
        public void ClickOkButton() { Click(_okButton, 30); Thread.Sleep(2000); }
        public void ClickStartSession() { Click(_startSession, 30); Thread.Sleep(3000); }
        public void ClickMaximize() { try { Click(_maximize, 10); } catch { } Thread.Sleep(1000); }
        public void ClickEyes() { Click(_eyes, 30); Thread.Sleep(500); }
        public void ClickEyesClosed() { Click(_eyesClosed, 30); Thread.Sleep(1000); }
        
        public void SetHeartRateTo100()
        {
            var hr = WaitHelper.WaitForElement(Driver, _hrValue, 30);
            hr.Click();
            hr.Clear();
            hr.SendKeys("100");
            hr.SendKeys(Keys.Enter);
            Thread.Sleep(1000);
        }
        
        public void PlayCoughing()
        {
            Click(_voices, 30);
            Thread.Sleep(500);
            Click(_coughing, 10);
            Thread.Sleep(500);
            Click(_play, 10);
            Thread.Sleep(1000);
        }
        
        public void CloseApp()
        {
            try { Driver.Close(); }
            catch { 
                var actions = new OpenQA.Selenium.Interactions.Actions(Driver);
                actions.SendKeys(Keys.Alt + Keys.F4).Perform();
            }
        }
    }
}
