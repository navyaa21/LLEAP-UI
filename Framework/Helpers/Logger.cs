using System;

namespace LLEAP.Helpers
{
    public static class Logger
    {
        public static void Info(string message) => Log("INFO", message);
        public static void Step(string message) => Log("STEP", $"→ {message}");
        public static void Success(string message) => Log("SUCCESS", $"✅ {message}");
        public static void Error(string message) => Log("ERROR", $"❌ {message}");
        
        private static void Log(string level, string message)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [{level}] {message}");
        }
    }
}
