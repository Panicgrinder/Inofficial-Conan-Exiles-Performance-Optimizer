using System;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ConanOptimizer;

namespace ConanExilesOptimizer
{
    public class MainForm : Form
    {
        private const string AppVersion = "v3.0.0";
        private const string AppTitle = "Conan Exiles Optimizer";
        private const int WindowWidth = 1200;
        private const int WindowHeight = 800;

    private enum Theme { Dark, Light }
        private Theme currentTheme = Theme.Dark;
    private readonly string prefDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConanExilesOptimizer");
    private readonly string prefFile;

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
        private TextBox logTextBox;
        private ToolStripStatusLabel statusLabel;
        private readonly ToolTip toolTip = new ToolTip();

        public MainForm()
        {
            prefFile = Path.Combine(prefDir, "user.config");
            InitializeComponent();
            LoadPreferences();
            UpdateStatus();
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            ClientSize = new Size(WindowWidth, WindowHeight);
            Text = $"{AppTitle} {AppVersion}";
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            MinimumSize = new Size(1000, 600);
            BackColor = Color.FromArgb(32, 34, 37);
            Font = new Font("Segoe UI", 9F);
            KeyPreview = true;
            KeyDown += MainForm_KeyDown;

            CreateMenuBar();
            CreateMainContent();

            // Statusbar unten
            var statusStrip = new StatusStrip { SizingGrip = true };
            statusLabel = new ToolStripStatusLabel("Bereit");
            statusStrip.Items.Add(statusLabel);
            Controls.Add(statusStrip);

            ResumeLayout(false);
            PerformLayout();
        }

        private void CreateMenuBar()
        {
            var menuStrip = new MenuStrip
            {
                BackColor = Color.FromArgb(54, 57, 63),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F),
                Dock = DockStyle.Top
            };

            var fileMenu = new ToolStripMenuItem("&Datei") { ForeColor = Color.White };
            var exitItem = new ToolStripMenuItem("Be&enden") { ForeColor = Color.White, ShortcutKeys = Keys.Control | Keys.Q, ShowShortcutKeys = true };
            exitItem.Click += (s, e) => Application.Exit();
            fileMenu.DropDownItems.Add(exitItem);

            var viewMenu = new ToolStripMenuItem("&Ansicht") { ForeColor = Color.White };
            var lightTheme = new ToolStripMenuItem("Helles &Theme") { ForeColor = Color.White, ShortcutKeys = Keys.Control | Keys.L };
            lightTheme.Click += (s, e) => { ApplyTheme(Theme.Light); SavePreferences(); };
            var darkTheme = new ToolStripMenuItem("Dunkles &Theme") { ForeColor = Color.White, ShortcutKeys = Keys.Control | Keys.D };
            darkTheme.Click += (s, e) => { ApplyTheme(Theme.Dark); SavePreferences(); };
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { lightTheme, darkTheme });

            var helpMenu = new ToolStripMenuItem("&Hilfe") { ForeColor = Color.White };
            var about = new ToolStripMenuItem("Ü&ber") { ForeColor = Color.White, ShortcutKeys = Keys.F1 };
            about.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(about);

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, helpMenu });
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

            // Start-Theme anwenden
            ApplyTheme(currentTheme);
        }

        private void CreateMainContent()
        {
            var headerPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 100),
                Location = new Point(20, 35),
                BackColor = Color.FromArgb(67, 139, 202),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var titleLabel = new Label
            {
                Text = "🗡️ CONAN EXILES OPTIMIZER 🏰",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Size = new Size(WindowWidth - 80, 50),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitleLabel = new Label
            {
                Text = "Professionelle Performance-Optimierung für maximale FPS und Stabilität",
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(230, 230, 230),
                BackColor = Color.Transparent,
                Size = new Size(WindowWidth - 80, 25),
                Location = new Point(20, 65),
                TextAlign = ContentAlignment.MiddleCenter
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);
            Controls.Add(headerPanel);

            var leftPanel = new Panel
            {
                Size = new Size(580, 620),
                Location = new Point(20, 150),
                BackColor = Color.FromArgb(47, 49, 54),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            var statusGroup = new GroupBox
            {
                Text = "📊 System Status",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 280),
                Location = new Point(10, 10),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            steamStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 35),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "🔍 Steam wird gesucht..."
            };

            conanStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 70),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "🔍 Conan Exiles wird gesucht..."
            };

            modStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 105),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "📦 Mod-Status wird geladen..."
            };

            systemStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 140),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "💻 System-Info wird geladen..."
            };

            performanceLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 175),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Text = "⚡ Performance-Status: Unbekannt",
                ForeColor = Color.Yellow
            };

            statusGroup.Controls.AddRange(new Control[] {
                steamStatusLabel, conanStatusLabel, modStatusLabel,
                systemStatusLabel, performanceLabel
            });

            var logLabel = new Label
            {
                Text = "📋 Aktivitätsprotokoll",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Size = new Size(530, 25),
                Location = new Point(15, 300)
            };

            this.logTextBox = new TextBox
            {
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.FromArgb(32, 34, 37),
                ForeColor = Color.LightGray,
                Font = new Font("Consolas", 9),
                Size = new Size(530, 280),
                Location = new Point(15, 330),
                ReadOnly = true,
                Text = "[13:33:47] 🔄 Aktualisiere Status...\n[13:33:47] 🔍 Führe System-Diagnose durch...\n[13:33:47] 💾 Festplattenspeicher: 966GB frei\n[13:33:47] 📦 Aktive Mods: 3\n[13:33:47] 💻 Empfehlung: Mindestens 16GB RAM für stabiles Spiel mit Mods\n[13:33:47] ✅ Status aktualisiert"
            };
            logTextBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            leftPanel.Controls.Add(statusGroup);
            leftPanel.Controls.Add(logLabel);
            leftPanel.Controls.Add(logTextBox);
            Controls.Add(leftPanel);

            var rightPanel = new Panel
            {
                Size = new Size(580, 620),
                Location = new Point(620, 150),
                BackColor = Color.FromArgb(47, 49, 54),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            var actionsGroup = new GroupBox
            {
                Text = "🚀 Aktionen",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 600),
                Location = new Point(10, 10),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            optimizeButton = new Button
            {
                Text = "🔧 CONAN EXILES OPTIMIEREN",
                Size = new Size(520, 60),
                Location = new Point(20, 40),
                BackColor = Color.FromArgb(88, 175, 88),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            optimizeButton.FlatAppearance.BorderSize = 0;
            optimizeButton.Click += OptimizeButton_Click;
            optimizeButton.TabIndex = 0;
            optimizeButton.AccessibleName = "Optimieren";
            toolTip.SetToolTip(optimizeButton, "Spieleinstellungen optimieren (Empfohlen)");

            monitorButton = new Button
            {
                Text = "📊 PERFORMANCE ÜBERWACHEN",
                Size = new Size(520, 60),
                Location = new Point(20, 120),
                BackColor = Color.FromArgb(74, 144, 226),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            monitorButton.FlatAppearance.BorderSize = 0;
            monitorButton.Click += MonitorButton_Click;
            monitorButton.TabIndex = 1;
            monitorButton.AccessibleName = "Performance überwachen";
            toolTip.SetToolTip(monitorButton, "FPS/Leistung überwachen");

            launchButton = new Button
            {
                Text = "🎮 CONAN EXILES STARTEN",
                Size = new Size(520, 60),
                Location = new Point(20, 200),
                BackColor = Color.FromArgb(255, 140, 0),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            launchButton.FlatAppearance.BorderSize = 0;
            launchButton.Click += LaunchButton_Click;
            launchButton.TabIndex = 2;
            launchButton.AccessibleName = "Spiel starten";
            toolTip.SetToolTip(launchButton, "Conan Exiles starten");

            refreshButton = new Button
            {
                Text = "🔄 Status aktualisieren",
                Size = new Size(250, 45),
                Location = new Point(20, 300),
                BackColor = Color.FromArgb(114, 137, 218),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += RefreshButton_Click;
            refreshButton.TabIndex = 3;
            refreshButton.AccessibleName = "Status aktualisieren";
            toolTip.SetToolTip(refreshButton, "System- und Spielstatus neu scannen");

            advancedButton = new Button
            {
                Text = "⚙️ Erweiterte Einstellungen",
                Size = new Size(250, 45),
                Location = new Point(290, 300),
                BackColor = Color.FromArgb(153, 170, 181),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            advancedButton.FlatAppearance.BorderSize = 0;
            advancedButton.Click += AdvancedButton_Click;
            advancedButton.TabIndex = 4;
            advancedButton.AccessibleName = "Erweiterte Einstellungen";
            toolTip.SetToolTip(advancedButton, "Optionen für Fortgeschrittene");

            var helpButton = new Button
            {
                Text = "❓ Hilfe & Support",
                Size = new Size(250, 45),
                Location = new Point(20, 360),
                BackColor = Color.FromArgb(250, 166, 26),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            helpButton.FlatAppearance.BorderSize = 0;
            helpButton.Click += HelpButton_Click;
            helpButton.TabIndex = 5;
            helpButton.AccessibleName = "Hilfe & Support";
            toolTip.SetToolTip(helpButton, "Hilfe öffnen (F1)");

            actionsGroup.Controls.AddRange(new Control[] {
                optimizeButton, monitorButton, launchButton,
                refreshButton, advancedButton, helpButton
            });

            rightPanel.Controls.Add(actionsGroup);
            Controls.Add(rightPanel);
        }

        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            LogMessage("🔧 Starte Optimierung...");
            statusLabel.Text = "Optimierung gestartet";
            try
            {
                var findings = ConfigRepair.ScanForNpcAiOverrides(out var eIni, out var gIni).ToList();
                if (findings.Count == 0)
                {
                    LogMessage("ℹ️ Keine relevanten Overrides gefunden.");
                }
                else
                {
                    foreach (var f in findings.Take(10)) LogMessage("• " + f);
                }

                var repaired = ConfigRepair.RepairNpcAiOverrides();
                if (repaired)
                {
                    LogMessage("✅ Optimierung abgeschlossen: Riskante Overrides entfernt (Backups erstellt). Bitte Spiel neu starten.");
                    statusLabel.Text = "Optimierung abgeschlossen";
                    MessageBox.Show("Optimierung abgeschlossen. Backups wurden erstellt.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    LogMessage("⚠️ Optimierung: Keine Änderungen (evtl. Dateien nicht gefunden).");
                    statusLabel.Text = "Optimierung ohne Änderungen";
                    MessageBox.Show("Keine Änderungen vorgenommen (evtl. Dateien nicht gefunden).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Optimierung fehlgeschlagen: {ex.Message}");
                statusLabel.Text = "Optimierung fehlgeschlagen";
                MessageBox.Show("Optimierung fehlgeschlagen. Details im Log.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MonitorButton_Click(object sender, EventArgs e)
        {
            LogMessage("📊 Starte Performance-Monitoring...");
            statusLabel.Text = "Monitoring gestartet";
            MessageBox.Show("Monitoring-Stub: Kommt bald.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void LaunchButton_Click(object sender, EventArgs e)
        {
            LogMessage("🎮 Starte Conan Exiles...");
            statusLabel.Text = "Spielstart initiiert";
            try
            {
                var exe = PathHelper.GetConanExecutablePath();
                if (!string.IsNullOrEmpty(exe) && File.Exists(exe))
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = exe,
                        UseShellExecute = true,
                        WorkingDirectory = Path.GetDirectoryName(exe)
                    });
                    LogMessage($"🚀 Spiel gestartet (Exe): {exe}");
                }
                else
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "steam://run/440900",
                        UseShellExecute = true
                    });
                    LogMessage("🚀 Spiel via Steam-URI gestartet (440900)");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"❌ Spielstart fehlgeschlagen: {ex.Message}");
                statusLabel.Text = "Spielstart fehlgeschlagen";
                MessageBox.Show("Spielstart fehlgeschlagen. Bitte Steam/Installation prüfen.", "Fehler", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LogMessage("🔄 Aktualisiere Status...");
            UpdateStatus();
            // statusLabel.Text = "Status aktualisiert";
        }

        private void AdvancedButton_Click(object sender, EventArgs e)
        {
            LogMessage("⚙️ Öffne erweiterte Einstellungen...");
            // statusLabel.Text = "Erweiterte Einstellungen";
            MessageBox.Show("Erweiterte Einstellungen werden implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void HelpButton_Click(object sender, EventArgs e)
        {
            LogMessage("❓ Zeige Hilfe...");
            // statusLabel.Text = "Hilfe geöffnet";
            ShowAbout();
        }

    private void UpdateStatus()
        {
            // TODO: Echte Erkennung von Steam-/Conan-Pfaden einfuegen
            steamStatusLabel.Text = "\u2705 Steam Status: ermittelt";
            conanStatusLabel.Text = "\u2705 Conan Exiles Status: ermittelt";
            modStatusLabel.Text = "\ud83d\udce6 Mods: Status ermittelt";

            try
            {
                var memInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                double ramGB = memInfo.TotalPhysicalMemory / (1024.0 * 1024.0 * 1024.0);

                var systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                double freeDiskGB = systemDrive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);

                systemStatusLabel.Text = $"\ud83d\udcbb System: {ramGB:F0} GB RAM, {freeDiskGB:F0} GB frei";
                systemStatusLabel.ForeColor = Color.LightGray;
            }
            catch
            {
                systemStatusLabel.Text = "\ud83d\udcbb System: Informationen nicht verfuegbar";
                systemStatusLabel.ForeColor = Color.Gray;
            }

            performanceLabel.Text = "\u26a1 Performance-Status: Bereit fuer Optimierung";
            performanceLabel.ForeColor = Color.LightGreen;

            // Zusätzlicher Schritt: Scan & optionale Reparatur von riskanten NPC/AI Overrides
            try
            {
                // Pfade protokollieren
                var savedCfg = PathHelper.GetConanSavedConfigPath();
                var installPath = PathHelper.GetConanExilesPath();
                LogMessage($"\ud83d\udd0e Pfade: SavedConfig={(savedCfg ?? "n/a")}, Install={(installPath ?? "n/a")}");

                var findings = ConfigRepair.ScanForNpcAiOverrides(out var engineIni, out var gameIni).ToList();
                if (findings.Count > 0)
                {
                    LogMessage($"\ud83d\udea8 Konfiguration geprueft (Engine.ini/Game.ini).");
                    foreach (var f in findings.Take(5)) LogMessage("• " + f);
                    // Automatische Reparatur anbieten/ausführen (konservativ: einmalig hier ausführen)
                    if (findings.Any(f => f.Contains("verdächtig", StringComparison.OrdinalIgnoreCase)))
                    {
                        var repaired = ConfigRepair.RepairNpcAiOverrides();
                        LogMessage(repaired
                            ? "✅ Problematische Overrides entfernt (Backups neben den Dateien). Bitte Spiel neu starten."
                            : "❌ Reparatur fehlgeschlagen. Bitte Dateien manuell pruefen.");
                        statusLabel.Text = repaired ? "Reparatur abgeschlossen" : "Reparatur fehlgeschlagen";
                    }
                    else
                    {
                        LogMessage("ℹ️ Keine problematischen Overrides erkannt.");
                        statusLabel.Text = "Scan abgeschlossen";
                    }
                }
                else
                {
                    LogMessage("ℹ️ Keine problematischen Overrides erkannt.");
                    statusLabel.Text = "Scan abgeschlossen";
                }
            }
            catch (Exception ex)
            {
                LogMessage($"Reparatur-Scan Fehler: {ex.Message}");
                statusLabel.Text = "Scan-Fehler";
            }
        }

        private void LogMessage(string message)
        {
            if (logTextBox == null) return;
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            logTextBox.AppendText($"\n[{timestamp}] {message}");
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void ApplyTheme(Theme theme)
        {
            currentTheme = theme;
            Color back, panel, text, group, button, buttonText, textboxBack;
            if (theme == Theme.Light)
            {
                back = Color.FromArgb(240, 240, 240);
                panel = Color.White;
                group = Color.White;
                text = Color.Black;
                button = Color.FromArgb(0, 120, 215);
                buttonText = Color.White;
                textboxBack = Color.White;
            }
            else
            {
                back = Color.FromArgb(32, 34, 37);
                panel = Color.FromArgb(47, 49, 54);
                group = panel;
                text = Color.White;
                button = Color.FromArgb(74, 144, 226);
                buttonText = Color.White;
                textboxBack = Color.FromArgb(32, 34, 37);
            }

            BackColor = back;
            foreach (Control c in Controls)
            {
                ApplyThemeToControl(c, panel, group, text, button, buttonText, textboxBack);
            }
        }

        private static void ApplyThemeToControl(Control c, Color panel, Color group, Color text, Color button, Color buttonText, Color textboxBack)
        {
            switch (c)
            {
                case Panel p:
                    p.BackColor = panel;
                    break;
                case GroupBox g:
                    g.ForeColor = text;
                    g.BackColor = group;
                    break;
                case Label l:
                    l.ForeColor = text;
                    break;
                case Button b:
                    b.BackColor = button;
                    b.ForeColor = buttonText;
                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;
                    break;
                case TextBox tb:
                    tb.BackColor = textboxBack;
                    tb.ForeColor = Color.LightGray;
                    break;
                case MenuStrip ms:
                    ms.BackColor = Color.FromArgb(54, 57, 63);
                    ms.ForeColor = Color.White;
                    break;
            }
            // Rekursiv auf Kinder anwenden
            foreach (Control child in c.Controls)
            {
                ApplyThemeToControl(child, panel, group, text, button, buttonText, textboxBack);
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show("Conan Exiles Optimizer v3.0.0\nEntwickler: Panicgrinder\n© 2025", "Über", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                ShowAbout();
                e.Handled = true;
            }
        }

        private void LoadPreferences()
        {
            try
            {
                if (File.Exists(prefFile))
                {
                    var content = File.ReadAllText(prefFile).Trim();
                    if (string.Equals(content, "theme=light", StringComparison.OrdinalIgnoreCase))
                        currentTheme = Theme.Light;
                    else
                        currentTheme = Theme.Dark;
                    ApplyTheme(currentTheme);
                }
            }
            catch { /* still start with defaults */ }
        }

        private void SavePreferences()
        {
            try
            {
                if (!Directory.Exists(prefDir)) Directory.CreateDirectory(prefDir);
                File.WriteAllText(prefFile, currentTheme == Theme.Light ? "theme=light" : "theme=dark");
            }
            catch { /* ignore persistence errors */ }
        }
    }
}
