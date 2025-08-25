using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace ConanOptimizer.Safe
{
    public partial class SafeOptimizerForm : Form
    {
        public SafeOptimizerForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Conan Exiles Optimizer - Safe Version v3.1.0";
            this.Size = new Size(600, 400);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Safe Version UI - Nur sichere Optimierungen
            var titleLabel = new Label
            {
                Text = "🛡️ 100% BAN-SICHERE OPTIMIERUNGEN",
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(500, 30),
                ForeColor = Color.DarkGreen
            };

            var descLabel = new Label
            {
                Text = "Diese Version verwendet nur Windows-Optimierungen und verändert keine Spieledateien.\nPerfekt für Official Server und Anfänger.",
                Location = new Point(20, 60),
                Size = new Size(500, 40)
            };

            var optimizeButton = new Button
            {
                Text = "✅ Sichere Optimierungen anwenden",
                Size = new Size(250, 50),
                Location = new Point(175, 150),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            optimizeButton.Click += OptimizeButton_Click;

            var infoLabel = new Label
            {
                Text = "Angewendete Optimierungen:\n• Windows Gaming Mode\n• GPU Hardware Scheduling\n• Power Management\n• Memory Management",
                Location = new Point(20, 220),
                Size = new Size(500, 100),
                ForeColor = Color.DarkBlue
            };

            this.Controls.AddRange(new Control[] { titleLabel, descLabel, optimizeButton, infoLabel });
        }

        private void OptimizeButton_Click(object sender, EventArgs e)
        {
            var settings = OptimizationSettings.GetOfficialServerSafe();
            var results = OptimizationApplier.ApplySelectedOptimizations(settings);
            
            var successCount = results.FindAll(r => r.StartsWith("✅")).Count;
            var failCount = results.FindAll(r => r.StartsWith("❌")).Count;

            MessageBox.Show(
                $"Sichere Optimierung abgeschlossen!\n\n" +
                $"✅ Erfolgreich: {successCount}\n" +
                $"❌ Fehlgeschlagen: {failCount}\n\n" +
                $"Details:\n{string.Join("\n", results)}\n\n" +
                $"Starte Conan Exiles neu, um die Änderungen zu übernehmen.",
                "Safe Optimierung Abgeschlossen",
                MessageBoxButtons.OK,
                failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information
            );
        }
    }

    public static class SafeProgram
    {
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SafeOptimizerForm());
        }
    }
}
