using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ConanOptimizer.Original
{
    public partial class OriginalOptimizerForm : Form
    {
        public OriginalOptimizerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Conan Exiles Optimizer - Original Version v3.0.0";
            this.Size = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Original Version UI - Bewährte Community-Optimierungen
            var titleLabel = new Label
            {
                Text = "🔧 BEWÄHRTE COMMUNITY-OPTIMIERUNGEN",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30),
                ForeColor = Color.DarkBlue
            };

            var descLabel = new Label
            {
                Text = "Diese Version basiert auf kเt's bewährtem Steam Guide.\nMittleres Risiko, bewährte Performance-Steigerung.",
                Location = new Point(20, 60),
                Size = new Size(500, 40)
            };

            var optimizeButton = new Button
            {
                Text = "⚡ Community-Optimierungen anwenden",
                Size = new Size(250, 50),
                Location = new Point(175, 150),
                BackColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            optimizeButton.Click += OptimizeButton_Click;

            var infoLabel = new Label
            {
                Text = "Angewendete Optimierungen:\n• Alle sicheren Windows-Optimierungen\n• Engine.ini Rendering-Verbesserungen\n• Game.ini Netzwerk-Optimierungen\n• Bewährte Community-Einstellungen",
                Location = new Point(20, 220),
                Size = new Size(500, 100),
                ForeColor = Color.DarkBlue
            };

            this.Controls.AddRange(new Control[] { titleLabel, descLabel, optimizeButton, infoLabel });
        }

        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            var settings = OptimizationSettings.GetPrivateServerOptimized();
            // Füge Engine.ini und Game.ini Optimierungen hinzu
            settings.EngineIniTweaks = true;
            settings.GameIniNetwork = true;
            
            var results = OptimizationApplier.ApplySelectedOptimizations(settings);
            
            var successCount = results.FindAll(r => r.StartsWith("✅")).Count;
            var failCount = results.FindAll(r => r.StartsWith("❌")).Count;

            MessageBox.Show(
                $"Community-Optimierung abgeschlossen!\n\n" +
                $"✅ Erfolgreich: {successCount}\n" +
                $"❌ Fehlgeschlagen: {failCount}\n\n" +
                $"Details:\n{string.Join("\n", results)}\n\n" +
                $"⚠️ Warnung: Diese Optimierungen können auf Official Servern zu einem Ban führen.\n" +
                $"Starte Conan Exiles neu, um die Änderungen zu übernehmen.",
                "Community-Optimierung Abgeschlossen",
                MessageBoxButtons.OK,
                failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
            );
        }
    }

    public static class OriginalProgram
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new OriginalOptimizerForm());
        }
    }
}
