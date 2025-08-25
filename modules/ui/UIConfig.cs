using System;
using System.Drawing;
using System.IO;
using System.Text.Json;

namespace ConanExilesOptimizer.UI
{
    /// <summary>
    /// Konfigurationsklasse für UI-Einstellungen
    /// </summary>
    public class UIConfig
    {
        #region Theme Properties
        public UITheme Theme { get; set; } = UITheme.Dark;
        public Color PrimaryColor { get; set; } = Color.FromArgb(0, 122, 204);
        public Color AccentColor { get; set; } = Color.FromArgb(255, 140, 0);
        public Color BackgroundColor { get; set; } = Color.FromArgb(30, 30, 30);
        public Color TextColor { get; set; } = Color.White;
        public Color ButtonColor { get; set; } = Color.FromArgb(60, 60, 60);
        public Color ButtonHoverColor { get; set; } = Color.FromArgb(80, 80, 80);
        #endregion

        #region Animation Settings
        public bool EnableAnimations { get; set; } = true;
        public int AnimationSpeed { get; set; } = 300; // ms
        public bool EnableFadeEffects { get; set; } = true;
        public bool EnableHoverEffects { get; set; } = true;
        #endregion

        #region Window Settings
        public Size WindowSize { get; set; } = new Size(900, 700);
        public Point WindowPosition { get; set; } = new Point(-1, -1); // -1 = Centered
        public bool RememberWindowPosition { get; set; } = true;
        public bool StartMaximized { get; set; } = false;
        public bool EnableMinimizeToTray { get; set; } = true;
        #endregion

        #region Accessibility
        public float FontScale { get; set; } = 1.0f;
        public bool HighContrast { get; set; } = false;
        public bool ShowTooltips { get; set; } = true;
        public bool EnableSounds { get; set; } = true;
        #endregion

        #region Language & Localization
        public string Language { get; set; } = "de-DE";
        public string DateFormat { get; set; } = "dd.MM.yyyy";
        public string TimeFormat { get; set; } = "HH:mm:ss";
        #endregion

        #region Performance Display
        public bool ShowPerformanceMetrics { get; set; } = true;
        public bool ShowDetailedStats { get; set; } = false;
        public int UpdateInterval { get; set; } = 1000; // ms
        public bool EnableGraphs { get; set; } = true;
        #endregion

        #region Config File Methods
        private static readonly string ConfigPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "ConanExilesOptimizer",
            "ui-config.json"
        );

        /// <summary>
        /// Lädt die UI-Konfiguration aus der Datei
        /// </summary>
        public static UIConfig Load()
        {
            try
            {
                if (File.Exists(ConfigPath))
                {
                    string json = File.ReadAllText(ConfigPath);
                    var config = JsonSerializer.Deserialize<UIConfig>(json);
                    return config ?? new UIConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der UI-Konfiguration: {ex.Message}");
            }
            
            return new UIConfig();
        }

        /// <summary>
        /// Speichert die UI-Konfiguration in eine Datei
        /// </summary>
        public void Save()
        {
            try
            {
                string directory = Path.GetDirectoryName(ConfigPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                string json = JsonSerializer.Serialize(this, options);
                File.WriteAllText(ConfigPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern der UI-Konfiguration: {ex.Message}");
            }
        }

        /// <summary>
        /// Setzt alle Einstellungen auf Standardwerte zurück
        /// </summary>
        public void ResetToDefaults()
        {
            Theme = UITheme.Dark;
            PrimaryColor = Color.FromArgb(0, 122, 204);
            AccentColor = Color.FromArgb(255, 140, 0);
            BackgroundColor = Color.FromArgb(30, 30, 30);
            TextColor = Color.White;
            ButtonColor = Color.FromArgb(60, 60, 60);
            ButtonHoverColor = Color.FromArgb(80, 80, 80);
            
            EnableAnimations = true;
            AnimationSpeed = 300;
            EnableFadeEffects = true;
            EnableHoverEffects = true;
            
            WindowSize = new Size(900, 700);
            WindowPosition = new Point(-1, -1);
            RememberWindowPosition = true;
            StartMaximized = false;
            EnableMinimizeToTray = true;
            
            FontScale = 1.0f;
            HighContrast = false;
            ShowTooltips = true;
            EnableSounds = true;
            
            Language = "de-DE";
            DateFormat = "dd.MM.yyyy";
            TimeFormat = "HH:mm:ss";
            
            ShowPerformanceMetrics = true;
            ShowDetailedStats = false;
            UpdateInterval = 1000;
            EnableGraphs = true;
        }

        #endregion
    }

    /// <summary>
    /// Verfügbare UI-Themes
    /// </summary>
    public enum UITheme
    {
        Light,
        Dark,
        HighContrast,
        Custom
    }
}
