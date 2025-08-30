using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ConanOptimizer.OfficialBeta
{
    public class MainForm : Form
    {
    private const string AppVersion = "v3.3.0-beta";
    private const string AppTitle = "Conan Exiles Optimizer – Official Beta";
        private const int WindowWidth = 1200;
        private const int WindowHeight = 800;

        private enum Theme { Dark, Light }
        private Theme currentTheme = Theme.Dark;
        private readonly string prefDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ConanExilesOptimizerBeta");
        private readonly string prefFile;

        private Label steamStatusLabel;
        private Label conanStatusLabel;
        private Label systemStatusLabel;
        private Label performanceLabel;
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
            var about = new ToolStripMenuItem("\u00dc&ber") { ForeColor = Color.White, ShortcutKeys = Keys.F1 };
            about.Click += (s, e) => ShowAbout();
            helpMenu.DropDownItems.Add(about);

            menuStrip.Items.AddRange(new ToolStripItem[] { fileMenu, viewMenu, helpMenu });
            MainMenuStrip = menuStrip;
            Controls.Add(menuStrip);

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
                Text = "CONAN EXILES OPTIMIZER",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Size = new Size(WindowWidth - 80, 50),
                Location = new Point(20, 15),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var subtitleLabel = new Label
            {
                Text = "Offizielle Beta – Performante und sichere Optimierung",
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
                Text = "System Status",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 220),
                Location = new Point(10, 10),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            steamStatusLabel = new Label { Size = new Size(530, 30), Location = new Point(15, 35), Font = new Font("Segoe UI", 10), ForeColor = Color.LightGray, Text = "Steam wird gesucht..." };
            conanStatusLabel = new Label { Size = new Size(530, 30), Location = new Point(15, 70), Font = new Font("Segoe UI", 10), ForeColor = Color.LightGray, Text = "Conan Exiles wird gesucht..." };
            systemStatusLabel = new Label { Size = new Size(530, 30), Location = new Point(15, 105), Font = new Font("Segoe UI", 10), ForeColor = Color.LightGray, Text = "System-Info wird geladen..." };
            performanceLabel = new Label { Size = new Size(530, 30), Location = new Point(15, 140), Font = new Font("Segoe UI", 10, FontStyle.Bold), Text = "Performance-Status: Unbekannt", ForeColor = Color.Yellow };

            statusGroup.Controls.AddRange(new Control[] { steamStatusLabel, conanStatusLabel, systemStatusLabel, performanceLabel });

            var logLabel = new Label { Text = "Aktivit\u00e4tsprotokoll", Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.White, Size = new Size(530, 25), Location = new Point(15, 240) };
            logTextBox = new TextBox { Multiline = true, ScrollBars = ScrollBars.Vertical, BackColor = Color.FromArgb(32, 34, 37), ForeColor = Color.LightGray, Font = new Font("Consolas", 9), Size = new Size(530, 320), Location = new Point(15, 270), ReadOnly = true, Text = "[00:00:00] Initialisiert" };
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
                Text = "Aktionen",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(560, 600),
                Location = new Point(10, 10),
                ForeColor = Color.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };

            var optimizeButton = new Button { Text = "CONAN EXILES OPTIMIEREN", Size = new Size(520, 60), Location = new Point(20, 40), BackColor = Color.FromArgb(88, 175, 88), ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            optimizeButton.FlatAppearance.BorderSize = 0;
            optimizeButton.Click += OptimizeButton_Click;

            var helpButton = new Button { Text = "Hilfe & Info", Size = new Size(250, 45), Location = new Point(20, 120), BackColor = Color.FromArgb(250, 166, 26), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            helpButton.FlatAppearance.BorderSize = 0;
            helpButton.Click += (s, e) => ShowAbout();

            actionsGroup.Controls.AddRange(new Control[] { optimizeButton, helpButton });
            rightPanel.Controls.Add(actionsGroup);
            Controls.Add(rightPanel);
        }

        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            LogMessage("Starte Optimierung (offizielle Beta)...");
            statusLabel.Text = "Optimierung gestartet";
            MessageBox.Show("Optimierung gestartet (Beta).", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void UpdateStatus()
        {
            steamStatusLabel.Text = "Steam Status: ermittelt";
            conanStatusLabel.Text = "Conan Exiles Status: ermittelt";

            try
            {
                var memInfo = new Microsoft.VisualBasic.Devices.ComputerInfo();
                double ramGB = memInfo.TotalPhysicalMemory / (1024.0 * 1024.0 * 1024.0);

                var systemDrive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory));
                double freeDiskGB = systemDrive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);

                systemStatusLabel.Text = $"System: {ramGB:F0} GB RAM, {freeDiskGB:F0} GB frei";
                systemStatusLabel.ForeColor = Color.LightGray;
            }
            catch
            {
                systemStatusLabel.Text = "System: Informationen nicht verfuegbar";
                systemStatusLabel.ForeColor = Color.Gray;
            }

            performanceLabel.Text = "Performance-Status: Bereit fuer Optimierung";
            performanceLabel.ForeColor = Color.LightGreen;
        }

        private void LogMessage(string message)
        {
            if (logTextBox == null) return;
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            logTextBox.AppendText($"\n[{timestamp}] {message}");
            logTextBox.SelectionStart = logTextBox.Text.Length;
            logTextBox.ScrollToCaret();
        }

        private void LoadPreferences()
        {
            try
            {
                if (File.Exists(prefFile))
                {
                    var content = File.ReadAllText(prefFile).Trim();
                    currentTheme = string.Equals(content, "theme=light", StringComparison.OrdinalIgnoreCase)
                        ? Theme.Light : Theme.Dark;
                    ApplyTheme(currentTheme);
                }
            }
            catch { /* ignore */ }
        }

        private void SavePreferences()
        {
            try
            {
                if (!Directory.Exists(prefDir)) Directory.CreateDirectory(prefDir);
                File.WriteAllText(prefFile, currentTheme == Theme.Light ? "theme=light" : "theme=dark");
            }
            catch { /* ignore */ }
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
            foreach (Control child in c.Controls)
            {
                ApplyThemeToControl(child, panel, group, text, button, buttonText, textboxBack);
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                ShowAbout();
                e.Handled = true;
            }
        }

        private void ShowAbout()
        {
            MessageBox.Show("Conan Exiles Optimizer – Official Beta\nEntwickler: Panicgrinder\n\u00A9 2025", "\u00dcber", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        
    }
}
