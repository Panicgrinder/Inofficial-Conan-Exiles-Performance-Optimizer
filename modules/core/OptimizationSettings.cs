using System;

namespace ConanOptimizer
{
    // Optimierungseinstellungen für die erweiterte Version
    public class OptimizationSettings
    {
        // 🛡️ SICHERE OPTIMIERUNGEN (0% Ban-Risiko)
        public bool WindowsGameMode { get; set; }
        public bool GpuScheduling { get; set; }
        public bool PowerManagement { get; set; }
        public bool MemoryManagement { get; set; }

        // 🟡 NIEDRIGE RISIKO OPTIMIERUNGEN (1% Ban-Risiko)
        public bool SteamLaunchOptions { get; set; }
        public bool NvidiaSettings { get; set; }
        public bool CpuAffinity { get; set; }
        public bool ProcessPriority { get; set; }

        // 🟠 MITTLERE RISIKO OPTIMIERUNGEN (5-10% Ban-Risiko)
        public bool EngineIniTweaks { get; set; }
        public bool GameIniNetwork { get; set; }
        public bool LodOptimizations { get; set; }
        public bool TextureStreaming { get; set; }

        // 🔴 HOHE RISIKO OPTIMIERUNGEN (15-30% Ban-Risiko)
        public bool ExperimentalPatches { get; set; }

        public OptimizationSettings()
        {
            // Standard: Nur sichere Optimierungen aktiviert
            WindowsGameMode = true;
            GpuScheduling = true;
            PowerManagement = true;
            MemoryManagement = true;
        }

        // Vordefinierte Preset-Konfigurationen
        public static OptimizationSettings GetOfficialServerSafe()
        {
            return new OptimizationSettings(); // Nur sichere Einstellungen
        }

        public static OptimizationSettings GetPrivateServerOptimized()
        {
            var settings = new OptimizationSettings();
            // Sichere + niedrige Risiko Optimierungen
            settings.SteamLaunchOptions = true;
            settings.NvidiaSettings = true;
            settings.CpuAffinity = true;
            settings.ProcessPriority = true;
            return settings;
        }

        public static OptimizationSettings GetSingleplayerMaximum()
        {
            var settings = GetPrivateServerOptimized();
            // Zusätzlich mittlere Risiko Optimierungen
            settings.EngineIniTweaks = true;
            settings.GameIniNetwork = true;
            settings.LodOptimizations = true;
            settings.TextureStreaming = true;
            return settings;
        }

        public static OptimizationSettings GetModdingAndTesting()
        {
            var settings = GetSingleplayerMaximum();
            // Alle Optimierungen inklusive experimentelle
            settings.ExperimentalPatches = true;
            return settings;
        }
    }
}
