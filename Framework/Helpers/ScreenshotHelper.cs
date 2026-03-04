using OpenQA.Selenium;
using System;
using System.IO;

namespace LLEAP.Helpers
{
    public static class ScreenshotHelper
    {
        private static readonly string ScreenshotDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Screenshots");

        static ScreenshotHelper()
        {
            Directory.CreateDirectory(ScreenshotDir);
        }

        public static string TakeScreenshot(IWebDriver driver, string testName)
        {
            try
            {
                var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
                var fileName = $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                var filePath = Path.Combine(ScreenshotDir, fileName);
                screenshot.SaveAsFile(filePath);
                Logger.Info($"Screenshot saved: {fileName}");
                return filePath;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to take screenshot: {ex.Message}");
                return null;
            }
        }
    }
}
