using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;

namespace ConanOptimizer
{
	// Helper-Klasse für Pfad-Operationen (Kopie der Modul-Implementierung)
	public static class PathHelper
	{
		public static string GetSteamPath()
		{
			try
			{
				// 1) HKCU bevorzugen
				using (var key = Registry.CurrentUser.OpenSubKey(@"Software\\Valve\\Steam"))
				{
					var steamPath = key?.GetValue("InstallPath")?.ToString();
					if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
						return steamPath;
				}
				// 2) HKLM Fallback (32-bit node)
				using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\WOW6432Node\\Valve\\Steam"))
				{
					var steamPath = key?.GetValue("InstallPath")?.ToString();
					if (!string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath))
						return steamPath;
				}
			}
			catch { }

			var steamPaths = new[]
			{
				@"C:\\Program Files (x86)\\Steam",
				@"C:\\Program Files\\Steam",
				@"D:\\Steam",
				@"E:\\Steam",
				@"F:\\Steam",
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
					// Zuerst Standard-Library unter Installationspfad
					var defaultCommon = Path.Combine(steamPath, "steamapps", "common");
					var conanPath = Path.Combine(defaultCommon, "Conan Exiles");
					if (Directory.Exists(conanPath)) return conanPath;

					// Danach alle Libraries aus libraryfolders.vdf
					foreach (var lib in EnumerateSteamLibraries(steamPath))
					{
						var candidate = Path.Combine(lib, "common", "Conan Exiles");
						if (Directory.Exists(candidate)) return candidate;
					}
				}

				var fallbackPaths = new[]
				{
					@"C:\\Program Files (x86)\\Steam\\steamapps\\common\\Conan Exiles",
					@"C:\\Program Files\\Steam\\steamapps\\common\\Conan Exiles",
					@"D:\\Steam\\steamapps\\common\\Conan Exiles",
					@"E:\\Steam\\steamapps\\common\\Conan Exiles",
					@"F:\\Steam\\steamapps\\common\\Conan Exiles"
				};

				return fallbackPaths.FirstOrDefault(Directory.Exists);
			}
			catch
			{
				return null;
			}
		}

		public static string GetConanSavedConfigPath()
		{
			// Typischer UE4-Client-Speicherpfad (manche Installationen schreiben in den Game-Ordner)
			try
			{
				var localApp = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
				// In Conan ist der Projektname ConanSandbox
				var path = Path.Combine(localApp, "ConanSandbox", "Saved", "Config", "WindowsNoEditor");
				if (Directory.Exists(path)) return path;

				// Fallback: im Installationsordner
				var game = GetConanExilesPath();
				if (!string.IsNullOrEmpty(game))
				{
					var alt = Path.Combine(game, "ConanSandbox", "Saved", "Config", "WindowsNoEditor");
					if (Directory.Exists(alt)) return alt;
				}
			}
			catch { }
			return null;
		}

		private static IEnumerable<string> EnumerateSteamLibraries(string steamPath)
		{
			var libs = new List<string>();
			try
			{
				var vdf = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
				if (!File.Exists(vdf)) return libs;

				// Sehr einfache Parser-Variante: alle Zeilen mit "path" herausholen
				foreach (var line in File.ReadAllLines(vdf))
				{
					var idx = line.IndexOf("\"path\"", StringComparison.OrdinalIgnoreCase);
					if (idx >= 0)
					{
						var parts = line.Split('"');
						var path = parts.LastOrDefault(p => p.Contains(":\\") || p.Contains("/"));
						if (!string.IsNullOrWhiteSpace(path))
						{
							var normalized = path.Replace("/", "\\");
							var steamapps = Path.Combine(normalized, "steamapps");
							if (Directory.Exists(steamapps)) libs.Add(steamapps);
						}
					}
				}
			}
			catch { }
			return libs.Distinct(StringComparer.OrdinalIgnoreCase);
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

	// Korrekt geschriebener Alias für Klarheit
	public static string GetConanExecutablePath() => GetConanExeutablePath();
	}
}

