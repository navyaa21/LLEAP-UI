using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;

namespace LLEAP.Tests.Helpers
{
    public static class WinAppDriverHelper
    {
        public static void EnsureWinAppDriverRunning()
        {
            bool winAppDriverRunning = false;

            try
            {
                using var tcpClient = new TcpClient();
                var result = tcpClient.BeginConnect("127.0.0.1", 4723, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));

                if (success)
                {
                    tcpClient.EndConnect(result);
                    winAppDriverRunning = true;
                }
            }
            catch
            {
            }

            if (!winAppDriverRunning)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Starting WinAppDriver...");

                foreach (var proc in Process.GetProcessesByName("WinAppDriver"))
                {
                    try { proc.Kill(); } catch { }
                }

                Thread.Sleep(1000);

                string winAppDriverPath =
                    @"C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe";

                Process.Start(winAppDriverPath);

                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting for WinAppDriver...");

                for (int i = 1; i <= 15; i++)
                {
                    Thread.Sleep(1000);

                    try
                    {
                        using var tcpClient = new TcpClient();
                        tcpClient.Connect("127.0.0.1", 4723);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] WinAppDriver ready");
                        break;
                    }
                    catch
                    {
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss} [INFO] Waiting... {i}/15");
                    }
                }
            }
            else
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SUCCESS] WinAppDriver already running");
            }
        }

        public static void KillProcessesByName(string processName)
        {
            foreach (var proc in Process.GetProcessesByName(processName))
            {
                try { proc.Kill(); } catch { }
            }
        }

        public static WindowsDriver<WindowsElement> LaunchApplication(string appPath)
        {
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", appPath);
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            options.AddAdditionalCapability("platformName", "Windows");
            options.AddAdditionalCapability("ms:waitForAppLaunch", 30);

            return new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"),
                options);
        }

        public static WindowsDriver<WindowsElement> SwitchToWindow(
            string windowName,
            string processName,
            int timeoutSeconds = 45)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SWITCH] Searching for window: {windowName}");

            var rootOptions = new AppiumOptions();
            rootOptions.AddAdditionalCapability("app", "Root");

            using var rootDriver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"),
                rootOptions);

            string hwnd = null;
            var endTime = DateTime.Now.AddSeconds(timeoutSeconds);

            while (DateTime.Now < endTime)
            {
                try
                {
                    int pid = Process.GetProcessesByName(processName).FirstOrDefault()?.Id ?? 0;
                    var windows = rootDriver.FindElements(By.XPath("//Window"));

                    foreach (var win in windows)
                    {
                        var name = win.GetAttribute("Name")?.ToLower();
                        var processIdText = win.GetAttribute("ProcessId");

                        if (name != null &&
                            name.Contains(windowName.ToLower()) &&
                            processIdText == pid.ToString())
                        {
                            hwnd = int.Parse(win.GetAttribute("NativeWindowHandle")).ToString("x");

                            Console.WriteLine(
                                $"{DateTime.Now:HH:mm:ss} [SWITCH] Found window '{name}' HWND=0x{hwnd}");
                            break;
                        }
                    }

                    if (hwnd != null)
                    {
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{DateTime.Now:HH:mm:ss} [DEBUG] Search error: {ex.Message}");
                }

                Thread.Sleep(1000);
            }

            if (hwnd == null)
            {
                throw new Exception($"Window '{windowName}' not found.");
            }

            var options = new AppiumOptions();
            options.AddAdditionalCapability("appTopLevelWindow", hwnd);

            var newDriver = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"),
                options);

            Console.WriteLine($"{DateTime.Now:HH:mm:ss} [SWITCH] Attached to window {windowName}");

            return newDriver;
        }

        public static void DumpUIElements(WindowsDriver<WindowsElement> driver)
        {
            var elements = driver.FindElements(By.XPath("//*"));

            Console.WriteLine("----- UI ELEMENT DUMP -----");

            foreach (var el in elements)
            {
                try
                {
                    var name = el.GetAttribute("Name");
                    var type = el.TagName;
                    var id = el.GetAttribute("AutomationId");

                    Console.WriteLine($"TYPE={type} NAME={name} ID={id}");
                }
                catch
                {
                }
            }

            Console.WriteLine("----- END UI DUMP -----");
        }

        public static void CleanupDrivers(params WindowsDriver<WindowsElement>[] drivers)
        {
            foreach (var drv in drivers)
            {
                try { drv?.Quit(); } catch { }
            }
        }
    }
}
