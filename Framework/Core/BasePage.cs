using OpenQA.Selenium;
using LLEAP.Helpers;
using LLEAP.Config;

namespace LLEAP.Core
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly AppConfig Config;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Config = AppConfig.Load();
        }

        protected IWebElement FindElement(By by, int timeout = 30)
        {
            return WaitHelper.WaitForElement(Driver, by, timeout);
        }

        protected void Click(By by, int timeout = 30)
        {
            WaitHelper.WaitForClickable(Driver, by, timeout)?.Click();
        }

        protected bool IsElementPresent(By by)
        {
            try
            {
                return Driver.FindElement(by) != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
