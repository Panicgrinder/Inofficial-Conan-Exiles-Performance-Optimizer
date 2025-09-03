using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConanOptimizer
{
    // Repariert problematische AI/Netzwerk-Overrides in Engine.ini und Game.ini
    public static class ConfigRepair
    {
        public static IEnumerable<string> ScanForNpcAiOverrides(out string engineIniPath, out string gameIniPath)
        {
            var findings = new List<string>();
            engineIniPath = null;
            gameIniPath = null;

            try
            {
                // Bevorzugt den UE4 Saved-Config-Pfad unter LocalAppData
                var savedConfig = PathHelper.GetConanSavedConfigPath();
                if (!string.IsNullOrEmpty(savedConfig))
                {
                    engineIniPath = Path.Combine(savedConfig, "Engine.ini");
                    gameIniPath = Path.Combine(savedConfig, "Game.ini");
                }
                else
                {
                    // Fallback: Pfad unterhalb der Installation
                    var gameDir = PathHelper.GetConanExilesPath();
                    if (string.IsNullOrEmpty(gameDir))
                    {
                        findings.Add("Conan Exiles Pfad nicht gefunden.");
                        return findings;
                    }
                    engineIniPath = Path.Combine(gameDir, @"ConanSandbox\Saved\Config\WindowsNoEditor\Engine.ini");
                    gameIniPath = Path.Combine(gameDir, @"ConanSandbox\Saved\Config\WindowsNoEditor\Game.ini");
                }

                findings.AddRange(ScanFile(engineIniPath, "Engine.ini"));
                findings.AddRange(ScanFile(gameIniPath, "Game.ini"));
            }
            catch (Exception ex)
            {
                findings.Add($"Scan-Fehler: {ex.Message}");
            }
            return findings;
        }

        public static bool RepairNpcAiOverrides()
        {
            try
            {
                string engineIniPath;
                string gameIniPath;

                var savedConfig = PathHelper.GetConanSavedConfigPath();
                if (!string.IsNullOrEmpty(savedConfig))
                {
                    engineIniPath = Path.Combine(savedConfig, "Engine.ini");
                    gameIniPath = Path.Combine(savedConfig, "Game.ini");
                }
                else
                {
                    var gameDir = PathHelper.GetConanExilesPath();
                    if (string.IsNullOrEmpty(gameDir)) return false;
                    engineIniPath = Path.Combine(gameDir, @"ConanSandbox\Saved\Config\WindowsNoEditor\Engine.ini");
                    gameIniPath = Path.Combine(gameDir, @"ConanSandbox\Saved\Config\WindowsNoEditor\Game.ini");
                }

                bool ok1 = RepairFile(engineIniPath, suspiciousSectionsEngine, suspiciousKeysEngine);
                bool ok2 = RepairFile(gameIniPath, suspiciousSectionsGame, suspiciousKeysGame);
                return ok1 && ok2;
            }
            catch
            {
                return false;
            }
        }

        private static readonly string[] suspiciousSectionsEngine = new[]
        {
            // Experimentelle/risikoreiche Engine-Abschnitte, die Animation/Threading beeinflussen können
            @"\[/Script/Engine\.GarbageCollectionSettings\]",
            @"\[Core\.System\]"
        };

        private static readonly string[] suspiciousKeysEngine = new[]
        {
            @"^\s*GameThread\.MaxFrameTime\s*=",
            @"^\s*GameThread\.TargetFrameTimeVariance\s*=",
            @"^\s*gc\.MaxObjectsNotConsideredByGC\s*=",
            @"^\s*gc\.SizeOfPermanentObjectPool\s*=",
            @"^\s*gc\.FlushStreamingOnGC\s*="
        };

        private static readonly string[] suspiciousSectionsGame = new[]
        {
            // Netzwerk/AI-Abschnitte, die häufig Glitches erzeugen
            @"\[/Script/Engine\.GameNetworkManager\]",
            @"\[/Script/ConanSandbox\.AISense_NewSight\]",
            @"\[/Script/AIModule\.EnvQueryManager\]",
            @"\[/Script/OnlineSubsystemUtils\.IpNetDriver\]"
        };

        private static readonly string[] suspiciousKeysGame = new[]
        {
            @"^\s*MaxTracesPerTick\s*=",
            @"^\s*MoveRepSize\s*=",
            @"^\s*MAXPOSITIONERRORSQUARED\s*=",
            @"^\s*MaxAllowedTestingTime\s*=",
            @"^\s*NetClientTicksPerSecond\s*=",
            @"^\s*TotalNetBandwidth\s*=",
            @"^\s*MaxDynamicBandwidth\s*=",
            @"^\s*MinDynamicBandwidth\s*="
        };

        private static IEnumerable<string> ScanFile(string path, string label)
        {
            var hits = new List<string>();
            if (!File.Exists(path)) return hits;

            var lines = File.ReadAllLines(path);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                string trimmed = line.Trim();

                if (Regex.IsMatch(trimmed, @"^\[.*\]$"))
                {
                    if (suspiciousSectionsEngine.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase)))
                        hits.Add($"{label}: Zeile {i + 1}: Abschnitt verdächtig: {trimmed}");
                    if (suspiciousSectionsGame.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase)))
                        hits.Add($"{label}: Zeile {i + 1}: Abschnitt verdächtig: {trimmed}");
                }

                if (suspiciousKeysEngine.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase)))
                    hits.Add($"{label}: Zeile {i + 1}: Schlüssel verdächtig: {trimmed}");
                if (suspiciousKeysGame.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase)))
                    hits.Add($"{label}: Zeile {i + 1}: Schlüssel verdächtig: {trimmed}");
            }
            return hits;
        }

        private static bool RepairFile(string path, string[] sectionPatterns, string[] keyPatterns)
        {
            try
            {
                if (!File.Exists(path)) return true; // nichts zu tun

                var backup = path + $".backup-npcfix-{DateTime.Now:yyyyMMddHHmmss}";
                File.Copy(path, backup, overwrite: false);

                var lines = File.ReadAllLines(path).ToList();
                var outLines = new List<string>(lines.Count);
                bool skipSection = false;

                foreach (var raw in lines)
                {
                    string line = raw;
                    string trimmed = line.Trim();

                    // Abschnittsanfang?
                    if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
                    {
                        // Bei neuem Abschnitt neu bewerten
                        skipSection = sectionPatterns.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase));
                        if (skipSection)
                        {
                            // Abschnitt-Header auslassen und alle folgenden Zeilen bis zum nächsten Abschnitt skippen
                            continue;
                        }
                        outLines.Add(line);
                        continue;
                    }

                    if (skipSection)
                    {
                        // Zeilen innerhalb verdächtiger Sektion überspringen
                        continue;
                    }

                    // Einzelne verdächtige Schlüssel entfernen
                    bool keyHit = keyPatterns.Any(p => Regex.IsMatch(trimmed, p, RegexOptions.IgnoreCase));
                    if (keyHit)
                        continue;

                    outLines.Add(line);
                }

                File.WriteAllLines(path, outLines);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
