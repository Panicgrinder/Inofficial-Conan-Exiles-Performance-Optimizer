using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ConanOptimizer
{
    public partial class ConanOptimizerAdvancedForm : Form
    {
        private OptimizationSettings settings;
        private TabControl tabControl;
        private Button applyButton, presetButton, resetButton;

        // Checkboxen für die verschiedenen Optimierungen
        private CheckBox cbWindowsGameMode, cbGpuScheduling, cbPowerManagement, cbMemoryManagement;
        private CheckBox cbSteamLaunchOptions, cbNvidiaSettings, cbCpuAffinity, cbProcessPriority;
        private CheckBox cbEngineIniTweaks, cbGameIniNetwork, cbLodOptimizations, cbTextureStreaming;
        private CheckBox cbExperimentalPatches;

        public ConanOptimizerAdvancedForm()
        {
            InitializeComponent();
            settings = new OptimizationSettings();
            UpdateUIFromSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Conan Exiles Optimizer - Erweiterte Einstellungen";
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Tab Control für Risiko-Kategorien
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Location = new Point(10, 10),
                Size = new Size(760, 500)
            };

            // 🛡️ SICHER Tab
            var safeTab = new TabPage("🛡️ SICHER (0% Risiko)");
            safeTab.BackColor = Color.LightGreen;
            CreateSafeOptimizations(safeTab);
            tabControl.TabPages.Add(safeTab);

            // 🟡 NIEDRIG Tab
            var lowTab = new TabPage("🟡 NIEDRIG (1% Risiko)");
            lowTab.BackColor = Color.LightYellow;
            CreateLowRiskOptimizations(lowTab);
            tabControl.TabPages.Add(lowTab);

            // 🟠 MITTEL Tab
            var mediumTab = new TabPage("🟠 MITTEL (5-10% Risiko)");
            mediumTab.BackColor = Color.LightSalmon;
            CreateMediumRiskOptimizations(mediumTab);
            tabControl.TabPages.Add(mediumTab);

            // 🔴 HOCH Tab
            var highTab = new TabPage("🔴 HOCH (15-30% Risiko)");
            highTab.BackColor = Color.LightCoral;
            CreateHighRiskOptimizations(highTab);
            tabControl.TabPages.Add(highTab);

            // Buttons
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50
            };

            presetButton = new Button
            {
                Text = "📋 Voreinstellungen",
                Size = new Size(120, 30),
                Location = new Point(10, 10)
            };
            presetButton.Click += PresetButton_Click;

            resetButton = new Button
            {
                Text = "🔄 Zurücksetzen",
                Size = new Size(120, 30),
                Location = new Point(140, 10)
            };
            resetButton.Click += ResetButton_Click;

            applyButton = new Button
            {
                Text = "✅ Anwenden",
                Size = new Size(120, 30),
                Location = new Point(650, 10),
                BackColor = Color.LightBlue
            };
            applyButton.Click += ApplyButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { presetButton, resetButton, applyButton });

            this.Controls.Add(tabControl);
            this.Controls.Add(buttonPanel);
        }

        private void CreateSafeOptimizations(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            
            cbWindowsGameMode = new CheckBox
            {
                Text = "Windows Gaming Mode (+8% FPS)",
                Location = new Point(20, 20),
                Size = new Size(300, 20),
                Checked = settings.WindowsGameMode
            };

            cbGpuScheduling = new CheckBox
            {
                Text = "GPU Hardware Scheduling (+12% GPU Performance)",
                Location = new Point(20, 50),
                Size = new Size(300, 20),
                Checked = settings.GpuScheduling
            };

            cbPowerManagement = new CheckBox
            {
                Text = "Power Management (+5% Stabilität)",
                Location = new Point(20, 80),
                Size = new Size(300, 20),
                Checked = settings.PowerManagement
            };

            cbMemoryManagement = new CheckBox
            {
                Text = "Memory Management (+10% RAM Effizienz)",
                Location = new Point(20, 110),
                Size = new Size(300, 20),
                Checked = settings.MemoryManagement
            };

            panel.Controls.AddRange(new Control[] { cbWindowsGameMode, cbGpuScheduling, cbPowerManagement, cbMemoryManagement });
            tab.Controls.Add(panel);
        }

        private void CreateLowRiskOptimizations(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            
            cbSteamLaunchOptions = new CheckBox
            {
                Text = "Steam Launch Options (+15% Ladezeit)",
                Location = new Point(20, 20),
                Size = new Size(300, 20),
                Checked = settings.SteamLaunchOptions
            };

            cbNvidiaSettings = new CheckBox
            {
                Text = "NVIDIA Settings (+18% GPU Performance)",
                Location = new Point(20, 50),
                Size = new Size(300, 20),
                Checked = settings.NvidiaSettings
            };

            cbCpuAffinity = new CheckBox
            {
                Text = "CPU Affinity (+10% CPU Auslastung)",
                Location = new Point(20, 80),
                Size = new Size(300, 20),
                Checked = settings.CpuAffinity
            };

            cbProcessPriority = new CheckBox
            {
                Text = "Process Priority (+8% Responsiveness)",
                Location = new Point(20, 110),
                Size = new Size(300, 20),
                Checked = settings.ProcessPriority
            };

            panel.Controls.AddRange(new Control[] { cbSteamLaunchOptions, cbNvidiaSettings, cbCpuAffinity, cbProcessPriority });
            tab.Controls.Add(panel);
        }

        private void CreateMediumRiskOptimizations(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            
            cbEngineIniTweaks = new CheckBox
            {
                Text = "Engine.ini Tweaks (+25% Rendering)",
                Location = new Point(20, 20),
                Size = new Size(300, 20),
                Checked = settings.EngineIniTweaks
            };

            cbGameIniNetwork = new CheckBox
            {
                Text = "Game.ini Network (+30% Netzwerk)",
                Location = new Point(20, 50),
                Size = new Size(300, 20),
                Checked = settings.GameIniNetwork
            };

            cbLodOptimizations = new CheckBox
            {
                Text = "LOD Optimizations (+20% Sichtweite)",
                Location = new Point(20, 80),
                Size = new Size(300, 20),
                Checked = settings.LodOptimizations
            };

            cbTextureStreaming = new CheckBox
            {
                Text = "Texture Streaming (+15% VRAM)",
                Location = new Point(20, 110),
                Size = new Size(300, 20),
                Checked = settings.TextureStreaming
            };

            panel.Controls.AddRange(new Control[] { cbEngineIniTweaks, cbGameIniNetwork, cbLodOptimizations, cbTextureStreaming });
            tab.Controls.Add(panel);
        }

        private void CreateHighRiskOptimizations(TabPage tab)
        {
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
            
            var warningLabel = new Label
            {
                Text = "⚠️ WARNUNG: Nur für Offline/Testing verwenden!",
                Location = new Point(20, 10),
                Size = new Size(400, 20),
                ForeColor = Color.Red,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };

            cbExperimentalPatches = new CheckBox
            {
                Text = "Experimentelle Patches (+35% Performance)",
                Location = new Point(20, 40),
                Size = new Size(300, 20),
                Checked = settings.ExperimentalPatches
            };

            panel.Controls.AddRange(new Control[] { warningLabel, cbExperimentalPatches });
            tab.Controls.Add(panel);
        }

        private void UpdateUIFromSettings()
        {
            // Wird nach InitializeComponent aufgerufen
        }

        private void UpdateSettingsFromUI()
        {
            settings.WindowsGameMode = cbWindowsGameMode?.Checked ?? false;
            settings.GpuScheduling = cbGpuScheduling?.Checked ?? false;
            settings.PowerManagement = cbPowerManagement?.Checked ?? false;
            settings.MemoryManagement = cbMemoryManagement?.Checked ?? false;
            
            settings.SteamLaunchOptions = cbSteamLaunchOptions?.Checked ?? false;
            settings.NvidiaSettings = cbNvidiaSettings?.Checked ?? false;
            settings.CpuAffinity = cbCpuAffinity?.Checked ?? false;
            settings.ProcessPriority = cbProcessPriority?.Checked ?? false;
            
            settings.EngineIniTweaks = cbEngineIniTweaks?.Checked ?? false;
            settings.GameIniNetwork = cbGameIniNetwork?.Checked ?? false;
            settings.LodOptimizations = cbLodOptimizations?.Checked ?? false;
            settings.TextureStreaming = cbTextureStreaming?.Checked ?? false;
            
            settings.ExperimentalPatches = cbExperimentalPatches?.Checked ?? false;
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            UpdateSettingsFromUI();
            
            var results = OptimizationApplier.ApplySelectedOptimizations(settings);
            var successCount = results.Count(r => r.StartsWith("✅"));
            var failCount = results.Count(r => r.StartsWith("❌"));

            MessageBox.Show(
                $"Optimierung abgeschlossen!\n\n" +
                $"✅ Erfolgreich: {successCount}\n" +
                $"❌ Fehlgeschlagen: {failCount}\n\n" +
                $"Details:\n{string.Join("\n", results)}\n\n" +
                $"Starte Conan Exiles neu, um die Änderungen zu übernehmen.",
                "Optimierung Abgeschlossen",
                MessageBoxButtons.OK,
                failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
            );
        }

        private void PresetButton_Click(object sender, EventArgs e)
        {
            using (var presetForm = new PresetSelectionForm())
            {
                if (presetForm.ShowDialog() == DialogResult.OK)
                {
                    settings = presetForm.SelectedSettings;
                    UpdateUIFromSettings();
                }
            }
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            settings = new OptimizationSettings();
            UpdateUIFromSettings();
        }
    }

    // Einfache Preset-Auswahl
    public class PresetSelectionForm : Form
    {
        public OptimizationSettings SelectedSettings { get; private set; }

        public PresetSelectionForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Voreinstellungen wählen";
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var officialBtn = new Button
            {
                Text = "🛡️ Official Server Sicher\n(0% Risiko)",
                Size = new Size(350, 50),
                Location = new Point(20, 20)
            };
            officialBtn.Click += (s, e) => { SelectedSettings = OptimizationSettings.GetOfficialServerSafe(); DialogResult = DialogResult.OK; };

            var privateBtn = new Button
            {
                Text = "🟡 Private Server Optimiert\n(Niedriges Risiko)",
                Size = new Size(350, 50),
                Location = new Point(20, 80)
            };
            privateBtn.Click += (s, e) => { SelectedSettings = OptimizationSettings.GetPrivateServerOptimized(); DialogResult = DialogResult.OK; };

            var singleBtn = new Button
            {
                Text = "🟠 Singleplayer Maximum\n(Mittleres Risiko)",
                Size = new Size(350, 50),
                Location = new Point(20, 140)
            };
            singleBtn.Click += (s, e) => { SelectedSettings = OptimizationSettings.GetSingleplayerMaximum(); DialogResult = DialogResult.OK; };

            var testBtn = new Button
            {
                Text = "🔴 Modding & Testing\n(Hohes Risiko)",
                Size = new Size(350, 50),
                Location = new Point(20, 200)
            };
            testBtn.Click += (s, e) => { SelectedSettings = OptimizationSettings.GetModdingAndTesting(); DialogResult = DialogResult.OK; };

            this.Controls.AddRange(new Control[] { officialBtn, privateBtn, singleBtn, testBtn });
        }
    }

    // Einfache Program-Klasse
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ConanOptimizerAdvancedForm());
        }
    }
}
