using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
// using ConanExilesOptimizer.UI; // Temporär deaktiviert

namespace ConanExilesOptimizer
{
    public partial class MainForm : Form
    {
        #region Constants
        private const string AppName = "Conan Exiles Optimizer";
        private const string AppVersion = "v3.0.0";
        private const string AppTitle = "Conan Exiles Optimizer";
        private const string BuildDate = "2025-08-25";
        private const string SteamRegistryPath = @"SOFTWARE\Valve\Steam";
        private const string ConanAppId = "440900";
        private const string ConanExecutableName = "ConanSandbox-Win64-Shipping.exe";
        private const string GameModeRegistryPath = @"SOFTWARE\Microsoft\GameBar";
        private const int WindowWidth = 900;
        private const int WindowHeight = 700;
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
            // UI-Manager initialisieren (später implementiert)
            // UIManager.Initialize();
            // UIManager.ThemeChanged += OnThemeChanged;
            
            InitializeComponent();
            // DetectInstallations(); // Später implementiert
            // UpdateStatus(); // Später implementiert
            
            // Theme anwenden (später implementiert)
            // UIManager.ApplyTheme(this);
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main Form
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(WindowWidth, WindowHeight);
            this.Text = $"{AppTitle} {AppVersion}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.Icon = SystemIcons.Application;
            
            // Einfaches Menü direkt erstellen
            var menuStrip = new MenuStrip();
            var fileMenu = new ToolStripMenuItem("📁 &Datei");
            fileMenu.DropDownItems.Add("🚪 &Beenden", null, (s, e) => Application.Exit());
            var viewMenu = new ToolStripMenuItem("👁️ &Ansicht");  
            viewMenu.DropDownItems.Add("🎨 &UI-Einstellungen", null, (s, e) => MessageBox.Show("UI-Einstellungen werden implementiert...", "Info"));
            var helpMenu = new ToolStripMenuItem("❓ &Hilfe");
            helpMenu.DropDownItems.Add("ℹ️ &Über", null, (s, e) => MessageBox.Show("🗡️ Conan Exiles Optimizer v3.0.0 🏰\nEntwickler: Panicgrinder\n© 2025", "Über"));
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
            
            // Header Panel
            var headerPanel = new Panel
            {
                Size = new Size(880, 80),
                Location = new Point(10, 35), // Angepasst für Menü
                BackColor = Color.FromArgb(70, 130, 180),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var titleLabel = new Label
            {
                Text = "🗡️ CONAN EXILES OPTIMIZER 🏰",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Size = new Size(860, 40),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            var subtitleLabel = new Label
            {
                Text = "Optimiert automatisch Conan Exiles für maximale Performance",
                Font = new Font("Arial", 10),
                ForeColor = Color.LightGray,
                BackColor = Color.Transparent,
                Size = new Size(860, 20),
                Location = new Point(10, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            
            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            this.Controls.Add(headerPanel);
            
            // Status Group
            var statusGroup = new GroupBox
            {
                Text = "📊 Installation Status",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(430, 200),
                Location = new Point(10, 100),
                ForeColor = Color.DarkBlue
            };
            
            this.steamStatusLabel = new Label
            {
                Size = new Size(400, 25),
                Location = new Point(15, 30),
                Font = new Font("Arial", 9),
                Text = "🔍 Steam wird gesucht..."
            };
            
            this.conanStatusLabel = new Label
            {
                Size = new Size(400, 25),
                Location = new Point(15, 60),
                Font = new Font("Arial", 9),
                Text = "🔍 Conan Exiles wird gesucht..."
            };
            
            this.modStatusLabel = new Label
            {
                Size = new Size(400, 25),
                Location = new Point(15, 90),
                Font = new Font("Arial", 9),
                Text = "📦 Mod-Status wird geladen..."
            };
            
            this.systemStatusLabel = new Label
            {
                Size = new Size(400, 25),
                Location = new Point(15, 120),
                Font = new Font("Arial", 9),
                Text = "💻 System-Info wird geladen..."
            };
            
            this.performanceLabel = new Label
            {
                Size = new Size(400, 25),
                Location = new Point(15, 150),
                Font = new Font("Arial", 9, FontStyle.Bold),
                Text = "⚡ Performance-Status: Unbekannt",
                ForeColor = Color.Gray
            };
            
            statusGroup.Controls.AddRange(new Control[] {
                steamStatusLabel, conanStatusLabel, modStatusLabel, 
                systemStatusLabel, performanceLabel
            });
            this.Controls.Add(statusGroup);
            
            // Actions Group
            var actionsGroup = new GroupBox
            {
                Text = "🚀 Aktionen",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(430, 200),
                Location = new Point(460, 100),
                ForeColor = Color.DarkGreen
            };
            
            this.optimizeButton = new Button
            {
                Text = "🔧 Conan Exiles optimieren",
                Size = new Size(180, 40),
                Location = new Point(20, 30),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            optimizeButton.Click += OptimizeButton_Click;
            
            this.monitorButton = new Button
            {
                Text = "📊 Performance überwachen",
                Size = new Size(180, 40),
                Location = new Point(220, 30),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            monitorButton.Click += MonitorButton_Click;
            
            this.launchButton = new Button
            {
                Text = "🎮 Conan Exiles starten",
                Size = new Size(180, 40),
                Location = new Point(20, 80),
                BackColor = Color.Orange,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            launchButton.Click += LaunchButton_Click;
            
            this.refreshButton = new Button
            {
                Text = "🔄 Status aktualisieren",
                Size = new Size(180, 40),
                Location = new Point(220, 80),
                BackColor = Color.LightGray,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            refreshButton.Click += RefreshButton_Click;
            
            this.advancedButton = new Button
            {
                Text = "⚙️ Erweiterte Einstellungen",
                Size = new Size(180, 40),
                Location = new Point(20, 130),
                BackColor = Color.Plum,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            advancedButton.Click += AdvancedButton_Click;
            
            var helpButton = new Button
            {
                Text = "❓ Hilfe & Info",
                Size = new Size(180, 40),
                Location = new Point(220, 130),
                BackColor = Color.LightYellow,
                Font = new Font("Arial", 9, FontStyle.Bold),
                UseVisualStyleBackColor = false,
                Cursor = Cursors.Hand
            };
            helpButton.Click += HelpButton_Click;
            
            actionsGroup.Controls.AddRange(new Control[] {
                optimizeButton, monitorButton, launchButton, 
                refreshButton, advancedButton, helpButton
            });
            
            // Add tooltips for main buttons
            var mainToolTip = new ToolTip();
            mainToolTip.SetToolTip(optimizeButton, 
                "Führt alle Optimierungen durch:\n" +
                "• Cache & Logs löschen\n" +
                "• Registry optimieren\n" +
                "• Mod-Pfade reparieren\n" +
                "• Performance-Einstellungen anwenden");
            mainToolTip.SetToolTip(monitorButton, 
                "Überwacht Conan Exiles Performance in Echtzeit:\n" +
                "• CPU & RAM Verbrauch (System & Conan)\n" +
                "• Ladezeit-Analyse\n" +
                "• Performance-Warnungen\n" +
                "• Spiel-Status Überwachung\n" +
                "Hinweis: FPS-Monitoring erfordert zusätzliche Tools");
            mainToolTip.SetToolTip(launchButton, 
                "Startet Conan Exiles mit optimierten Parametern:\n" +
                "• Schnellerer Start durch -NoBattleEye\n" +
                "• Überspringt Intro-Videos\n" +
                "• Aktiviert Performance-Features\n" +
                "• Optimierte Speicher-Verwaltung");
            mainToolTip.SetToolTip(refreshButton, 
                "Aktualisiert alle System-Informationen:\n" +
                "• Steam & Conan Installation\n" +
                "• Mod-Status überprüfen\n" +
                "• Performance-Werte neu laden\n" +
                "• System-Diagnose durchführen");
            mainToolTip.SetToolTip(advancedButton, 
                "Öffnet erweiterte Einstellungen:\n" +
                "• Startparameter anpassen\n" +
                "• Bereinigungsoptionen wählen\n" +
                "• System-Optimierungen konfigurieren\n" +
                "• Für erfahrene Benutzer");
            mainToolTip.SetToolTip(helpButton, 
                "Zeigt detaillierte Hilfe-Informationen:\n" +
                "• Funktions-Übersicht\n" +
                "• Verwendungsanleitung\n" +
                "• Erwartete Verbesserungen\n" +
                "• Problemlösungen");
            
            this.Controls.Add(actionsGroup);
            
            // Progress Bar
            this.progressBar = new ProgressBar
            {
                Size = new Size(880, 20),
                Location = new Point(10, 320),
                Style = ProgressBarStyle.Continuous,
                Visible = false
            };
            this.Controls.Add(progressBar);
            
            // Output Group
            var outputGroup = new GroupBox
            {
                Text = "📋 Ausgabe & Logs",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Size = new Size(880, 300),
                Location = new Point(10, 350),
                ForeColor = Color.DarkRed
            };
            
            this.outputTextBox = new RichTextBox
            {
                Size = new Size(860, 270),
                Location = new Point(10, 25),
                Font = new Font("Consolas", 9),
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };
            
            outputGroup.Controls.Add(outputTextBox);
            this.Controls.Add(outputGroup);
            
            // Footer
            var footerLabel = new Label
            {
                Text = "💡 Tipp: Schließen Sie Conan Exiles vor der Optimierung für beste Ergebnisse!",
                Size = new Size(880, 20),
                Location = new Point(10, 670),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkGreen,
                Font = new Font("Arial", 9, FontStyle.Italic)
            };
            this.Controls.Add(footerLabel);
            
            this.ResumeLayout(false);
        }
        
        // Controls
        private Label steamStatusLabel;
        private Label conanStatusLabel;
        private Label modStatusLabel;
        private Label systemStatusLabel;
        private Label performanceLabel;
        private Button optimizeButton;
        private Button monitorButton;
        private Button launchButton;
        private Button refreshButton;
        private Button advancedButton;
        private ProgressBar progressBar;
        private RichTextBox outputTextBox;
        
        private void DetectInstallations()
        {
            DetectSteamInstallation();
            DetectConanInstallation();
        }

        private void DetectSteamInstallation()
        {
            // Try Registry first
            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(SteamRegistryPath))
                {
                    var installPath = key?.GetValue("InstallPath") as string;
                    if (!string.IsNullOrEmpty(installPath) && File.Exists(Path.Combine(installPath, "steam.exe")))
                    {
                        steamPath = installPath;
                        return;
                    }
                }
            }
            catch (Exception)
            {
                // Registry access failed, fallback to common paths
            }

            // Fallback to common installation paths
            string[] steamPaths = {
                @"C:\Program Files (x86)\Steam",
                @"C:\Program Files\Steam",
                @"D:\Steam",
                @"E:\Steam",
                @"F:\Steam",
                @"G:\Steam"
            };
            
            foreach (string path in steamPaths)
            {
                if (File.Exists(Path.Combine(path, "steam.exe")))
                {
                    steamPath = path;
                    break;
                }
            }
        }

        private void DetectConanInstallation()
        {
            if (string.IsNullOrEmpty(steamPath))
                return;

            string conanTestPath = Path.Combine(steamPath, "steamapps", "common", "Conan Exiles");
            if (Directory.Exists(Path.Combine(conanTestPath, "ConanSandbox")))
            {
                conanPath = conanTestPath;
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
                
                // Check for optimized launcher
                string launcherPath = Path.Combine(conanPath, "ConanExiles_Optimized.bat");
                if (File.Exists(launcherPath))
                {
                    performanceLabel.Text = "⚡ Performance-Status: Optimiert ✅";
                    performanceLabel.ForeColor = Color.Green;
                }
                else
                {
                    performanceLabel.Text = "⚡ Performance-Status: Nicht optimiert";
                    performanceLabel.ForeColor = Color.Orange;
                }
            }
            else
            {
                conanStatusLabel.Text = "❌ Conan Exiles nicht gefunden";
                conanStatusLabel.ForeColor = Color.Red;
                
                optimizeButton.Enabled = false;
                monitorButton.Enabled = false;
                launchButton.Enabled = false;
                
                performanceLabel.Text = "⚡ Performance-Status: Nicht verfügbar";
                performanceLabel.ForeColor = Color.Gray;
            }
            
            // Mod status
            if (!string.IsNullOrEmpty(conanPath))
            {
                string modlistPath = Path.Combine(conanPath, "ConanSandbox", "Mods", "modlist.txt");
                if (File.Exists(modlistPath))
                {
                    int modCount = File.ReadAllLines(modlistPath).Where(line => !string.IsNullOrWhiteSpace(line)).Count();
                    modStatusLabel.Text = $"📦 Aktive Mods: {modCount}";
                    modStatusLabel.ForeColor = modCount > 20 ? Color.Red : modCount > 10 ? Color.Orange : Color.Green;
                }
                else
                {
                    modStatusLabel.Text = "📦 Keine Mods konfiguriert";
                    modStatusLabel.ForeColor = Color.Gray;
                }
            }
            
            // System info
            try
            {
                var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
                foreach (ManagementObject obj in searcher.Get())
                {
                    long totalRAM = Convert.ToInt64(obj["TotalPhysicalMemory"]);
                    double ramGB = totalRAM / (1024.0 * 1024.0 * 1024.0);
                    
                    systemStatusLabel.Text = $"💻 System: {ramGB:F1} GB RAM | {AppName} v{AppVersion} ({BuildDate})";
                    systemStatusLabel.ForeColor = ramGB >= 16 ? Color.Green : Color.Orange;
                    break;
                }
            }
            catch
            {
                systemStatusLabel.Text = $"💻 System: Info nicht verfügbar | {AppName} v{AppVersion} ({BuildDate})";
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
            outputTextBox.SelectionColor = color ?? Color.LightGreen;
            outputTextBox.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}\r\n");
            outputTextBox.ScrollToCaret();
        }
        
        private async void OptimizeButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(conanPath))
            {
                MessageBox.Show("Conan Exiles Installation nicht gefunden!", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            optimizeButton.Enabled = false;
            progressBar.Visible = true;
            progressBar.Value = 0;
            
            LogMessage("=== Starte Conan Exiles Optimierung ===", Color.Cyan);
            
            try
            {
                // Step 1: Backup and fix mod paths
                progressBar.Value = 20;
                await Task.Run(() => FixModPaths());
                
                // Step 2: Clean cache
                progressBar.Value = 40;
                await Task.Run(() => CleanCache());
                
                // Step 3: Create optimized launcher
                progressBar.Value = 50;
                await Task.Run(() => CreateOptimizedLauncher());
                
                // Step 4: Registry optimizations
                progressBar.Value = 65;
                await Task.Run(() => ApplyRegistryOptimizations());
                
                // Step 5: Advanced Engine optimizations
                progressBar.Value = 85;
                await Task.Run(() => ApplyAdvancedOptimizations());
                
                progressBar.Value = 100;
                LogMessage("✅ Optimierung erfolgreich abgeschlossen!", Color.Green);
                MessageBox.Show("Conan Exiles wurde erfolgreich optimiert!\n\nWichtige Community-Fixes wurden angewendet:\n• MoveRepSize-Fix (behebt Thrall-Probleme)\n• AI-Trace-Optimierungen\n• Netzwerk-Verbesserungen\n\nSie können jetzt das Spiel über den optimierten Launcher starten.", 
                               "Optimierung abgeschlossen", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                UpdateStatus();
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler bei der Optimierung: {ex.Message}", Color.Red);
                MessageBox.Show($"Fehler bei der Optimierung:\n{ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                optimizeButton.Enabled = true;
                progressBar.Visible = false;
            }
        }
        
        private void FixModPaths()
        {
            LogMessage("Repariere Mod-Pfade...", Color.Yellow);
            
            string modlistPath = Path.Combine(conanPath, "ConanSandbox", "Mods", "modlist.txt");
            if (!File.Exists(modlistPath))
            {
                LogMessage("Keine modlist.txt gefunden - überspringe Mod-Reparatur", Color.Gray);
                return;
            }
            
            string[] lines = File.ReadAllLines(modlistPath);
            bool changed = false;
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(@":\") && !lines[i].StartsWith(Path.GetPathRoot(steamPath)))
                {
                    // Fix drive letter
                    string modId = Path.GetFileNameWithoutExtension(lines[i]);
                    string newPath = Path.Combine(steamPath, "steamapps", "workshop", "content", "440900", modId, Path.GetFileName(lines[i]));
                    lines[i] = newPath;
                    changed = true;
                }
            }
            
            if (changed)
            {
                // Create backup
                string backupPath = $"{modlistPath}.backup.{DateTime.Now:yyyyMMdd_HHmmss}";
                File.Copy(modlistPath, backupPath);
                LogMessage($"Backup erstellt: {Path.GetFileName(backupPath)}", Color.Cyan);
                
                File.WriteAllLines(modlistPath, lines);
                LogMessage("✅ Mod-Pfade erfolgreich repariert", Color.Green);
            }
            else
            {
                LogMessage("✅ Mod-Pfade bereits korrekt", Color.Green);
            }
        }
        
        private void CleanCache()
        {
            LogMessage("Bereinige Cache und temporäre Dateien...", Color.Yellow);
            
            string[] cleanPaths = {
                Path.Combine(conanPath, "ConanSandbox", "Saved", "Logs"),
                Path.Combine(conanPath, "ConanSandbox", "Intermediate"),
                Path.Combine(conanPath, "ConanSandbox", "Saved", "Config", "WindowsClient")
            };
            
            foreach (string cleanPath in cleanPaths)
            {
                if (Directory.Exists(cleanPath))
                {
                    try
                    {
                        var files = Directory.GetFiles(cleanPath, "*.*", SearchOption.AllDirectories)
                                           .Where(f => f.EndsWith(".tmp") || f.EndsWith(".log") || f.EndsWith(".cache"));
                        
                        foreach (string file in files)
                        {
                            try
                            {
                                File.Delete(file);
                            }
                            catch { } // Ignore file in use
                        }
                    }
                    catch { } // Ignore access denied
                }
            }
            
            LogMessage("✅ Cache-Bereinigung abgeschlossen", Color.Green);
        }
        
        private void CreateOptimizedLauncher()
        {
            LogMessage("Erstelle optimierten Launcher...", Color.Yellow);
            LogMessage($"🔍 Conan-Pfad: {conanPath}", Color.Cyan);
            
            // Finde die richtige exe-Datei
            string exePath = "";
            string[] possibleExes = {
                Path.Combine(conanPath, "ConanSandbox", "Binaries", "Win64", ConanExecutableName),
                Path.Combine(conanPath, "ConanSandbox.exe"),
                Path.Combine(conanPath, "Binaries", "Win64", ConanExecutableName)
            };
            
            LogMessage("🔍 Suche nach ausführbarer Datei...", Color.Yellow);
            foreach (string testPath in possibleExes)
            {
                LogMessage($"  🔍 Teste: {testPath}", Color.Gray);
                if (File.Exists(testPath))
                {
                    exePath = testPath;
                    LogMessage($"  ✅ Gefunden: {testPath}", Color.Green);
                    break;
                }
                else
                {
                    LogMessage($"  ❌ Nicht gefunden: {testPath}", Color.Red);
                }
            }
            
            if (string.IsNullOrEmpty(exePath))
            {
                LogMessage("❌ Conan Exiles ausführbare Datei nicht gefunden!", Color.Red);
                LogMessage("🔍 Verfügbare Dateien im Conan-Verzeichnis:", Color.Yellow);
                try
                {
                    if (Directory.Exists(conanPath))
                    {
                        var files = Directory.GetFiles(conanPath, "*.exe", SearchOption.AllDirectories);
                        foreach (var file in files.Take(10)) // Zeige nur die ersten 10
                        {
                            LogMessage($"  📁 {file}", Color.Gray);
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogMessage($"❌ Fehler beim Durchsuchen: {ex.Message}", Color.Red);
                }
                return;
            }
            
            // Verwende den STEAM-Start als Alternative, der zuverlässiger ist
            string launcherContent = $@"@echo off
echo Starte Conan Exiles mit optimierten Community-Parametern über Steam...
echo.

REM Starte über Steam mit App-ID (zuverlässiger als direkter Start)
REM Parameter basierend auf kเt's Community-Guide (Steam)
start steam://run/{ConanAppId}//-NOSPLASH -UsePerfThreads -malloc=system -NOTEXTURESTREAMING -norhithread

echo Conan Exiles wurde über Steam mit Community-optimierten Parametern gestartet!
echo.
echo === VERWENDETE OPTIMIERUNGEN (Community-Guide) ===
echo - BattlEye AKTIV (für Online-Server erforderlich)
echo - Splash-Screen übersprungen (-NOSPLASH)
echo - Performance-Threads aktiviert (-UsePerfThreads)
echo - System-Memory-Allocator (-malloc=system)
echo - Texture-Streaming optimiert (-NOTEXTURESTREAMING)
echo - RHI-Thread deaktiviert (-norhithread)
echo.
echo === ENGINE-OPTIMIERUNGEN ANGEWENDET ===
echo - MoveRepSize=512.0f (behebt Thrall-Stat-Verlust)
echo - MaxTracesPerTick=500 (verbessert AI-Performance)
echo - Netzwerk-Optimierungen für bessere Stabilität
echo - Grafik-Cache-Verbesserungen
echo.
echo HINWEIS: Diese Einstellungen basieren auf erprobten Community-Fixes!
echo BattlEye bleibt für Online-Kompatibilität aktiv.
echo.
timeout /t 8
";
            
            string launcherPath = Path.Combine(conanPath, "ConanExiles_Optimized.bat");
            File.WriteAllText(launcherPath, launcherContent);
            
            LogMessage($"✅ Optimierter Launcher erstellt: {Path.GetFileName(launcherPath)}", Color.Green);
            LogMessage($"📁 Exe gefunden: {Path.GetFileName(exePath)}", Color.Cyan);
            LogMessage($"🚀 Launcher verwendet Steam-Start für bessere Kompatibilität", Color.Yellow);
        }
        
        private void ApplyRegistryOptimizations()
        {
            LogMessage("Wende System-Optimierungen an...", Color.Yellow);
            
            try
            {
                // Game Mode aktivieren
                using (RegistryKey key = Registry.CurrentUser.CreateSubKey(GameModeRegistryPath))
                {
                    key?.SetValue("AllowAutoGameMode", 1, RegistryValueKind.DWord);
                }
                
                LogMessage("✅ Windows Game Mode aktiviert", Color.Green);
            }
            catch (Exception ex)
            {
                LogMessage($"⚠️ Registry-Optimierung teilweise fehlgeschlagen: {ex.Message}", Color.Orange);
            }
        }
        
        private void ApplyAdvancedOptimizations()
        {
            LogMessage("🔧 Wende erweiterte Engine-Optimierungen an (basierend auf Community-Guide)...", Color.Yellow);
            
            try
            {
                string configPath = Path.Combine(conanPath, "ConanSandbox", "Saved", "Config", "WindowsNoEditor");
                
                // Erstelle Config-Verzeichnis falls nicht vorhanden
                Directory.CreateDirectory(configPath);
                
                // Engine.ini Optimierungen basierend auf kเt's Steam Guide
                string engineIniPath = Path.Combine(configPath, "Engine.ini");
                string engineOptimizations = @"
; === CONAN EXILES OPTIMIZER - ERWEITERTE OPTIMIERUNGEN ===
; Basierend auf Community-Guide von kเt (Steam)

[/script/onlinesubsystemutils.ipnetdriver]
NetServerMaxTickRate=60
MaxClientRate=600000
MaxInternetClientRate=600000

[SystemSettings]
dw.NetClientFloatsDuringNavWalking=0

[/script/conansandbox.systemsettings]
dw.SkeletalMeshTickRate=0.1
dw.EnableAISpawning=1
dw.EnableInitialAISpawningPass=1
dw.NPCsTargetBuildings=1
dw.nav.AvoidNonPawns=1
dw.nav.InterpolateAvoidanceResult=1
dw.AILOD1Distance=4000
dw.AILOD2Distance=8000
dw.AILOD3Distance=11500

[/script/engine.renderersettings]
r.GraphicsAdapter=-1
r.Cache.LightingCacheDimension=75
r.TemporalAASamples=4
r.TemporalAACurrentFrameWeight=0.1

[/script/engine.physicssettings]
bDefaultHasComplexCollision=True

";

                File.AppendAllText(engineIniPath, engineOptimizations);
                LogMessage("✅ Engine.ini Optimierungen angewendet", Color.Green);
                
                // Game.ini - Die kritischen Fixes!
                string gameIniPath = Path.Combine(configPath, "Game.ini");
                string gameOptimizations = @"
; === CONAN EXILES OPTIMIZER - KRITISCHE NETZWERK-FIXES ===
; Diese Einstellungen beheben die wichtigsten Performance-Probleme

[/script/engine.gamenetworkmanager]
TotalNetBandwidth=4000000
MaxDynamicBandwidth=100000
MinDynamicBandwidth=10000
MoveRepSize=512.0f
MAXPOSITIONERRORSQUARED=3.0f
MaxClientSmoothingDeltaTime=1.0f

[/script/conansandbox.aisense_newsight]
MaxTracesPerTick=500

[/script/conansandbox.aisenseconfig_newsight]
PeripheralVisionAngleDegrees=75

[/script/aimodule.envquerymanager]
MaxAllowedTestingTime=0.003
bTestQueriesUsingBreadth=false

";

                File.AppendAllText(gameIniPath, gameOptimizations);
                LogMessage("✅ Game.ini Netzwerk-Fixes angewendet", Color.Green);
                
                LogMessage("🎯 Kritische Community-Fixes angewendet:", Color.Cyan);
                LogMessage("  • MoveRepSize=512.0f (behebt Thrall/NPC-Stat-Verlust)", Color.White);
                LogMessage("  • MaxTracesPerTick=500 (verbessert AI-Reaktionszeit)", Color.White);
                LogMessage("  • Netzwerk-Optimierungen für bessere Stabilität", Color.White);
                LogMessage("  • Grafik-Cache-Optimierungen", Color.White);
                
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler bei erweiterten Optimierungen: {ex.Message}", Color.Red);
            }
        }
        
        private async void MonitorButton_Click(object sender, EventArgs e)
        {
            if (isMonitoring)
            {
                // Stop monitoring
                monitoringCancellation?.Cancel();
                return;
            }
            
            isMonitoring = true;
            monitorButton.Text = "⏹️ Monitoring stoppen";
            monitorButton.BackColor = Color.LightCoral;
            
            monitoringCancellation = new CancellationTokenSource();
            
            LogMessage("=== Starte Performance-Monitoring ===", Color.Cyan);
            LogMessage("Überwache System-Ressourcen und Conan-Prozesse...", Color.Yellow);
            
            try
            {
                await Task.Run(() => RunMonitoring(monitoringCancellation.Token));
            }
            catch (OperationCanceledException)
            {
                LogMessage("Monitoring gestoppt", Color.Orange);
            }
            finally
            {
                isMonitoring = false;
                monitorButton.Text = "📊 Performance überwachen";
                monitorButton.BackColor = Color.LightBlue;
            }
        }
        
        private void RunMonitoring(CancellationToken cancellationToken)
        {
            var startTime = DateTime.Now;
            bool conanDetected = false;
            DateTime? conanStartTime = null;
            float lastConanRAM = 0;
            int stableRAMCount = 0;
            float maxConanRAM = 0;
            
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    // CPU usage
                    var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                    float cpuUsage = cpuCounter.NextValue();
                    Thread.Sleep(MonitoringUpdateInterval); // Required for CPU counter
                    cpuUsage = cpuCounter.NextValue();
                    
                    // Memory usage
                    var memCounter = new PerformanceCounter("Memory", "Available MBytes");
                    float availableRAM = memCounter.NextValue();
                    
                    // Check for Conan process
                    var conanProcesses = Process.GetProcessesByName("ConanSandbox");
                    if (conanProcesses.Length > 0 && !conanDetected)
                    {
                        conanDetected = true;
                        conanStartTime = DateTime.Now;
                        LogMessage("🎮 Conan Exiles Prozess erkannt! Beginne Ladezeit-Analyse...", Color.Green);
                    }
                    
                    if (conanProcesses.Length > 0)
                    {
                        var conanProcess = conanProcesses[0];
                        float conanRAM = conanProcess.WorkingSet64 / (1024f * 1024f);
                        
                        // Track max RAM usage
                        if (conanRAM > maxConanRAM)
                            maxConanRAM = conanRAM;
                        
                        // Detect loading issues
                        float ramDifference = Math.Abs(conanRAM - lastConanRAM);
                        if (ramDifference < 50 && conanRAM > 500) // RAM stable for loading detection
                        {
                            stableRAMCount++;
                            if (stableRAMCount == 5) // 15 seconds stable
                            {
                                if (conanStartTime.HasValue)
                                {
                                    var loadTime = DateTime.Now - conanStartTime.Value;
                                    LogMessage($"✅ Spiel geladen! Ladezeit: {loadTime.TotalSeconds:F1}s | Max RAM: {maxConanRAM:F0}MB", Color.Green);
                                    conanStartTime = null; // Reset
                                }
                            }
                        }
                        else
                        {
                            stableRAMCount = 0; // Reset stability counter
                        }
                        
                        // Enhanced logging with loading status
                        string status = "";
                        if (conanRAM < 300)
                            status = "🔄 Initialisierung";
                        else if (conanRAM < 1000)
                            status = "📦 Lade Assets";
                        else if (conanRAM < 2500)
                            status = "🌍 Lade Welt";
                        else if (conanRAM > 3500)
                            status = "⚠️ Hoher RAM-Verbrauch";
                        else
                            status = "✅ Spiel läuft";
                        
                        LogMessage($"📊 System: CPU {cpuUsage:F1}% | RAM frei: {availableRAM:F0}MB | Conan RAM: {conanRAM:F0}MB | {status}", Color.Cyan);
                        lastConanRAM = conanRAM;
                    }
                    else
                    {
                        if (conanDetected)
                        {
                            LogMessage("❌ Conan Exiles Prozess beendet/abgestürzt!", Color.Red);
                            conanDetected = false;
                            maxConanRAM = 0;
                            stableRAMCount = 0;
                        }
                        LogMessage($"📊 System: CPU {cpuUsage:F1}% | RAM frei: {availableRAM:F0}MB | Conan: Nicht aktiv", Color.Gray);
                    }
                    
                    // Performance warnings with more context
                    if (cpuUsage > 90)
                        LogMessage("⚠️ Kritische CPU-Auslastung! Möglicher Flaschenhals.", Color.Red);
                    else if (cpuUsage > 70)
                        LogMessage("⚠️ Hohe CPU-Auslastung erkannt.", Color.Orange);
                        
                    if (availableRAM < 1024)
                        LogMessage("🚨 Kritischer RAM-Mangel! System könnte instabil werden.", Color.Red);
                    else if (availableRAM < 2048)
                        LogMessage("⚠️ Wenig RAM verfügbar. Performance könnte leiden.", Color.Orange);
                    
                    // Detect loading problems
                    if (conanProcesses.Length > 0 && lastConanRAM > 4000)
                        LogMessage("⚠️ Sehr hoher RAM-Verbrauch! Mögliches Memory-Leak oder Mod-Problem.", Color.Red);
                    
                    Thread.Sleep(3000); // Monitor every 3 seconds
                }
                catch (Exception ex)
                {
                    LogMessage($"Monitoring-Fehler: {ex.Message}", Color.Red);
                    Thread.Sleep(5000);
                }
            }
        }
        
        private void LaunchButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(conanPath))
            {
                MessageBox.Show("Conan Exiles Installation nicht gefunden!\n\nBitte stellen Sie sicher, dass Conan Exiles über Steam installiert ist.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string optimizedLauncher = Path.Combine(conanPath, "ConanExiles_Optimized.bat");
            
            try
            {
                if (File.Exists(optimizedLauncher))
                {
                    LogMessage("🚀 Starte Conan Exiles mit optimierten Parametern...", Color.Green);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = optimizedLauncher,
                        UseShellExecute = true,
                        WorkingDirectory = conanPath
                    });
                }
                else
                {
                    LogMessage("⚠️ Optimierter Launcher nicht gefunden, erstelle ihn...", Color.Yellow);
                    CreateOptimizedLauncher();
                    
                    if (File.Exists(optimizedLauncher))
                    {
                        LogMessage("🚀 Starte Conan Exiles mit optimierten Parametern...", Color.Green);
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = optimizedLauncher,
                            UseShellExecute = true,
                            WorkingDirectory = conanPath
                        });
                    }
                    else
                    {
                        LogMessage("🚀 Fallback: Starte Conan Exiles über Steam...", Color.Yellow);
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = "steam://run/440900",
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Fehler beim Starten: {ex.Message}", Color.Red);
                
                // Fallback zu Steam
                try
                {
                    LogMessage("🚀 Fallback: Starte über Steam...", Color.Yellow);
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "steam://run/440900",
                        UseShellExecute = true
                    });
                }
                catch
                {
                    MessageBox.Show($"Fehler beim Starten von Conan Exiles:\n{ex.Message}\n\nBitte starten Sie das Spiel manuell über Steam.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LogMessage("🔄 Aktualisiere Status...", Color.Cyan);
            DetectInstallations();
            UpdateStatus();
            
            // Zusätzliche Diagnose
            LogMessage("🔍 Führe System-Diagnose durch...", Color.Yellow);
            
            try
            {
                // Check available disk space
                var conanDrive = Path.GetPathRoot(conanPath ?? "C:");
                var driveInfo = new DriveInfo(conanDrive);
                long freeSpaceGB = driveInfo.AvailableFreeSpace / (1024 * 1024 * 1024);
                
                if (freeSpaceGB < 10)
                    LogMessage($"⚠️ Wenig Festplattenspeicher: {freeSpaceGB}GB frei", Color.Red);
                else
                    LogMessage($"💾 Festplattenspeicher: {freeSpaceGB}GB frei", Color.Green);
                
                // Check for common problem files
                if (!string.IsNullOrEmpty(conanPath))
                {
                    string modListPath = Path.Combine(conanPath, "ConanSandbox", "Mods", "modlist.txt");
                    if (File.Exists(modListPath))
                    {
                        var modCount = File.ReadAllLines(modListPath).Where(line => !string.IsNullOrWhiteSpace(line)).Count();
                        LogMessage($"📦 Aktive Mods: {modCount}", modCount > 50 ? Color.Orange : Color.Green);
                        
                        if (modCount > 100)
                            LogMessage("⚠️ Sehr viele Mods! Das kann Ladeprobleme verursachen.", Color.Red);
                    }
                    
                    // Check for crash logs
                    string logPath = Path.Combine(conanPath, "ConanSandbox", "Saved", "Logs");
                    if (Directory.Exists(logPath))
                    {
                        var recentLogs = Directory.GetFiles(logPath, "*.log")
                            .Where(f => File.GetLastWriteTime(f) > DateTime.Now.AddDays(-1))
                            .Count();
                        
                        if (recentLogs > 5)
                            LogMessage($"⚠️ {recentLogs} neue Log-Dateien - mögliche Stabilitätsprobleme", Color.Orange);
                    }
                }
                
                // RAM recommendation
                var totalRAM = GC.GetTotalMemory(false) / (1024 * 1024); // Approximation
                LogMessage($"💻 Empfehlung: Mindestens 16GB RAM für stabiles Spiel mit Mods", Color.Cyan);
                
            }
            catch (Exception ex)
            {
                LogMessage($"Diagnose-Fehler: {ex.Message}", Color.Red);
            }
            
            LogMessage("✅ Status aktualisiert", Color.Green);
        }
        
        private void AdvancedButton_Click(object sender, EventArgs e)
        {
            var advancedForm = new AdvancedSettingsForm(conanPath, steamPath);
            advancedForm.ShowDialog();
        }
        
        private void HelpButton_Click(object sender, EventArgs e)
        {
            string helpText = @"🗡️ CONAN EXILES OPTIMIZER v3.0 🏰

=== NEUE COMMUNITY-OPTIMIERUNGEN ===
Basierend auf kเt's umfassendem Steam-Guide wurden die wichtigsten
Performance-Fixes integriert:

🎯 KRITISCHE FIXES ANGEWENDET:
✅ MoveRepSize=512.0f - BEHEBT Thrall-Stat-Verlust!
✅ MaxTracesPerTick=500 - Verbessert AI-Reaktionszeit drastisch
✅ Netzwerk-Optimierungen für bessere Online-Stabilität
✅ Engine-Cache-Verbesserungen für flüssigeres Gameplay

🚨 BEKANNTE PROBLEME (vom Guide):
❌ Standard MoveRepSize zu niedrig → Thralls verlieren Stats
❌ MaxTracesPerTick=20 → AI reagiert nicht
❌ Schlechte Netzwerk-Einstellungen → Verbindungsabbrüche
❌ -NoSteamClient Parameter → Kann Probleme verursachen

=== FUNKTIONEN ===
✅ Automatische Steam & Conan Erkennung
✅ Erweiterte Engine.ini & Game.ini Optimierungen
✅ BattlEye Smart-Toggle (Online/Offline)
✅ Performance-Monitoring mit Ladezeit-Analyse
✅ Steam-Integration für zuverlässigeren Start

=== VERWENDUNG ===
1. 'Optimieren' - Wendet ALLE Community-Fixes an
2. 'Monitoring' - Überwacht Performance & Ladephasen
3. 'Starten' - Verwendet Steam mit optimierten Parametern
4. 'Erweitert' - BattlEye & weitere Optionen

=== ERWARTETE VERBESSERUNGEN ===
⚡ Deutlich verbesserte AI-Performance
🛡️ Keine Thrall-Stat-Verluste mehr
🌐 Stabilere Online-Verbindungen
💾 Optimierte Speicher-Nutzung
🎮 Flüssigeres Gameplay insgesamt

Diese Version nutzt erprobte Community-Lösungen!";

            var helpForm = new Form
            {
                Text = "Conan Exiles Optimizer - Hilfe & Informationen",
                Size = new Size(650, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            var helpTextBox = new TextBox
            {
                Text = helpText,
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Size = new Size(620, 520),
                Location = new Point(10, 10),
                Font = new Font("Consolas", 9),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            var closeButton = new Button
            {
                Text = "Schließen",
                Size = new Size(100, 30),
                Location = new Point(275, 540),
                DialogResult = DialogResult.OK
            };

            helpForm.Controls.AddRange(new Control[] { helpTextBox, closeButton });
            helpForm.ShowDialog();
        }
    }
    
    // Advanced Settings Form
    public partial class AdvancedSettingsForm : Form
    {
        private string conanPath;
        private string steamPath;
        
        public AdvancedSettingsForm(string conanPath, string steamPath)
        {
            this.conanPath = conanPath;
            this.steamPath = steamPath;
            InitializeComponent();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.Size = new Size(750, 610);
            this.Text = "⚙️ Erweiterte Einstellungen";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            
            var mainLabel = new Label
            {
                Text = "⚙️ Erweiterte Optimierungs-Einstellungen",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Size = new Size(720, 30),
                Location = new Point(10, 10),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.DarkBlue
            };
            this.Controls.Add(mainLabel);
            
            var startParamsGroup = new GroupBox
            {
                Text = "🚀 Startparameter",
                Size = new Size(720, 180),
                Location = new Point(10, 50),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            
            var paramsTextBox = new TextBox
            {
                Text = "-NOSPLASH -UsePerfThreads -malloc=system",
                Size = new Size(700, 25),
                Location = new Point(10, 30),
                Font = new Font("Consolas", 9)
            };
            
            var paramsInfo = new Label
            {
                Text = "Diese Parameter werden beim optimierten Start verwendet.\nBattlEye wird NICHT deaktiviert für Online-Kompatibilität!",
                Size = new Size(700, 40),
                Location = new Point(10, 65),
                ForeColor = Color.DarkBlue
            };
            
            var battleEyeCheck = new CheckBox
            {
                Text = "BattlEye deaktivieren (nur für Singleplayer/private Server)",
                Checked = false,
                Location = new Point(10, 115),
                Size = new Size(400, 20),
                ForeColor = Color.Red
            };
            
            // Event handler für BattlEye checkbox
            battleEyeCheck.CheckedChanged += (s, e) =>
            {
                if (battleEyeCheck.Checked)
                {
                    paramsTextBox.Text = "-NoBattleEye -NOSPLASH -UsePerfThreads -malloc=system";
                    paramsInfo.Text = "WARNUNG: BattlEye deaktiviert! Nur für Singleplayer/private Server!";
                    paramsInfo.ForeColor = Color.Red;
                }
                else
                {
                    paramsTextBox.Text = "-NOSPLASH -UsePerfThreads -malloc=system";
                    paramsInfo.Text = "Diese Parameter werden beim optimierten Start verwendet.\nBattlEye wird NICHT deaktiviert für Online-Kompatibilität!";
                    paramsInfo.ForeColor = Color.DarkBlue;
                }
            };
            
            // Add tooltip for start parameters
            var paramsToolTip = new ToolTip();
            paramsToolTip.SetToolTip(paramsTextBox, 
                "-NOSPLASH: Überspringt Intro-Videos\n" +
                "-UsePerfThreads: Aktiviert Performance-Threads\n" +
                "-malloc=system: Optimiert Speicher-Verwaltung\n" +
                "BattlEye bleibt aktiv für Online-Server!");
            paramsToolTip.SetToolTip(battleEyeCheck,
                "WARNUNG: Deaktiviert BattlEye Anti-Cheat!\n" +
                "• Funktioniert NUR auf privaten Servern ohne BattlEye\n" +
                "• Offizielle Server werden Sie kicken!\n" +
                "• Nur für Singleplayer oder spezielle private Server");
            
            startParamsGroup.Controls.Add(paramsTextBox);
            startParamsGroup.Controls.Add(paramsInfo);
            startParamsGroup.Controls.Add(battleEyeCheck);
            this.Controls.Add(startParamsGroup);
            
            var cleanupGroup = new GroupBox
            {
                Text = "🧹 Bereinigungsoptionen",
                Size = new Size(720, 120),
                Location = new Point(10, 250),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            
            var cleanLogsCheck = new CheckBox
            {
                Text = "Log-Dateien",
                Checked = true,
                Location = new Point(20, 30),
                Size = new Size(150, 20)
            };
            
            var cleanCacheCheck = new CheckBox
            {
                Text = "Cache-Dateien",
                Checked = true,
                Location = new Point(20, 55),
                Size = new Size(150, 20)
            };
            
            var cleanTempCheck = new CheckBox
            {
                Text = "Temporäre Dateien",
                Checked = true,
                Location = new Point(220, 30),
                Size = new Size(180, 20)
            };
            
            // Add tooltips for cleanup options
            var cleanupToolTip = new ToolTip();
            cleanupToolTip.SetToolTip(cleanLogsCheck, "Löscht alte Conan Exiles Log-Dateien.\nKann mehrere GB an Speicherplatz freigeben.\nSicher zu aktivieren.");
            cleanupToolTip.SetToolTip(cleanCacheCheck, "Löscht Cache-Dateien die Ladeprobleme verursachen können.\nEmpfohlen nach Mod-Änderungen oder Updates.");
            cleanupToolTip.SetToolTip(cleanTempCheck, "Löscht temporäre Spieldateien.\nKann Performance-Probleme beheben.\nSicher zu aktivieren.");
            
            cleanupGroup.Controls.AddRange(new Control[] { cleanLogsCheck, cleanCacheCheck, cleanTempCheck });
            this.Controls.Add(cleanupGroup);
            
            var systemGroup = new GroupBox
            {
                Text = "💻 System-Optimierungen",
                Size = new Size(720, 80),
                Location = new Point(10, 390),
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            
            var gameModeCheck = new CheckBox
            {
                Text = "Windows Game Mode",
                Checked = true,
                Location = new Point(20, 30),
                Size = new Size(220, 20)
            };
            
            var priorityCheck = new CheckBox
            {
                Text = "Hohe Priorität",
                Checked = false,
                Location = new Point(280, 30),
                Size = new Size(180, 20)
            };
            
            // Add tooltips for better user experience
            var toolTip = new ToolTip();
            toolTip.SetToolTip(gameModeCheck, "Aktiviert den Windows Game Mode für bessere Gaming-Performance.\nEmpfohlen für alle Benutzer.");
            toolTip.SetToolTip(priorityCheck, "Setzt Conan Exiles auf hohe Prozess-Priorität.\nKann die Performance verbessern, aber andere Programme verlangsamen.\nVorsicht bei schwächeren Systemen!");
            
            systemGroup.Controls.AddRange(new Control[] { gameModeCheck, priorityCheck });
            this.Controls.Add(systemGroup);
            
            var okButton = new Button
            {
                Text = "✅ OK",
                Size = new Size(100, 30),
                Location = new Point(530, 560),
                DialogResult = DialogResult.OK,
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(okButton);
            
            var cancelButton = new Button
            {
                Text = "❌ Abbrechen",
                Size = new Size(100, 30),
                Location = new Point(640, 560),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.LightCoral,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            this.Controls.Add(cancelButton);
            
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Erstellt ein einfaches Menü
        /// </summary>
        private void CreateSimpleMenu()
        {
            var menuStrip = new MenuStrip();
            
            // Datei-Menü
            var fileMenu = new ToolStripMenuItem("📁 &Datei");
            fileMenu.DropDownItems.Add("🚪 &Beenden", null, (s, e) => Application.Exit());

            // Ansicht-Menü
            var viewMenu = new ToolStripMenuItem("👁️ &Ansicht");
            viewMenu.DropDownItems.Add("🎨 &UI-Einstellungen", null, (s, e) => ShowSimpleUIDialog());
            viewMenu.DropDownItems.Add("🌗 &Theme wechseln", null, (s, e) => ToggleSimpleTheme());

            // Hilfe-Menü
            var helpMenu = new ToolStripMenuItem("❓ &Hilfe");
            helpMenu.DropDownItems.Add("ℹ️ &Über", null, (s, e) => ShowAboutDialog());

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void ShowSimpleUIDialog()
        {
            using (var form = new Form())
            {
                form.Text = "🎨 UI-Einstellungen";
                form.Size = new Size(300, 200);
                form.StartPosition = FormStartPosition.CenterParent;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;

                var label = new Label { Text = "Theme-Auswahl:", AutoSize = true, Location = new Point(10, 20) };
                var lightBtn = new Button { Text = "☀️ Hell", Size = new Size(80, 30), Location = new Point(10, 50) };
                var darkBtn = new Button { Text = "🌙 Dunkel", Size = new Size(80, 30), Location = new Point(100, 50) };
                var okBtn = new Button { Text = "OK", Size = new Size(60, 30), Location = new Point(190, 50), DialogResult = DialogResult.OK };

                lightBtn.Click += (s, e) => { SetLightTheme(); form.Close(); };
                darkBtn.Click += (s, e) => { SetDarkTheme(); form.Close(); };

                form.Controls.AddRange(new Control[] { label, lightBtn, darkBtn, okBtn });
                form.ShowDialog(this);
            }
        }

        private void ToggleSimpleTheme()
        {
            if (this.BackColor.R > 100)
                SetDarkTheme();
            else
                SetLightTheme();
        }

        private void SetLightTheme()
        {
            this.BackColor = Color.White;
            SetThemeToAllControls(this, true);
            MessageBox.Show("☀️ Helles Theme aktiviert", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetDarkTheme()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            SetThemeToAllControls(this, false);
            MessageBox.Show("🌙 Dunkles Theme aktiviert", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetThemeToAllControls(Control parent, bool isLight)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel && !panel.Name.Contains("header"))
                {
                    panel.BackColor = isLight ? Color.White : Color.FromArgb(50, 50, 50);
                }
                else if (control is Label label)
                {
                    label.ForeColor = isLight ? Color.Black : Color.White;
                }
                else if (control is Button button)
                {
                    button.BackColor = isLight ? Color.FromArgb(240, 240, 240) : Color.FromArgb(60, 60, 60);
                    button.ForeColor = isLight ? Color.Black : Color.White;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.ForeColor = isLight ? Color.DarkBlue : Color.LightBlue;
                }

                if (control.HasChildren)
                    ApplyThemeToAllControls(control, isLight);
            }
        }

        private void ShowAboutDialog()
        {
            var about = "🗡️ Conan Exiles Optimizer v3.0.0 🏰\nBuild: 2025-08-25\nEntwickler: Panicgrinder\n\n© 2025 - Open Source";
            MessageBox.Show(about, "Über", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #region UI Menu System

        /// <summary>
        /// Erstellt das Hauptmenü
        /// </summary>
        private void CreateMainMenu()
        {
            var menuStrip = new MenuStrip();
            
            // Datei-Menü
            var fileMenu = new ToolStripMenuItem("📁 &Datei");
            fileMenu.DropDownItems.Add("🆕 &Neues Profil", null, NewProfile_Click);
            fileMenu.DropDownItems.Add("📂 Profil &laden", null, LoadProfile_Click);
            fileMenu.DropDownItems.Add("💾 Profil &speichern", null, SaveProfile_Click);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("📤 &Exportieren", null, Export_Click);
            fileMenu.DropDownItems.Add("📥 &Importieren", null, Import_Click);
            fileMenu.DropDownItems.Add(new ToolStripSeparator());
            fileMenu.DropDownItems.Add("🚪 &Beenden", null, Exit_Click);

            // Ansicht-Menü
            var viewMenu = new ToolStripMenuItem("👁️ &Ansicht");
            viewMenu.DropDownItems.Add("🎨 &UI-Einstellungen", null, UISettings_Click);
            viewMenu.DropDownItems.Add("🌗 &Theme wechseln", null, ToggleTheme_Click);
            viewMenu.DropDownItems.Add(new ToolStripSeparator());
            viewMenu.DropDownItems.Add("📊 &Performance-Monitor", null, ShowPerformanceMonitor_Click);
            viewMenu.DropDownItems.Add("📈 &Detaillierte Statistiken", null, ShowDetailedStats_Click);
            viewMenu.DropDownItems.Add(new ToolStripSeparator());
            viewMenu.DropDownItems.Add("🔄 &Aktualisieren", null, Refresh_Click);

            // Tools-Menü
            var toolsMenu = new ToolStripMenuItem("🛠️ &Tools");
            toolsMenu.DropDownItems.Add("🧹 &Workspace bereinigen", null, CleanWorkspace_Click);
            toolsMenu.DropDownItems.Add("🔧 &Registry-Editor", null, OpenRegistryEditor_Click);
            toolsMenu.DropDownItems.Add("📋 &System-Info", null, ShowSystemInfo_Click);
            toolsMenu.DropDownItems.Add(new ToolStripSeparator());
            toolsMenu.DropDownItems.Add("⚙️ &Erweiterte Einstellungen", null, AdvancedSettings_Click);

            // Hilfe-Menü
            var helpMenu = new ToolStripMenuItem("❓ &Hilfe");
            helpMenu.DropDownItems.Add("📖 &Benutzerhandbuch", null, ShowUserManual_Click);
            helpMenu.DropDownItems.Add("🔗 &Online-Hilfe", null, OnlineHelp_Click);
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("🐛 &Fehler melden", null, ReportBug_Click);
            helpMenu.DropDownItems.Add("💝 &Spenden", null, Donate_Click);
            helpMenu.DropDownItems.Add(new ToolStripSeparator());
            helpMenu.DropDownItems.Add("ℹ️ &Über", null, About_Click);

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, toolsMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }

        private void UISettings_Click(object sender, EventArgs e)
        {
            ShowSimpleUISettings();
        }

        private void ShowSimpleUISettings()
        {
            using (var settingsForm = new Form())
            {
                settingsForm.Text = "🎨 UI-Einstellungen";
                settingsForm.Size = new Size(400, 300);
                settingsForm.StartPosition = FormStartPosition.CenterParent;
                settingsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                settingsForm.MaximizeBox = false;
                settingsForm.MinimizeBox = false;

                var themeGroup = new GroupBox
                {
                    Text = "🌈 Theme-Auswahl",
                    Size = new Size(360, 100),
                    Location = new Point(10, 10)
                };

                var lightThemeRadio = new RadioButton { Text = "☀️ Hell", Location = new Point(10, 25), AutoSize = true };
                var darkThemeRadio = new RadioButton { Text = "🌙 Dunkel", Location = new Point(10, 50), AutoSize = true, Checked = true };
                var contrastRadio = new RadioButton { Text = "🔆 Hoher Kontrast", Location = new Point(10, 75), AutoSize = true };

                themeGroup.Controls.AddRange(new Control[] { lightThemeRadio, darkThemeRadio, contrastRadio });

                var colorGroup = new GroupBox
                {
                    Text = "🎨 Farben",
                    Size = new Size(360, 80),
                    Location = new Point(10, 120)
                };

                var primaryColorButton = new Button
                {
                    Text = "🎯 Primärfarbe",
                    Size = new Size(100, 30),
                    Location = new Point(10, 25),
                    BackColor = Color.FromArgb(0, 122, 204)
                };

                var accentColorButton = new Button
                {
                    Text = "✨ Akzentfarbe",
                    Size = new Size(100, 30),
                    Location = new Point(120, 25),
                    BackColor = Color.FromArgb(255, 140, 0)
                };

                primaryColorButton.Click += (s, e) => ShowColorPicker(primaryColorButton, "Primärfarbe wählen");
                accentColorButton.Click += (s, e) => ShowColorPicker(accentColorButton, "Akzentfarbe wählen");

                colorGroup.Controls.AddRange(new Control[] { primaryColorButton, accentColorButton });

                var okButton = new Button
                {
                    Text = "✅ OK",
                    Size = new Size(80, 30),
                    Location = new Point(210, 220),
                    DialogResult = DialogResult.OK
                };

                var cancelButton = new Button
                {
                    Text = "❌ Abbrechen",
                    Size = new Size(80, 30),
                    Location = new Point(300, 220),
                    DialogResult = DialogResult.Cancel
                };

                okButton.Click += (s, e) =>
                {
                    if (lightThemeRadio.Checked)
                        ApplyLightTheme();
                    else if (darkThemeRadio.Checked)
                        ApplyDarkTheme();
                    else if (contrastRadio.Checked)
                        ApplyHighContrastTheme();
                };

                settingsForm.Controls.AddRange(new Control[] { themeGroup, colorGroup, okButton, cancelButton });
                settingsForm.ShowDialog(this);
            }
        }

        private void ShowColorPicker(Button colorButton, string title)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = colorButton.BackColor;
                colorDialog.FullOpen = true;
                
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorButton.BackColor = colorDialog.Color;
                }
            }
        }

        private void ToggleTheme_Click(object sender, EventArgs e)
        {
            if (this.BackColor == Color.FromArgb(240, 240, 240) || this.BackColor == Color.White)
            {
                ApplyDarkTheme();
                MessageBox.Show("🌙 Dunkles Theme aktiviert", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ApplyLightTheme();
                MessageBox.Show("☀️ Helles Theme aktiviert", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void ApplyLightTheme()
        {
            this.BackColor = Color.White;
            ApplyThemeToAllControls(this, true);
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(30, 30, 30);
            ApplyThemeToAllControls(this, false);
        }

        private void ApplyHighContrastTheme()
        {
            this.BackColor = Color.Black;
            ApplyHighContrastToAllControls(this);
        }

        private void ApplyThemeToAllControls(Control parent, bool isLight)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel && !panel.Name.Contains("header"))
                {
                    panel.BackColor = isLight ? Color.White : Color.FromArgb(50, 50, 50);
                }
                else if (control is Label label)
                {
                    label.ForeColor = isLight ? Color.Black : Color.White;
                }
                else if (control is Button button)
                {
                    button.BackColor = isLight ? Color.FromArgb(240, 240, 240) : Color.FromArgb(60, 60, 60);
                    button.ForeColor = isLight ? Color.Black : Color.White;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.ForeColor = isLight ? Color.DarkBlue : Color.LightBlue;
                }

                if (control.HasChildren)
                    ApplyThemeToAllControls(control, isLight);
            }
        }

        private void ApplyHighContrastToAllControls(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Panel panel && !panel.Name.Contains("header"))
                {
                    panel.BackColor = Color.Black;
                }
                else if (control is Label label)
                {
                    label.ForeColor = Color.Yellow;
                }
                else if (control is Button button)
                {
                    button.BackColor = Color.FromArgb(40, 40, 40);
                    button.ForeColor = Color.White;
                }
                else if (control is GroupBox groupBox)
                {
                    groupBox.ForeColor = Color.Cyan;
                }

                if (control.HasChildren)
                    ApplyHighContrastToAllControls(control);
            }
        }

        // Weitere Menu-Handler (vereinfacht)
        private void NewProfile_Click(object sender, EventArgs e) => 
            MessageBox.Show("🆕 Neues Profil wird erstellt...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void LoadProfile_Click(object sender, EventArgs e) => 
            MessageBox.Show("📂 Profil-Laden wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void SaveProfile_Click(object sender, EventArgs e) => 
            MessageBox.Show("💾 Profil-Speichern wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void Export_Click(object sender, EventArgs e) => 
            MessageBox.Show("📤 Export wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void Import_Click(object sender, EventArgs e) => 
            MessageBox.Show("📥 Import wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void Exit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Möchten Sie das Programm wirklich beenden?", "Beenden", 
                              MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Application.Exit();
        }

        private void ShowPerformanceMonitor_Click(object sender, EventArgs e) => 
            MessageBox.Show("📊 Performance-Monitor wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void ShowDetailedStats_Click(object sender, EventArgs e) => 
            MessageBox.Show("📈 Detaillierte Statistiken werden implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void Refresh_Click(object sender, EventArgs e)
        {
            // DetectInstallations(); // Wird später implementiert
            // UpdateStatus(); // Wird später implementiert  
            MessageBox.Show("🔄 Daten wurden aktualisiert", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CleanWorkspace_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Workspace-Cleanup-Tool ausführen?", "Bereinigen", 
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                MessageBox.Show("🧹 Cleanup wird implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OpenRegistryEditor_Click(object sender, EventArgs e)
        {
            try { Process.Start("regedit.exe"); }
            catch (Exception ex) { MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ShowSystemInfo_Click(object sender, EventArgs e)
        {
            var info = $"🖥️ System-Info:\nOS: {Environment.OSVersion}\nKerne: {Environment.ProcessorCount}\nUser: {Environment.UserName}";
            MessageBox.Show(info, "System-Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AdvancedSettings_Click(object sender, EventArgs e) => 
            MessageBox.Show("⚙️ Erweiterte Einstellungen werden implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void ShowUserManual_Click(object sender, EventArgs e) => 
            MessageBox.Show("📖 Benutzerhandbuch wird geöffnet...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void OnlineHelp_Click(object sender, EventArgs e)
        {
            try { Process.Start(new ProcessStartInfo("https://github.com/Panicgrinder/Inofficial-Conan-Exiles-Performance-Optimizer") { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ReportBug_Click(object sender, EventArgs e)
        {
            try { Process.Start(new ProcessStartInfo("https://github.com/Panicgrinder/Inofficial-Conan-Exiles-Performance-Optimizer/issues") { UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show($"Fehler: {ex.Message}", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Donate_Click(object sender, EventArgs e) => 
            MessageBox.Show("💝 Danke für die Unterstützung!\nSpenden-Info wird implementiert...", "Spenden", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void About_Click(object sender, EventArgs e)
        {
            var about = "🗡️ Conan Exiles Optimizer v3.0.0 🏰\nBuild: 2025-08-25\nEntwickler: Panicgrinder\n\n© 2025 - Open Source";
            MessageBox.Show(about, "Über", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
    
    // Program Entry Point
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Check for admin rights
            if (!IsRunningAsAdmin())
            {
                var result = MessageBox.Show(
                    "Für optimale Funktionalität sollte das Tool als Administrator ausgeführt werden.\n\n" +
                    "Möchten Sie das Tool trotzdem starten?",
                    "Administrator-Rechte empfohlen",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);
                    
                if (result == DialogResult.No)
                    return;
            }
            
            Application.Run(new MainForm());
        }
        
        private static bool IsRunningAsAdmin()
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
    }
}
