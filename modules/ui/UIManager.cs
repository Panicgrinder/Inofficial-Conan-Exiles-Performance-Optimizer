using System;
using System.Drawing;
using System.Windows.Forms;

namespace ConanExilesOptimizer.UI
{
    /// <summary>
    /// Manager für UI-Themes und Styling
    /// </summary>
    public static class UIManager
    {
        private static UIConfig _config;
        
        /// <summary>
        /// Aktuelle UI-Konfiguration
        /// </summary>
        public static UIConfig Config
        {
            get => _config ??= UIConfig.Load();
            set => _config = value;
        }

        /// <summary>
        /// Event wird ausgelöst wenn das Theme geändert wird
        /// </summary>
        public static event EventHandler ThemeChanged;

        /// <summary>
        /// Initialisiert den UI-Manager
        /// </summary>
        public static void Initialize()
        {
            Config = UIConfig.Load();
        }

        /// <summary>
        /// Wendet das Theme auf ein Form an
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            if (form == null) return;

            form.BackColor = Config.BackgroundColor;
            form.ForeColor = Config.TextColor;
            
            // Rekursiv auf alle Controls anwenden
            ApplyThemeToControls(form.Controls);
        }

        /// <summary>
        /// Wendet das Theme auf eine Control-Collection an
        /// </summary>
        private static void ApplyThemeToControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                ApplyThemeToControl(control);
                
                // Rekursiv für verschachtelte Controls
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control.Controls);
                }
            }
        }

        /// <summary>
        /// Wendet das Theme auf ein einzelnes Control an
        /// </summary>
        public static void ApplyThemeToControl(Control control)
        {
            if (control == null) return;

            switch (control)
            {
                case Button button:
                    StyleButton(button);
                    break;
                    
                case Label label:
                    StyleLabel(label);
                    break;
                    
                case Panel panel:
                    StylePanel(panel);
                    break;
                    
                case GroupBox groupBox:
                    StyleGroupBox(groupBox);
                    break;
                    
                case TextBox textBox:
                    StyleTextBox(textBox);
                    break;
                    
                case ComboBox comboBox:
                    StyleComboBox(comboBox);
                    break;
                    
                case CheckBox checkBox:
                    StyleCheckBox(checkBox);
                    break;
                    
                case RadioButton radioButton:
                    StyleRadioButton(radioButton);
                    break;
                    
                case ProgressBar progressBar:
                    StyleProgressBar(progressBar);
                    break;
                    
                default:
                    // Standard-Styling für unbekannte Controls
                    control.BackColor = Config.BackgroundColor;
                    control.ForeColor = Config.TextColor;
                    break;
            }

            // Font-Skalierung anwenden
            if (Config.FontScale != 1.0f && control.Font != null)
            {
                float newSize = control.Font.Size * Config.FontScale;
                control.Font = new Font(control.Font.FontFamily, newSize, control.Font.Style);
            }
        }

        #region Control Styling Methods

        private static void StyleButton(Button button)
        {
            button.BackColor = Config.ButtonColor;
            button.ForeColor = Config.TextColor;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderColor = Config.PrimaryColor;
            button.FlatAppearance.BorderSize = 1;
            
            if (Config.EnableHoverEffects)
            {
                button.MouseEnter += (s, e) => button.BackColor = Config.ButtonHoverColor;
                button.MouseLeave += (s, e) => button.BackColor = Config.ButtonColor;
            }
        }

        private static void StyleLabel(Label label)
        {
            label.BackColor = Color.Transparent;
            label.ForeColor = Config.TextColor;
        }

        private static void StylePanel(Panel panel)
        {
            panel.BackColor = Config.BackgroundColor;
            panel.ForeColor = Config.TextColor;
        }

        private static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.BackColor = Config.BackgroundColor;
            groupBox.ForeColor = Config.AccentColor;
        }

        private static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Color.FromArgb(50, 50, 50);
            textBox.ForeColor = Config.TextColor;
            textBox.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.BackColor = Color.FromArgb(50, 50, 50);
            comboBox.ForeColor = Config.TextColor;
            comboBox.FlatStyle = FlatStyle.Flat;
        }

        private static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.BackColor = Color.Transparent;
            checkBox.ForeColor = Config.TextColor;
        }

        private static void StyleRadioButton(RadioButton radioButton)
        {
            radioButton.BackColor = Color.Transparent;
            radioButton.ForeColor = Config.TextColor;
        }

        private static void StyleProgressBar(ProgressBar progressBar)
        {
            progressBar.BackColor = Color.FromArgb(50, 50, 50);
            progressBar.ForeColor = Config.PrimaryColor;
        }

        #endregion

        /// <summary>
        /// Ändert das Theme und benachrichtigt alle Listener
        /// </summary>
        public static void ChangeTheme(UITheme theme)
        {
            Config.Theme = theme;
            ApplyThemeColors(theme);
            Config.Save();
            ThemeChanged?.Invoke(null, EventArgs.Empty);
        }

        /// <summary>
        /// Wendet vordefinierte Farbschemata basierend auf dem Theme an
        /// </summary>
        private static void ApplyThemeColors(UITheme theme)
        {
            switch (theme)
            {
                case UITheme.Light:
                    Config.BackgroundColor = Color.White;
                    Config.TextColor = Color.Black;
                    Config.ButtonColor = Color.FromArgb(240, 240, 240);
                    Config.ButtonHoverColor = Color.FromArgb(220, 220, 220);
                    Config.PrimaryColor = Color.FromArgb(0, 122, 204);
                    Config.AccentColor = Color.FromArgb(255, 140, 0);
                    break;
                    
                case UITheme.Dark:
                    Config.BackgroundColor = Color.FromArgb(30, 30, 30);
                    Config.TextColor = Color.White;
                    Config.ButtonColor = Color.FromArgb(60, 60, 60);
                    Config.ButtonHoverColor = Color.FromArgb(80, 80, 80);
                    Config.PrimaryColor = Color.FromArgb(0, 122, 204);
                    Config.AccentColor = Color.FromArgb(255, 140, 0);
                    break;
                    
                case UITheme.HighContrast:
                    Config.BackgroundColor = Color.Black;
                    Config.TextColor = Color.White;
                    Config.ButtonColor = Color.FromArgb(40, 40, 40);
                    Config.ButtonHoverColor = Color.FromArgb(80, 80, 80);
                    Config.PrimaryColor = Color.Yellow;
                    Config.AccentColor = Color.Cyan;
                    Config.HighContrast = true;
                    break;
            }
        }

        /// <summary>
        /// Zeigt das UI-Einstellungen-Menü an
        /// </summary>
        public static void ShowSettingsDialog(Form parent)
        {
            using (var settingsForm = new UISettingsForm())
            {
                settingsForm.ShowDialog(parent);
            }
        }

        /// <summary>
        /// Erstellt einen animierten Übergang für Theme-Änderungen
        /// </summary>
        public static void AnimateThemeChange(Form form)
        {
            if (!Config.EnableAnimations || !Config.EnableFadeEffects)
            {
                ApplyTheme(form);
                return;
            }

            // Einfache Fade-Animation
            var timer = new System.Windows.Forms.Timer();
            timer.Interval = 50;
            double opacity = form.Opacity;
            
            timer.Tick += (s, e) =>
            {
                opacity -= 0.1;
                if (opacity <= 0)
                {
                    form.Opacity = 0;
                    ApplyTheme(form);
                    opacity = 0;
                    
                    // Fade-In
                    var fadeInTimer = new System.Windows.Forms.Timer();
                    fadeInTimer.Interval = 50;
                    fadeInTimer.Tick += (s2, e2) =>
                    {
                        opacity += 0.1;
                        form.Opacity = opacity;
                        if (opacity >= 1)
                        {
                            form.Opacity = 1;
                            fadeInTimer.Stop();
                            fadeInTimer.Dispose();
                        }
                    };
                    fadeInTimer.Start();
                    
                    timer.Stop();
                    timer.Dispose();
                }
                else
                {
                    form.Opacity = opacity;
                }
            };
            
            timer.Start();
        }
    }
}
