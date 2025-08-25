using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConanExilesOptimizer.UI
{
    /// <summary>
    /// Dialog für UI-Einstellungen und Theme-Konfiguration
    /// </summary>
    public partial class UISettingsForm : Form
    {
        private UIConfig config;
        private bool suppressEvents = false;

        #region Controls
        private TabControl tabControl;
        private TabPage themeTab;
        private TabPage windowTab;
        private TabPage accessibilityTab;
        private TabPage performanceTab;

        // Theme Tab Controls
        private ComboBox themeComboBox;
        private Panel primaryColorPanel;
        private Panel accentColorPanel;
        private Panel backgroundColorPanel;
        private Panel textColorPanel;
        private CheckBox animationsCheckBox;
        private CheckBox hoverEffectsCheckBox;
        private TrackBar animationSpeedTrackBar;

        // Window Tab Controls
        private CheckBox rememberPositionCheckBox;
        private CheckBox startMaximizedCheckBox;
        private CheckBox minimizeToTrayCheckBox;
        private NumericUpDown widthNumeric;
        private NumericUpDown heightNumeric;

        // Accessibility Tab Controls
        private TrackBar fontScaleTrackBar;
        private CheckBox highContrastCheckBox;
        private CheckBox showTooltipsCheckBox;
        private CheckBox enableSoundsCheckBox;
        private ComboBox languageComboBox;

        // Performance Tab Controls
        private CheckBox showMetricsCheckBox;
        private CheckBox showDetailedStatsCheckBox;
        private NumericUpDown updateIntervalNumeric;
        private CheckBox enableGraphsCheckBox;

        // Dialog Buttons
        private Button okButton;
        private Button cancelButton;
        private Button applyButton;
        private Button resetButton;
        #endregion

        public UISettingsForm()
        {
            config = UIConfig.Load();
            InitializeComponent();
            LoadSettings();
            UIManager.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form Setup
            this.Text = "UI-Einstellungen";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;

            // Tab Control
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(10)
            };

            CreateThemeTab();
            CreateWindowTab();
            CreateAccessibilityTab();
            CreatePerformanceTab();
            CreateDialogButtons();

            this.Controls.Add(tabControl);
            this.ResumeLayout(false);
        }

        #region Tab Creation Methods

        private void CreateThemeTab()
        {
            themeTab = new TabPage("🎨 Theme & Farben");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            // Theme Selection
            panel.Controls.Add(new Label { Text = "Theme:", AutoSize = true }, 0, 0);
            themeComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };
            themeComboBox.Items.AddRange(new[] { "Hell", "Dunkel", "Hoher Kontrast", "Benutzerdefiniert" });
            themeComboBox.SelectedIndexChanged += ThemeComboBox_SelectedIndexChanged;
            panel.Controls.Add(themeComboBox, 1, 0);

            // Color Panels
            panel.Controls.Add(new Label { Text = "Primärfarbe:", AutoSize = true }, 0, 1);
            primaryColorPanel = CreateColorPanel("Primärfarbe wählen", PrimaryColorPanel_Click);
            panel.Controls.Add(primaryColorPanel, 1, 1);

            panel.Controls.Add(new Label { Text = "Akzentfarbe:", AutoSize = true }, 0, 2);
            accentColorPanel = CreateColorPanel("Akzentfarbe wählen", AccentColorPanel_Click);
            panel.Controls.Add(accentColorPanel, 1, 2);

            panel.Controls.Add(new Label { Text = "Hintergrund:", AutoSize = true }, 0, 3);
            backgroundColorPanel = CreateColorPanel("Hintergrundfarbe wählen", BackgroundColorPanel_Click);
            panel.Controls.Add(backgroundColorPanel, 1, 3);

            panel.Controls.Add(new Label { Text = "Textfarbe:", AutoSize = true }, 0, 4);
            textColorPanel = CreateColorPanel("Textfarbe wählen", TextColorPanel_Click);
            panel.Controls.Add(textColorPanel, 1, 4);

            // Animation Settings
            var animationGroup = new GroupBox { Text = "⚡ Animationen", Height = 120 };
            var animationPanel = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };

            animationsCheckBox = new CheckBox { Text = "Animationen aktivieren", AutoSize = true };
            animationPanel.Controls.Add(animationsCheckBox, 0, 0);

            hoverEffectsCheckBox = new CheckBox { Text = "Hover-Effekte", AutoSize = true };
            animationPanel.Controls.Add(hoverEffectsCheckBox, 1, 0);

            animationPanel.Controls.Add(new Label { Text = "Geschwindigkeit:", AutoSize = true }, 0, 1);
            animationSpeedTrackBar = new TrackBar
            {
                Minimum = 100,
                Maximum = 1000,
                TickFrequency = 100,
                Dock = DockStyle.Fill
            };
            animationPanel.Controls.Add(animationSpeedTrackBar, 1, 1);

            animationGroup.Controls.Add(animationPanel);
            panel.Controls.Add(animationGroup, 0, 5);
            panel.SetColumnSpan(animationGroup, 2);

            themeTab.Controls.Add(panel);
            tabControl.TabPages.Add(themeTab);
        }

        private void CreateWindowTab()
        {
            windowTab = new TabPage("🪟 Fenster");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            // Window Options
            rememberPositionCheckBox = new CheckBox { Text = "Fensterposition merken", AutoSize = true };
            panel.Controls.Add(rememberPositionCheckBox, 0, 0);

            startMaximizedCheckBox = new CheckBox { Text = "Maximiert starten", AutoSize = true };
            panel.Controls.Add(startMaximizedCheckBox, 1, 0);

            minimizeToTrayCheckBox = new CheckBox { Text = "In Systemleiste minimieren", AutoSize = true };
            panel.Controls.Add(minimizeToTrayCheckBox, 0, 1);

            // Window Size
            panel.Controls.Add(new Label { Text = "Breite:", AutoSize = true }, 0, 2);
            widthNumeric = new NumericUpDown
            {
                Minimum = 600,
                Maximum = 2000,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(widthNumeric, 1, 2);

            panel.Controls.Add(new Label { Text = "Höhe:", AutoSize = true }, 0, 3);
            heightNumeric = new NumericUpDown
            {
                Minimum = 400,
                Maximum = 1500,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(heightNumeric, 1, 3);

            windowTab.Controls.Add(panel);
            tabControl.TabPages.Add(windowTab);
        }

        private void CreateAccessibilityTab()
        {
            accessibilityTab = new TabPage("♿ Barrierefreiheit");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            // Font Scale
            panel.Controls.Add(new Label { Text = "Schriftgröße:", AutoSize = true }, 0, 0);
            fontScaleTrackBar = new TrackBar
            {
                Minimum = 50,
                Maximum = 200,
                TickFrequency = 25,
                Value = 100,
                Dock = DockStyle.Fill
            };
            fontScaleTrackBar.ValueChanged += FontScaleTrackBar_ValueChanged;
            panel.Controls.Add(fontScaleTrackBar, 1, 0);

            // Accessibility Options
            highContrastCheckBox = new CheckBox { Text = "Hoher Kontrast", AutoSize = true };
            panel.Controls.Add(highContrastCheckBox, 0, 1);

            showTooltipsCheckBox = new CheckBox { Text = "Tooltips anzeigen", AutoSize = true };
            panel.Controls.Add(showTooltipsCheckBox, 1, 1);

            enableSoundsCheckBox = new CheckBox { Text = "Sounds aktivieren", AutoSize = true };
            panel.Controls.Add(enableSoundsCheckBox, 0, 2);

            // Language
            panel.Controls.Add(new Label { Text = "Sprache:", AutoSize = true }, 0, 3);
            languageComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Dock = DockStyle.Fill
            };
            languageComboBox.Items.AddRange(new[] { "Deutsch", "English", "Français", "Español" });
            panel.Controls.Add(languageComboBox, 1, 3);

            accessibilityTab.Controls.Add(panel);
            tabControl.TabPages.Add(accessibilityTab);
        }

        private void CreatePerformanceTab()
        {
            performanceTab = new TabPage("⚡ Performance");
            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(10)
            };

            // Performance Options
            showMetricsCheckBox = new CheckBox { Text = "Performance-Metriken anzeigen", AutoSize = true };
            panel.Controls.Add(showMetricsCheckBox, 0, 0);

            showDetailedStatsCheckBox = new CheckBox { Text = "Detaillierte Statistiken", AutoSize = true };
            panel.Controls.Add(showDetailedStatsCheckBox, 1, 0);

            enableGraphsCheckBox = new CheckBox { Text = "Diagramme aktivieren", AutoSize = true };
            panel.Controls.Add(enableGraphsCheckBox, 0, 1);

            // Update Interval
            panel.Controls.Add(new Label { Text = "Update-Intervall (ms):", AutoSize = true }, 0, 2);
            updateIntervalNumeric = new NumericUpDown
            {
                Minimum = 500,
                Maximum = 5000,
                Increment = 250,
                Dock = DockStyle.Fill
            };
            panel.Controls.Add(updateIntervalNumeric, 1, 2);

            performanceTab.Controls.Add(panel);
            tabControl.TabPages.Add(performanceTab);
        }

        private void CreateDialogButtons()
        {
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                FlowDirection = FlowDirection.RightToLeft,
                Height = 40,
                Padding = new Padding(10, 5, 10, 5)
            };

            resetButton = new Button
            {
                Text = "🔄 Zurücksetzen",
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            };
            resetButton.Click += ResetButton_Click;

            cancelButton = new Button
            {
                Text = "❌ Abbrechen",
                Size = new Size(80, 30),
                DialogResult = DialogResult.Cancel,
                UseVisualStyleBackColor = true
            };

            applyButton = new Button
            {
                Text = "✅ Anwenden",
                Size = new Size(80, 30),
                UseVisualStyleBackColor = true
            };
            applyButton.Click += ApplyButton_Click;

            okButton = new Button
            {
                Text = "✅ OK",
                Size = new Size(60, 30),
                DialogResult = DialogResult.OK,
                UseVisualStyleBackColor = true
            };
            okButton.Click += OkButton_Click;

            buttonPanel.Controls.AddRange(new Control[] { resetButton, cancelButton, applyButton, okButton });
            this.Controls.Add(buttonPanel);
        }

        #endregion

        #region Helper Methods

        private Panel CreateColorPanel(string tooltip, EventHandler clickHandler)
        {
            var panel = new Panel
            {
                Size = new Size(50, 25),
                BorderStyle = BorderStyle.FixedSingle,
                Cursor = Cursors.Hand
            };
            
            panel.Click += clickHandler;
            
            if (UIManager.Config.ShowTooltips)
            {
                var toolTip = new ToolTip();
                toolTip.SetToolTip(panel, tooltip);
            }
            
            return panel;
        }

        #endregion

        #region Event Handlers

        private void ThemeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (suppressEvents) return;

            var selectedTheme = (UITheme)themeComboBox.SelectedIndex;
            UIManager.ChangeTheme(selectedTheme);
            UpdateColorPanels();
        }

        private void PrimaryColorPanel_Click(object sender, EventArgs e)
        {
            ShowColorDialog(primaryColorPanel, "Primärfarbe wählen", 
                color => config.PrimaryColor = color);
        }

        private void AccentColorPanel_Click(object sender, EventArgs e)
        {
            ShowColorDialog(accentColorPanel, "Akzentfarbe wählen", 
                color => config.AccentColor = color);
        }

        private void BackgroundColorPanel_Click(object sender, EventArgs e)
        {
            ShowColorDialog(backgroundColorPanel, "Hintergrundfarbe wählen", 
                color => config.BackgroundColor = color);
        }

        private void TextColorPanel_Click(object sender, EventArgs e)
        {
            ShowColorDialog(textColorPanel, "Textfarbe wählen", 
                color => config.TextColor = color);
        }

        private void FontScaleTrackBar_ValueChanged(object sender, EventArgs e)
        {
            // Font-Skalierung in Echtzeit anzeigen
            var scale = fontScaleTrackBar.Value / 100f;
            // Tooltip mit aktuellem Wert anzeigen
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
            UIManager.ApplyTheme(this.Owner);
        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Alle Einstellungen auf Standardwerte zurücksetzen?", 
                               "Bestätigung", 
                               MessageBoxButtons.YesNo, 
                               MessageBoxIcon.Question) == DialogResult.Yes)
            {
                config.ResetToDefaults();
                LoadSettings();
                UIManager.ApplyTheme(this);
            }
        }

        #endregion

        #region Settings Management

        private void LoadSettings()
        {
            suppressEvents = true;

            // Theme Tab
            themeComboBox.SelectedIndex = (int)config.Theme;
            animationsCheckBox.Checked = config.EnableAnimations;
            hoverEffectsCheckBox.Checked = config.EnableHoverEffects;
            animationSpeedTrackBar.Value = Math.Max(100, Math.Min(1000, config.AnimationSpeed));

            // Window Tab
            rememberPositionCheckBox.Checked = config.RememberWindowPosition;
            startMaximizedCheckBox.Checked = config.StartMaximized;
            minimizeToTrayCheckBox.Checked = config.EnableMinimizeToTray;
            widthNumeric.Value = config.WindowSize.Width;
            heightNumeric.Value = config.WindowSize.Height;

            // Accessibility Tab
            fontScaleTrackBar.Value = (int)(config.FontScale * 100);
            highContrastCheckBox.Checked = config.HighContrast;
            showTooltipsCheckBox.Checked = config.ShowTooltips;
            enableSoundsCheckBox.Checked = config.EnableSounds;
            languageComboBox.SelectedIndex = config.Language switch
            {
                "de-DE" => 0,
                "en-US" => 1,
                "fr-FR" => 2,
                "es-ES" => 3,
                _ => 0
            };

            // Performance Tab
            showMetricsCheckBox.Checked = config.ShowPerformanceMetrics;
            showDetailedStatsCheckBox.Checked = config.ShowDetailedStats;
            updateIntervalNumeric.Value = config.UpdateInterval;
            enableGraphsCheckBox.Checked = config.EnableGraphs;

            UpdateColorPanels();
            suppressEvents = false;
        }

        private void SaveSettings()
        {
            // Theme Tab
            config.Theme = (UITheme)themeComboBox.SelectedIndex;
            config.EnableAnimations = animationsCheckBox.Checked;
            config.EnableHoverEffects = hoverEffectsCheckBox.Checked;
            config.AnimationSpeed = animationSpeedTrackBar.Value;

            // Window Tab
            config.RememberWindowPosition = rememberPositionCheckBox.Checked;
            config.StartMaximized = startMaximizedCheckBox.Checked;
            config.EnableMinimizeToTray = minimizeToTrayCheckBox.Checked;
            config.WindowSize = new Size((int)widthNumeric.Value, (int)heightNumeric.Value);

            // Accessibility Tab
            config.FontScale = fontScaleTrackBar.Value / 100f;
            config.HighContrast = highContrastCheckBox.Checked;
            config.ShowTooltips = showTooltipsCheckBox.Checked;
            config.EnableSounds = enableSoundsCheckBox.Checked;
            config.Language = languageComboBox.SelectedIndex switch
            {
                0 => "de-DE",
                1 => "en-US",
                2 => "fr-FR",
                3 => "es-ES",
                _ => "de-DE"
            };

            // Performance Tab
            config.ShowPerformanceMetrics = showMetricsCheckBox.Checked;
            config.ShowDetailedStats = showDetailedStatsCheckBox.Checked;
            config.UpdateInterval = (int)updateIntervalNumeric.Value;
            config.EnableGraphs = enableGraphsCheckBox.Checked;

            config.Save();
        }

        private void UpdateColorPanels()
        {
            primaryColorPanel.BackColor = config.PrimaryColor;
            accentColorPanel.BackColor = config.AccentColor;
            backgroundColorPanel.BackColor = config.BackgroundColor;
            textColorPanel.BackColor = config.TextColor;
        }

        private void ShowColorDialog(Panel colorPanel, string title, Action<Color> onColorSelected)
        {
            using (var colorDialog = new ColorDialog())
            {
                colorDialog.Color = colorPanel.BackColor;
                colorDialog.FullOpen = true;
                
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    colorPanel.BackColor = colorDialog.Color;
                    onColorSelected(colorDialog.Color);
                    config.Theme = UITheme.Custom; // Wechsel zu benutzerdefiniert
                    themeComboBox.SelectedIndex = (int)UITheme.Custom;
                }
            }
        }

        #endregion
    }
}
