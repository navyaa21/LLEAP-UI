using OpenQA.Selenium;
using LLEAP.Core;
using LLEAP.Helpers;
using System.Threading;

namespace LLEAP.Pages
{
    public class InstructorAppPage : BasePage
    {
        private readonly By _instructorAppButton = By.Name("Instructor Application");

        public InstructorAppPage(IWebDriver driver) : base(driver) { }

        public LicenseDialogPage ClickInstructorApplication()
        {
            Logger.Step("Clicking Instructor Application");
            Click(_instructorAppButton, Config.AppSettings.ExplicitWaitTimeout);
            Thread.Sleep(3000);
            return new LicenseDialogPage(Driver);
        }
    }
}
