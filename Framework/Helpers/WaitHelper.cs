using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;

namespace LLEAP.Helpers
{
    public static class WaitHelper
    {
        public static IWebElement WaitForElement(IWebDriver driver, By by, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(d => d.FindElement(by));
        }

        public static IWebElement WaitForClickable(IWebDriver driver, By by, int timeoutInSeconds = 30)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            return wait.Until(d =>
            {
                try
                {
                    var element = d.FindElement(by);
                    return element != null && element.Enabled ? element : null;
                }
                catch { return null; }
            });
        }

        public static bool WaitForDialog(IWebDriver driver, string dialogName, int timeoutInSeconds = 10)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
            try
            {
                return wait.Until(d =>
                {
                    var dialogs = d.FindElements(By.XPath("//Window"));
                    foreach (var dialog in dialogs)
                    {
                        if (dialog.Text.Contains(dialogName) || dialog.GetAttribute("Name").Contains(dialogName))
                            return true;
                    }
                    return false;
                });
            }
            catch { return false; }
        }
    }
}
