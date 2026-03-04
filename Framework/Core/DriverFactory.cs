using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using LLEAP.Config;
using LLEAP.Helpers;

namespace LLEAP.Core
{
    public static class DriverFactory
    {
        public static WindowsDriver<WindowsElement> CreateDriver()
        {
            var config = AppConfig.Load();
            
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", config.AppSettings.AppPath);
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("ms:experimental-webdriver", true);

            var driver = new WindowsDriver<WindowsElement>(
                new Uri(config.AppSettings.WinAppDriverUrl), 
                options
            );
            
            driver.Manage().Timeouts().ImplicitWait = 
                TimeSpan.FromSeconds(config.AppSettings.ImplicitWaitTimeout);
            
            return driver;
        }
    }
}
