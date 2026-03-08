using System.IO;
using Microsoft.Extensions.Configuration;

namespace LLEAP.Framework.Config
{
    public static class AppConfig
    {
        private static readonly IConfigurationRoot Configuration;

        static AppConfig()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();
        }

        public static string InstructorApp =>
            Configuration["Paths:InstructorApp"];

        public static string SimulationHome =>
            Configuration["Paths:SimulationHome"];

        public static string WinAppDriver =>
            Configuration["Paths:WinAppDriver"];
    }
}