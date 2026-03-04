using System;
using System.IO;
using System.Text.Json;

namespace LLEAP.Config
{
    public class AppConfig
    {
        public AppSettings AppSettings { get; set; }
        public ElementLocators ElementLocators { get; set; }

        public static AppConfig Load()
        {
            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonSerializer.Deserialize<AppConfig>(json);
            }
            return new AppConfig
            {
                AppSettings = new AppSettings(),
                ElementLocators = new ElementLocators()
            };
        }
    }

    public class AppSettings
    {
        public string AppPath { get; set; } = @"C:\Program Files (x86)\Laerdal Medical\Instructor Application\InstructorApplication\InstructorApplication.exe";
        public string WinAppDriverUrl { get; set; } = "http://127.0.0.1:4723";
        public int ImplicitWaitTimeout { get; set; } = 10;
        public int ExplicitWaitTimeout { get; set; } = 30;
    }

    public class ElementLocators
    {
        public ElementLocator LicenseButton { get; set; } = new ElementLocator();
        public ElementLocator InstructorApp { get; set; } = new ElementLocator();
    }

    public class ElementLocator
    {
        public string Name { get; set; } = "";
        public string AutomationId { get; set; } = "";
        public string ClassName { get; set; } = "Button";
        public string XPath { get; set; } = "";
    }
}
