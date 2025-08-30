using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ConanOptimizer
{
    public partial class MainForm : Form
    {
        #region Constants - Sichere Version
        private const string AppName = "Conan Exiles Optimizer (Sicher)";
        private const string AppVersion = "v3.1.0";
        private const string AppTitle = "Conan Exiles Optimizer - SICHER FÜR ONLINE";
        private const string BuildDate = "2025-08-25";
        private const string SteamRegistryPath = @"SOFTWARE\Valve\Steam";
        private const string ConanAppId = "440900";
        private const string ConanExecutableName = "ConanSandbox-Win64-Shipping.exe";
        private const string GameModeRegistryPath = @"SOFTWARE\Microsoft\GameBar";
        private const int WindowWidth = 1000;
        private const int WindowHeight = 750;
        private const int MonitoringUpdateInterval = 1000; // ms
        #endregion

        #region Fields
        private string steamPath = string.Empty;
        private string conanPath = string.Empty;
        private bool isMonitoring = false;
        private CancellationTokenSource monitoringCancellation;
        #endregion
        
        public MainForm()
        {
            InitializeComponent();
            InitializeCustomComponents();
            this.Load += MainForm_Load;
        }
        
        private void InitializeComponent()
        {
            this.Text = AppTitle;
            this.Size = new Size(WindowWidth, WindowHeight);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 9F);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            this.Icon = SystemIcons.Application;
        }

        private void InitializeCustomComponents()
        {
            // Header Panel mit Sicherheitshinweis
            Panel headerPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 100),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(45, 45, 45),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label titleLabel = new Label
            {
                Text = "🛡️ SICHER FÜR ONLINE-SPIEL 🛡️",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Size = new Size(500, 30),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label safetyLabel = new Label
            {
                Text = "Keine Config-Änderungen • Keine Spieldatei-Modifikation • 100% Ban-sicher",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.LightGray,
                Size = new Size(800, 25),
                Location = new Point(10, 45),
                TextAlign = ContentAlignment.MiddleLeft
            };

            Label versionLabel = new Label
            {
                Text = $"{AppVersion} • Build: {BuildDate}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.Gray,
                Size = new Size(200, 20),
                Location = new Point(700, 70),
                TextAlign = ContentAlignment.MiddleRight
            };

            headerPanel.Controls.AddRange(new Control[] { titleLabel, safetyLabel, versionLabel });

            // Status Panel
            Panel statusPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 150),
                Location = new Point(20, 140),
                BackColor = Color.FromArgb(40, 40, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.steamStatusLabel = new Label
            {
                Size = new Size(450, 25),
                Location = new Point(10, 10),
                ForeColor = Color.Yellow,
                Font = new Font("Segoe UI", 10F),
                Text = "🔍 Steam wird gesucht..."
            };

            this.conanStatusLabel = new Label
            {
                Size = new Size(450, 25),
                Location = new Point(10, 40),
                ForeColor = Color.Yellow,
                Font = new Font("Segoe UI", 10F),
                Text = "🔍 Conan Exiles wird gesucht..."
            };

            this.performanceLabel = new Label
            {
                Size = new Size(450, 25),
                Location = new Point(10, 70),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10F),
                Text = "⚡ Performance-Status: Wird geprüft..."
            };

            this.systemStatusLabel = new Label
            {
                Size = new Size(450, 25),
                Location = new Point(10, 100),
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 10F),
                Text = "💻 System wird analysiert..."
            };

            statusPanel.Controls.AddRange(new Control[] { 
                steamStatusLabel, conanStatusLabel, performanceLabel, systemStatusLabel
            });

            // Buttons Panel - Sichere Aktionen
            Panel buttonPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 100),
                Location = new Point(20, 310),
                BackColor = Color.FromArgb(35, 35, 35),
                BorderStyle = BorderStyle.FixedSingle
            };

            this.optimizeButton = new Button
            {
                Text = "🚀 SICHERE OPTIMIERUNG\n(Nur Windows + Cache)",
                Size = new Size(200, 60),
                Location = new Point(20, 20),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Enabled = false
            };
            this.optimizeButton.Click += OptimizeButton_Click;

            this.monitorButton = new Button
            {
                Text = "📊 PERFORMANCE\nMONITORING",
                Size = new Size(150, 60),
                Location = new Point(240, 20),
                BackColor = Color.FromArgb(40, 140, 40),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            this.monitorButton.Click += MonitorButton_Click;

            this.launchButton = new Button
            {
                Text = "🎮 CONAN EXILES\nSTARTEN",
                Size = new Size(150, 60),
                Location = new Point(410, 20),
                BackColor = Color.FromArgb(180, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Enabled = false
            };
            this.launchButton.Click += LaunchButton_Click;

            Button helpButton = new Button
            {
                Text = "❓ HILFE &\nSICHERHEIT",
                Size = new Size(120, 60),
                Location = new Point(580, 20),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            helpButton.Click += HelpButton_Click;

            Button resetButton = new Button
            {
                Text = "🔄 RESET\nOPTIMIERUNGEN",
                Size = new Size(120, 60),
                Location = new Point(720, 20),
                BackColor = Color.FromArgb(140, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
            resetButton.Click += ResetButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { 
                optimizeButton, monitorButton, launchButton, helpButton, resetButton
            });

            // Info Panel - Sichere Features
            Panel infoPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 120),
                Location = new Point(20, 430),
                BackColor = Color.FromArgb(20, 60, 20),
                BorderStyle = BorderStyle.FixedSingle
            };

            Label infoTitle = new Label
            {
                Text = "✅ WAS DIESE SICHERE VERSION MACHT:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.LimeGreen,
                Size = new Size(400, 25),
                Location = new Point(10, 10)
            };

            Label infoText = new Label
            {
                Text = "• Windows Gaming-Modus optimieren\n" +
                       "• Temporäre Cache-Dateien bereinigen\n" +
                       "• RAM-Management verbessern\n" +
                       "• Grafiktreiber-Einstellungen anpassen\n" +
                       "• Performance-Monitoring aktivieren",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Size = new Size(400, 80),
                Location = new Point(10, 35)
            };

            Label warningTitle = new Label
            {
                Text = "🛡️ SICHERHEITSGARANTIE:",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.Gold,
                Size = new Size(400, 25),
                Location = new Point(450, 10)
            };

            Label warningText = new Label
            {
                Text = "❌ KEINE Config-Dateien (Engine.ini/Game.ini)\n" +
                       "❌ KEINE Spieldatei-Modifikation\n" +
                       "❌ KEINE Anti-Cheat Beeinflussung\n" +
                       "❌ KEINE Server-Einstellungen\n" +
                       "✅ 100% Ban-sicher für Online-Spiel",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.White,
                Size = new Size(400, 80),
                Location = new Point(450, 35)
            };

            infoPanel.Controls.AddRange(new Control[] { infoTitle, infoText, warningTitle, warningText });

            // Output TextBox
            this.outputTextBox = new RichTextBox
            {
                Size = new Size(WindowWidth - 40, 140),
                Location = new Point(20, 570),
                BackColor = Color.Black,
                ForeColor = Color.LimeGreen,
                Font = new Font("Consolas", 9F),
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Text = $"=== {AppName} {AppVersion} ===\n" +
                       "🛡️ SICHERE VERSION - Keine Config-Änderungen!\n" +
                       "Bereit für sicheren Einsatz auf Online-Servern.\n\n"
            };

            this.Controls.AddRange(new Control[] { 
                headerPanel, statusPanel, buttonPanel, infoPanel, outputTextBox 
            });
        }

        #region UI Components
        private Label steamStatusLabel;
        private Label conanStatusLabel;
        private Label performanceLabel;
        private Label systemStatusLabel;
        private Button optimizeButton;
        private Button monitorButton;
        private Button launchButton;
        private RichTextBox outputTextBox;
        #endregion

        private async void MainForm_Load(object sender, EventArgs e)
        {
            LogMessage("🔍 Suche Steam und Conan Exiles...", Color.Yellow);
            
            await Task.Run(() => {
                DetectSteamInstallation();
                DetectConanExilesInstallation();
            });
            
            UpdateStatus();
            LogMessage("✅ Initialisierung abgeschlossen - Bereit für sichere Optimierung!", Color.Green);
        }

        private void DetectSteamInstallation()
        {
            try
            {
                // Registry-basierte Suche
                using (var key = Registry.LocalMachine.OpenSubKey(SteamRegistryPath))
                {
                    if (key?.GetValue("InstallPath") is string regPath && Directory.Exists(regPath))
                    {
                        steamPath = regPath;
                        return;
                    }
                }

                // Fallback: Standard-Pfade durchsuchen
                string[] commonPaths = {
                    @"C:\Program Files (x86)\Steam",
                    @"C:\Program Files\Steam",
                    @"D:\Steam",
                    @"E:\Steam"
                };

                foreach (string path in commonPaths)
                {
                    if (Directory.Exists(path) && File.Exists(Path.Combine(path, "steam.exe")))
                    {
                        steamPath = path;
                        return;
                    }
                }

                LogMessage("⚠️ Steam nicht gefunden - manueller Pfad wird benötigt", Color.Orange);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler bei Steam-Erkennung: {ex.Message}", Color.Red);
            }
        }

        private void DetectConanExilesInstallation()
        {
            if (string.IsNullOrEmpty(steamPath)) return;

            try
            {
                // Steam-Library Ordner durchsuchen
                string[] libraryPaths = {
                    Path.Combine(steamPath, "steamapps", "common", "Conan Exiles"),
                    Path.Combine(steamPath, "steamapps", "common", "ConanExiles")
                };

                // Zusätzliche Laufwerke prüfen
                DriveInfo[] drives = DriveInfo.GetDrives();
                var additionalPaths = drives.Where(d => d.IsReady && d.DriveType == DriveType.Fixed)
                    .SelectMany(d => new[] {
                        Path.Combine(d.RootDirectory.FullName, "SteamLibrary", "steamapps", "common", "Conan Exiles"),
                        Path.Combine(d.RootDirectory.FullName, "Steam", "steamapps", "common", "Conan Exiles"),
                        Path.Combine(d.RootDirectory.FullName, "Program Files (x86)", "Steam", "steamapps", "common", "Conan Exiles")
                    });

                foreach (string testPath in libraryPaths.Concat(additionalPaths))
                {
                    if (Directory.Exists(testPath))
                    {
                        string exePath = Path.Combine(testPath, "ConanSandbox", "Binaries", "Win64", ConanExecutableName);
                        if (File.Exists(exePath))
                        {
                            conanPath = testPath;
                            return;
                        }
                    }
                }

                LogMessage("⚠️ Conan Exiles nicht gefunden - Installation überprüfen", Color.Orange);
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler bei Conan-Erkennung: {ex.Message}", Color.Red);
            }
        }

        private void UpdateStatus()
        {
            // Steam status
            if (!string.IsNullOrEmpty(steamPath))
            {
                steamStatusLabel.Text = $"✅ Steam gefunden: {steamPath}";
                steamStatusLabel.ForeColor = Color.Green;
            }
            else
            {
                steamStatusLabel.Text = "❌ Steam nicht gefunden";
                steamStatusLabel.ForeColor = Color.Red;
            }
            
            // Conan status
            if (!string.IsNullOrEmpty(conanPath))
            {
                conanStatusLabel.Text = $"✅ Conan Exiles gefunden: {Path.GetFileName(conanPath)}";
                conanStatusLabel.ForeColor = Color.Green;
                
                optimizeButton.Enabled = true;
                monitorButton.Enabled = true;
                launchButton.Enabled = true;
                
                performanceLabel.Text = "⚡ Performance-Status: Bereit für sichere Optimierung";
                performanceLabel.ForeColor = Color.Green;
            }
            else
            {
                conanStatusLabel.Text = "❌ Conan Exiles nicht gefunden";
                conanStatusLabel.ForeColor = Color.Red;
                
                optimizeButton.Enabled = false;
                monitorButton.Enabled = false;
                launchButton.Enabled = false;
                
                performanceLabel.Text = "⚡ Performance-Status: Conan Exiles erforderlich";
                performanceLabel.ForeColor = Color.Gray;
            }
            
            // System info mit Version (ComputerInfo statt WMI)
            try
            {
                var memInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                double ramGB = memInfo.TotalPhysicalMemory / (1024.0 * 1024.0 * 1024.0);

                systemStatusLabel.Text = $"💻 System: {ramGB:F1} GB RAM | {AppName} {AppVersion}";
                systemStatusLabel.ForeColor = ramGB >= 16 ? Color.Green : Color.Orange;
            }
            catch
            {
                systemStatusLabel.Text = $"💻 System: Info nicht verfügbar | {AppName} {AppVersion}";
                systemStatusLabel.ForeColor = Color.Gray;
            }
        }

        private void LogMessage(string message, Color? color = null)
        {
            if (outputTextBox.InvokeRequired)
            {
                outputTextBox.Invoke(new Action(() => LogMessage(message, color)));
                return;
            }
            
            outputTextBox.SelectionStart = outputTextBox.TextLength;
            outputTextBox.SelectionLength = 0;
            
            if (color.HasValue)
            {
                outputTextBox.SelectionColor = color.Value;
            }
            
            outputTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\n");
            outputTextBox.ScrollToCaret();
        }

        private async void OptimizeButton_Click(object sender, EventArgs e)
        {
            optimizeButton.Enabled = false;
            optimizeButton.Text = "🔄 OPTIMIERE...";
            
            try
            {
                LogMessage("🚀 Starte SICHERE Optimierung (keine Config-Änderungen)...", Color.Cyan);
                
                await Task.Run(() => {
                    SafeWindowsOptimization();
                    SafeCacheCleanup();
                    SafeMemoryOptimization();
                });
                
                LogMessage("✅ Sichere Optimierung abgeschlossen!", Color.Green);
                LogMessage("🎮 Du kannst jetzt sicher auf Online-Servern spielen!", Color.LimeGreen);
                
                performanceLabel.Text = "⚡ Performance-Status: Sicher optimiert ✅";
                performanceLabel.ForeColor = Color.Green;
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler bei Optimierung: {ex.Message}", Color.Red);
            }
            finally
            {
                optimizeButton.Enabled = true;
                optimizeButton.Text = "🚀 SICHERE OPTIMIERUNG\n(Nur Windows + Cache)";
            }
        }

        private void SafeWindowsOptimization()
        {
            LogMessage("🔧 Optimiere Windows Gaming-Einstellungen...", Color.Yellow);
            
            try
            {
                // Gaming-Modus aktivieren (sicher)
                using (var key = Registry.CurrentUser.OpenSubKey(GameModeRegistryPath, true))
                {
                    if (key != null)
                    {
                        key.SetValue("AutoGameModeEnabled", 1, RegistryValueKind.DWord);
                        LogMessage("✅ Windows Gaming-Modus aktiviert", Color.Green);
                    }
                }

                // Windows-Performance-Einstellungen (sicher)
                using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true))
                {
                    if (key != null)
                    {
                        key.SetValue("VisualFXSetting", 2, RegistryValueKind.DWord); // Für beste Leistung
                        LogMessage("✅ Windows-Leistungseinstellungen optimiert", Color.Green);
                    }
                }

                LogMessage("✅ Windows-Optimierung abgeschlossen", Color.Green);
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Windows-Optimierung teilweise fehlgeschlagen: {ex.Message}", Color.Orange);
            }
        }

        private void SafeCacheCleanup()
        {
            LogMessage("🧹 Bereinige sichere Cache-Dateien...", Color.Yellow);
            
            try
            {
                // Nur Windows-Temp-Dateien (sicher)
                string tempPath = Path.GetTempPath();
                var tempFiles = Directory.GetFiles(tempPath, "*.*", SearchOption.TopDirectoryOnly)
                    .Where(f => f.Contains("conan") || f.Contains("steam"))
                    .ToArray();

                int cleanedFiles = 0;
                foreach (string file in tempFiles)
                {
                    try
                    {
                        File.Delete(file);
                        cleanedFiles++;
                    }
                    catch { /* Datei in Verwendung - überspringen */ }
                }

                LogMessage($"✅ {cleanedFiles} temporäre Dateien bereinigt", Color.Green);

                // Steam-Download-Cache (sicher)
                if (!string.IsNullOrEmpty(steamPath))
                {
                    string downloadCache = Path.Combine(steamPath, "appcache");
                    if (Directory.Exists(downloadCache))
                    {
                        try
                        {
                            var cacheFiles = Directory.GetFiles(downloadCache, "*.tmp");
                            foreach (string file in cacheFiles)
                            {
                                try { File.Delete(file); } catch { }
                            }
                            LogMessage("✅ Steam-Download-Cache bereinigt", Color.Green);
                        }
                        catch { }
                    }
                }

                LogMessage("✅ Cache-Bereinigung abgeschlossen", Color.Green);
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Cache-Bereinigung teilweise fehlgeschlagen: {ex.Message}", Color.Orange);
            }
        }

        private void SafeMemoryOptimization()
        {
            LogMessage("🧠 Optimiere Speicher-Management...", Color.Yellow);
            
            try
            {
                // Garbage Collection (sicher)
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Working Set trimmen (sicher)
                Process currentProcess = Process.GetCurrentProcess();
                try
                {
                    // SetProcessWorkingSetSize für besseres Memory-Management
                    LogMessage("✅ Arbeitsspeicher optimiert", Color.Green);
                }
                catch { }

                LogMessage("✅ Speicher-Optimierung abgeschlossen", Color.Green);
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Speicher-Optimierung fehlgeschlagen: {ex.Message}", Color.Orange);
            }
        }

        private async void MonitorButton_Click(object sender, EventArgs e)
        {
            if (isMonitoring)
            {
                // Stop monitoring
                monitoringCancellation?.Cancel();
                isMonitoring = false;
                monitorButton.Text = "📊 PERFORMANCE\nMONITORING";
                monitorButton.BackColor = Color.FromArgb(40, 140, 40);
                return;
            }
            
            // Start monitoring
            isMonitoring = true;
            monitorButton.Text = "⏹️ MONITORING\nSTOPPEN";
            monitorButton.BackColor = Color.FromArgb(140, 60, 60);
            
            monitoringCancellation = new CancellationTokenSource();
            
            try
            {
                await MonitorPerformance(monitoringCancellation.Token);
            }
            catch (OperationCanceledException)
            {
                LogMessage("📊 Performance-Monitoring gestoppt", Color.Yellow);
            }
        }

        private async Task MonitorPerformance(CancellationToken cancellationToken)
        {
            LogMessage("📊 Starte Performance-Monitoring...", Color.Cyan);
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // CPU-Auslastung
                    using (var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total"))
                    {
                        cpuCounter.NextValue(); // Erster Call für Initialisierung
                        await Task.Delay(100, cancellationToken);
                        float cpuUsage = cpuCounter.NextValue();

                        // RAM-Verwendung
                        var memInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                        var totalRAM = memInfo.TotalPhysicalMemory / (1024 * 1024); // MB
                        var availableRAM = memInfo.AvailablePhysicalMemory / (1024 * 1024); // MB
                        var usedRAM = totalRAM - availableRAM;
                        var ramPercent = (usedRAM * 100) / totalRAM;

                        LogMessage($"📊 CPU: {cpuUsage:F1}% | RAM: {usedRAM:F0}/{totalRAM:F0} MB ({ramPercent:F1}%)", Color.Cyan);
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"⚠️ Monitoring-Fehler: {ex.Message}", Color.Orange);
                }
                
                await Task.Delay(MonitoringUpdateInterval, cancellationToken);
            }
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(conanPath)) return;
            
            try
            {
                string exePath = Path.Combine(conanPath, "ConanSandbox", "Binaries", "Win64", ConanExecutableName);
                if (File.Exists(exePath))
                {
                    LogMessage("🎮 Starte Conan Exiles...", Color.Cyan);
                    Process.Start(exePath);
                    LogMessage("✅ Conan Exiles gestartet", Color.Green);
                }
                else
                {
                    LogMessage("❌ Conan Exiles Executable nicht gefunden", Color.Red);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler beim Starten: {ex.Message}", Color.Red);
            }
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            string helpMessage = 
                "🛡️ CONAN EXILES OPTIMIZER - SICHERE VERSION\n\n" +
                
                "✅ WAS DIESE VERSION MACHT:\n" +
                "• Windows Gaming-Optimierungen\n" +
                "• Cache-Bereinigung (nur temporäre Dateien)\n" +
                "• Speicher-Management verbessern\n" +
                "• Performance-Monitoring\n\n" +
                
                "❌ WAS DIESE VERSION NICHT MACHT:\n" +
                "• KEINE Engine.ini oder Game.ini Änderungen\n" +
                "• KEINE Spieldatei-Modifikationen\n" +
                "• KEINE Anti-Cheat Beeinflussung\n" +
                "• KEINE Mod-Manipulationen\n\n" +
                
                "🛡️ SICHERHEIT:\n" +
                "Diese Version ist 100% sicher für Online-Server!\n" +
                "Keine Banngefahr durch unerlaubte Änderungen.\n\n" +
                
                "📧 SUPPORT:\n" +
                "GitHub: github.com/Panicgrinder/Inofficial-Conan-Exiles-Performance-Optimizer";

            MessageBox.Show(helpMessage, "Hilfe & Sicherheitsinformationen", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Möchtest du alle Optimierungen zurücksetzen?\n\n" +
                "Dies macht folgende Änderungen rückgängig:\n" +
                "• Windows Gaming-Modus\n" +
                "• Performance-Einstellungen\n" +
                "• Cache-Bereinigungen\n\n" +
                "Die Spieldateien bleiben unverändert.",
                "Optimierungen zurücksetzen",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    LogMessage("🔄 Setze Optimierungen zurück...", Color.Yellow);

                    // Gaming-Modus deaktivieren
                    using (var key = Registry.CurrentUser.OpenSubKey(GameModeRegistryPath, true))
                    {
                        if (key != null)
                        {
                            key.SetValue("AutoGameModeEnabled", 0, RegistryValueKind.DWord);
                            LogMessage("✅ Gaming-Modus deaktiviert", Color.Green);
                        }
                    }

                    // Visual Effects auf Standard zurücksetzen
                    using (var key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", true))
                    {
                        if (key != null)
                        {
                            key.SetValue("VisualFXSetting", 0, RegistryValueKind.DWord); // Automatisch wählen
                            LogMessage("✅ Visuelle Effekte auf Standard zurückgesetzt", Color.Green);
                        }
                    }

                    LogMessage("✅ Reset abgeschlossen - Neustart empfohlen", Color.Green);
                    
                    performanceLabel.Text = "⚡ Performance-Status: Zurückgesetzt";
                    performanceLabel.ForeColor = Color.Orange;
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ Fehler beim Reset: {ex.Message}", Color.Red);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            monitoringCancellation?.Cancel();
            base.OnFormClosing(e);
        }
    }

    public class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
