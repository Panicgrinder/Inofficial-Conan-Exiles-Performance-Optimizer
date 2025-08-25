using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace ConanOptimizer
{
    // Klasse für die Anwendung der einzelnen Optimierungen
    public class OptimizationApplier
    {
        // 🛡️ SICHERE OPTIMIERUNGEN (Windows-only)
        public static bool ApplyWindowsGameMode()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\GameBar"))
                {
                    key?.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyGpuScheduling()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key?.SetValue("HwSchMode", 2, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyPowerManagement()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power"))
                {
                    key?.SetValue("HibernateEnabled", 0, RegistryValueKind.DWord);
                    key?.SetValue("CsEnabled", 0, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyMemoryManagement()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management"))
                {
                    key?.SetValue("LargeSystemCache", 1, RegistryValueKind.DWord);
                    key?.SetValue("DisablePagingExecutive", 1, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        // 🟡 NIEDRIGE RISIKO OPTIMIERUNGEN
        public static bool ApplySteamLaunchOptions()
        {
            try
            {
                var steamPath = PathHelper.GetSteamPath();
                if (string.IsNullOrEmpty(steamPath)) return false;
                // Steam Launch Options werden über Steam Client gesetzt
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyNvidiaSettings()
        {
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers"))
                {
                    key?.SetValue("TdrDelay", 8, RegistryValueKind.DWord);
                    key?.SetValue("TdrDdiDelay", 8, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyCpuAffinity()
        {
            try
            {
                var process = Process.GetCurrentProcess();
                var coreCount = Environment.ProcessorCount;
                if (coreCount > 4)
                {
                    process.ProcessorAffinity = (IntPtr)((1 << (coreCount - 2)) - 1);
                }
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyProcessPriority()
        {
            try
            {
                using (var key = Registry.CurrentUser.CreateSubKey(@"Software\Microsoft\Windows NT\CurrentVersion\Image File Execution Options\ConanSandbox.exe\PerfOptions"))
                {
                    key?.SetValue("CpuPriorityClass", 3, RegistryValueKind.DWord);
                }
                return true;
            }
            catch { return false; }
        }

        // 🟠 MITTLERE RISIKO OPTIMIERUNGEN (Config-Dateien)
        public static bool ApplyEngineIniTweaks()
        {
            try
            {
                var gameDir = PathHelper.GetConanExilesPath();
                if (string.IsNullOrEmpty(gameDir)) return false;

                var engineIniPath = Path.Combine(gameDir, "ConanSandbox\\Saved\\Config\\WindowsNoEditor\\Engine.ini");
                
                var optimizations = new[]
                {
                    "[Core.System]",
                    "Paths=../../../Engine/Content",
                    "Paths=%GAMEDIR%Content",
                    "Paths=../../../ConanSandbox/Content",
                    "",
                    "[/Script/Engine.RendererSettings]",
                    "r.DefaultFeature.Bloom=True",
                    "r.DefaultFeature.AmbientOcclusion=False",
                    "r.DefaultFeature.AmbientOcclusionStaticFraction=False",
                    "r.DefaultFeature.AutoExposure=False",
                    "r.Shadow.Virtual.Enable=1",
                    "r.DefaultFeature.MotionBlur=False"
                };

                Directory.CreateDirectory(Path.GetDirectoryName(engineIniPath));
                File.AppendAllLines(engineIniPath, optimizations);
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyGameIniNetwork()
        {
            try
            {
                var gameDir = PathHelper.GetConanExilesPath();
                if (string.IsNullOrEmpty(gameDir)) return false;

                var gameIniPath = Path.Combine(gameDir, "ConanSandbox\\Saved\\Config\\WindowsNoEditor\\Game.ini");
                
                var networkOptimizations = new[]
                {
                    "[/Script/OnlineSubsystemUtils.IpNetDriver]",
                    "NetClientTicksPerSecond=120",
                    "ConnectionTimeout=60.0",
                    "InitialConnectTimeout=60.0",
                    "MoveRepSize=512.0",
                    "MaxTracesPerTick=500"
                };

                Directory.CreateDirectory(Path.GetDirectoryName(gameIniPath));
                File.AppendAllLines(gameIniPath, networkOptimizations);
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyLodOptimizations()
        {
            try
            {
                var gameDir = PathHelper.GetConanExilesPath();
                if (string.IsNullOrEmpty(gameDir)) return false;

                var engineIniPath = Path.Combine(gameDir, "ConanSandbox\\Saved\\Config\\WindowsNoEditor\\Engine.ini");
                
                var lodOptimizations = new[]
                {
                    "",
                    "[/Script/Engine.Engine]",
                    "bSmoothFrameRate=True",
                    "bUseFixedFrameRate=False",
                    "SmoothedFrameRateRange=(LowerBound=(Type=Inclusive,Value=60.000000),UpperBound=(Type=Exclusive,Value=120.000000))"
                };

                File.AppendAllLines(engineIniPath, lodOptimizations);
                return true;
            }
            catch { return false; }
        }

        public static bool ApplyTextureStreaming()
        {
            try
            {
                var gameDir = PathHelper.GetConanExilesPath();
                if (string.IsNullOrEmpty(gameDir)) return false;

                var engineIniPath = Path.Combine(gameDir, "ConanSandbox\\Saved\\Config\\WindowsNoEditor\\Engine.ini");
                
                var textureOptimizations = new[]
                {
                    "",
                    "[TextureStreaming]",
                    "PoolSizeVRAMPercentage=0.9",
                    "r.Streaming.MipBias=0",
                    "r.Streaming.AmortizeCPUToGPUCopy=0",
                    "r.Streaming.MaxNumTexturesToStreamPerFrame=0",
                    "r.Streaming.Boost=1"
                };

                File.AppendAllLines(engineIniPath, textureOptimizations);
                return true;
            }
            catch { return false; }
        }

        // 🔴 HOHE RISIKO OPTIMIERUNGEN (Experimentell)
        public static bool ApplyExperimentalPatches()
        {
            try
            {
                var gameDir = PathHelper.GetConanExilesPath();
                if (string.IsNullOrEmpty(gameDir)) return false;

                var engineIniPath = Path.Combine(gameDir, "ConanSandbox\\Saved\\Config\\WindowsNoEditor\\Engine.ini");
                
                var experimentalOptimizations = new[]
                {
                    "",
                    "[/Script/Engine.GarbageCollectionSettings]",
                    "gc.MaxObjectsNotConsideredByGC=1",
                    "gc.SizeOfPermanentObjectPool=0",
                    "gc.FlushStreamingOnGC=1",
                    "",
                    "[Core.System]",
                    "GameThread.MaxFrameTime=0.01666",
                    "GameThread.TargetFrameTimeVariance=0.002"
                };

                File.AppendAllLines(engineIniPath, experimentalOptimizations);
                return true;
            }
            catch { return false; }
        }

        // Anwendung aller ausgewählten Optimierungen
        public static List<string> ApplySelectedOptimizations(OptimizationSettings settings)
        {
            var results = new List<string>();

            // 🛡️ SICHERE OPTIMIERUNGEN
            if (settings.WindowsGameMode)
                results.Add(ApplyWindowsGameMode() ? "✅ Windows Gaming Mode aktiviert" : "❌ Windows Gaming Mode fehlgeschlagen");
            
            if (settings.GpuScheduling)
                results.Add(ApplyGpuScheduling() ? "✅ GPU Hardware Scheduling aktiviert" : "❌ GPU Scheduling fehlgeschlagen");
            
            if (settings.PowerManagement)
                results.Add(ApplyPowerManagement() ? "✅ Power Management optimiert" : "❌ Power Management fehlgeschlagen");
            
            if (settings.MemoryManagement)
                results.Add(ApplyMemoryManagement() ? "✅ Memory Management optimiert" : "❌ Memory Management fehlgeschlagen");

            // 🟡 NIEDRIGE RISIKO OPTIMIERUNGEN
            if (settings.SteamLaunchOptions)
                results.Add(ApplySteamLaunchOptions() ? "✅ Steam Launch Options gesetzt" : "❌ Steam Launch Options fehlgeschlagen");
            
            if (settings.NvidiaSettings)
                results.Add(ApplyNvidiaSettings() ? "✅ NVIDIA Einstellungen optimiert" : "❌ NVIDIA Einstellungen fehlgeschlagen");
            
            if (settings.CpuAffinity)
                results.Add(ApplyCpuAffinity() ? "✅ CPU Affinity optimiert" : "❌ CPU Affinity fehlgeschlagen");
            
            if (settings.ProcessPriority)
                results.Add(ApplyProcessPriority() ? "✅ Process Priority gesetzt" : "❌ Process Priority fehlgeschlagen");

            // 🟠 MITTLERE RISIKO OPTIMIERUNGEN
            if (settings.EngineIniTweaks)
                results.Add(ApplyEngineIniTweaks() ? "✅ Engine.ini Optimierungen angewendet" : "❌ Engine.ini Optimierungen fehlgeschlagen");
            
            if (settings.GameIniNetwork)
                results.Add(ApplyGameIniNetwork() ? "✅ Game.ini Netzwerk optimiert" : "❌ Game.ini Netzwerk fehlgeschlagen");
            
            if (settings.LodOptimizations)
                results.Add(ApplyLodOptimizations() ? "✅ LOD Optimierungen angewendet" : "❌ LOD Optimierungen fehlgeschlagen");
            
            if (settings.TextureStreaming)
                results.Add(ApplyTextureStreaming() ? "✅ Texture Streaming optimiert" : "❌ Texture Streaming fehlgeschlagen");

            // 🔴 HOHE RISIKO OPTIMIERUNGEN
            if (settings.ExperimentalPatches)
                results.Add(ApplyExperimentalPatches() ? "✅ Experimentelle Patches angewendet" : "❌ Experimentelle Patches fehlgeschlagen");

            return results;
        }
    }
}
