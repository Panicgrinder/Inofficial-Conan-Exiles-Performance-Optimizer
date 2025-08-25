using System;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace ConanOptimizer
{
    // Helper-Klasse für Pfad-Operationen
    public static class PathHelper
    {
        public static string GetSteamPath()
        {
            try
            {
                // Versuche Steam-Pfad aus Registry zu lesen
                using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Valve\Steam"))
                {
                    var steamPath = key?.GetValue("InstallPath")?.ToString();
                    if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
                        return steamPath;
                }
            }
            catch { }

            // Fallback zu Standard-Pfaden
            var steamPaths = new[]
            {
                @"C:\Program Files (x86)\Steam",
                @"C:\Program Files\Steam",
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam")
            };

            return steamPaths.FirstOrDefault(Directory.Exists);
        }

        public static string GetConanExilesPath()
        {
            try
            {
                var steamPath = GetSteamPath();
                if (!string.IsNullOrEmpty(steamPath))
                {
                    var conanPath = Path.Combine(steamPath, "steamapps", "common", "Conan Exiles");
                    if (Directory.Exists(conanPath))
                        return conanPath;
                }

                // Fallback zu Standard-Pfaden
                var fallbackPaths = new[]
                {
                    @"C:\Program Files (x86)\Steam\steamapps\common\Conan Exiles",
                    @"C:\Program Files\Steam\steamapps\common\Conan Exiles",
                    @"D:\Steam\steamapps\common\Conan Exiles",
                    @"E:\Steam\steamapps\common\Conan Exiles"
                };

                return fallbackPaths.FirstOrDefault(Directory.Exists);
            }
            catch
            {
                return null;
            }
        }

        public static bool IsConanExilesInstalled()
        {
            return !string.IsNullOrEmpty(GetConanExilesPath());
        }

        public static string GetConanExeutablePath()
        {
            var gamePath = GetConanExilesPath();
            if (string.IsNullOrEmpty(gamePath)) return null;

            var exePath = Path.Combine(gamePath, "ConanSandbox", "Binaries", "Win64", "ConanSandbox.exe");
            return File.Exists(exePath) ? exePath : null;
        }
    }
}
