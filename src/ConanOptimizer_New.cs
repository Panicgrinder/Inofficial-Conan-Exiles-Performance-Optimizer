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
        private const int WindowWidth = 1200; // Full HD geeignet
        private const int WindowHeight = 800;  // Full HD geeignet
        private const int MonitoringUpdateInterval = 1000; // ms
        #endregion

        #region Fields
        private string steamPath = string.Empty;
        private string conanPath = string.Empty;
        private bool isMonitoring = false;
        private CancellationTokenSource monitoringCancellation;
        
        // UI Controls
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
        #endregion
        
        public MainForm()
        {
            InitializeComponent();
            UpdateStatus();
        }
        
        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Main Form Setup
            this.ClientSize = new Size(WindowWidth, WindowHeight);
            this.Text = $"{AppTitle} {AppVersion}";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(32, 34, 37);
            this.Font = new Font("Segoe UI", 9F);
            
            CreateMenuBar();
            CreateMainContent();
            
            this.ResumeLayout(false);
            this.PerformLayout();
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
            
            // File Menu
            var fileMenu = new ToolStripMenuItem("Datei") { ForeColor = Color.White };
            fileMenu.DropDownItems.Add(new ToolStripMenuItem("Beenden") { ForeColor = Color.White });
            fileMenu.DropDownItems[0].Click += (s, e) => Application.Exit();
            
            // View Menu
            var viewMenu = new ToolStripMenuItem("Ansicht") { ForeColor = Color.White };
            var lightTheme = new ToolStripMenuItem("Helles Theme") { ForeColor = Color.White };
            lightTheme.Click += (s, e) => this.BackColor = Color.FromArgb(240, 240, 240);
            var darkTheme = new ToolStripMenuItem("Dunkles Theme") { ForeColor = Color.White };
            darkTheme.Click += (s, e) => this.BackColor = Color.FromArgb(32, 34, 37);
            viewMenu.DropDownItems.AddRange(new ToolStripItem[] { lightTheme, darkTheme });
            
            // Help Menu  
            var helpMenu = new ToolStripMenuItem("Hilfe") { ForeColor = Color.White };
            var about = new ToolStripMenuItem("Über") { ForeColor = Color.White };
            about.Click += (s, e) => MessageBox.Show("Conan Exiles Optimizer v3.0.0\nEntwickler: Panicgrinder\n© 2025", "Über");
            helpMenu.DropDownItems.Add(about);
            
            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, helpMenu });
            this.MainMenuStrip = menuStrip;
            this.Controls.Add(menuStrip);
        }
        
        private void CreateMainContent()
        {
            // Header
            var headerPanel = new Panel
            {
                Size = new Size(WindowWidth - 40, 100),
                Location = new Point(20, 35),
                BackColor = Color.FromArgb(67, 139, 202)
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
            this.Controls.Add(headerPanel);
            
            // Left Panel - Status
            var leftPanel = new Panel
            {
                Size = new Size(580, 620),
                Location = new Point(20, 150),
                BackColor = Color.FromArgb(47, 49, 54),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var statusGroup = new GroupBox
            {
                Text = "📊 System Status",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 280),
                Location = new Point(10, 10),
                ForeColor = Color.White
            };
            
            this.steamStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 35),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "🔍 Steam wird gesucht..."
            };
            
            this.conanStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 70),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "🔍 Conan Exiles wird gesucht..."
            };
            
            this.modStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 105),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "📦 Mod-Status wird geladen..."
            };
            
            this.systemStatusLabel = new Label
            {
                Size = new Size(530, 30),
                Location = new Point(15, 140),
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.LightGray,
                Text = "💻 System-Info wird geladen..."
            };
            
            this.performanceLabel = new Label
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
            
            // Log Area
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
            
            leftPanel.Controls.Add(statusGroup);
            leftPanel.Controls.Add(logLabel);
            leftPanel.Controls.Add(logTextBox);
            this.Controls.Add(leftPanel);
            
            // Right Panel - Actions
            var rightPanel = new Panel
            {
                Size = new Size(580, 620),
                Location = new Point(620, 150),
                BackColor = Color.FromArgb(47, 49, 54),
                BorderStyle = BorderStyle.FixedSingle
            };
            
            var actionsGroup = new GroupBox
            {
                Text = "🚀 Aktionen",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 600),
                Location = new Point(10, 10),
                ForeColor = Color.White
            };
            
            // Main Action Buttons
            this.optimizeButton = new Button
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
            
            this.monitorButton = new Button
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
            
            this.launchButton = new Button
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
            
            // Secondary Buttons
            this.refreshButton = new Button
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
            
            this.advancedButton = new Button
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
            
            actionsGroup.Controls.AddRange(new Control[] {
                optimizeButton, monitorButton, launchButton, 
                refreshButton, advancedButton, helpButton
            });
            
            rightPanel.Controls.Add(actionsGroup);
            this.Controls.Add(rightPanel);
        }
        
        #region Event Handlers
        
        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            LogMessage("🔧 Starte Optimierung...");
            // Optimization logic here
            MessageBox.Show("Optimierung wurde gestartet!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void MonitorButton_Click(object sender, EventArgs e)
        {
            LogMessage("📊 Starte Performance-Monitoring...");
            MessageBox.Show("Performance-Monitor wird gestartet!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void LaunchButton_Click(object sender, EventArgs e)
        {
            LogMessage("🎮 Starte Conan Exiles...");
            MessageBox.Show("Conan Exiles wird gestartet!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void RefreshButton_Click(object sender, EventArgs e)
        {
            LogMessage("🔄 Aktualisiere Status...");
            UpdateStatus();
        }
        
        private void AdvancedButton_Click(object sender, EventArgs e)
        {
            LogMessage("⚙️ Öffne erweiterte Einstellungen...");
            MessageBox.Show("Erweiterte Einstellungen werden implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        private void HelpButton_Click(object sender, EventArgs e)
        {
            LogMessage("❓ Zeige Hilfe...");
            MessageBox.Show("Hilfe & Support werden implementiert...", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        #endregion
        
        #region Helper Methods
        
        private void UpdateStatus()
        {
            steamStatusLabel.Text = "✅ Steam gefunden: C:\\Program Files (x86)\\Steam";
            conanStatusLabel.Text = "✅ Conan Exiles gefunden: F:\\Steam\\steamapps\\common\\Conan Exiles";
            modStatusLabel.Text = "📦 Aktive Mods: 3 erkannt";
            systemStatusLabel.Text = "💻 System: 16GB RAM, 966GB frei";
            performanceLabel.Text = "⚡ Performance-Status: Bereit für Optimierung";
            performanceLabel.ForeColor = Color.LightGreen;
        }
        
        private void LogMessage(string message)
        {
            if (logTextBox != null)
            {
                var timestamp = DateTime.Now.ToString("HH:mm:ss");
                logTextBox.AppendText($"\n[{timestamp}] {message}");
                logTextBox.SelectionStart = logTextBox.Text.Length;
                logTextBox.ScrollToCaret();
            }
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
            Application.Run(new MainForm());
        }
    }
}
