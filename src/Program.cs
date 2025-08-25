using System;
using System.Windows.Forms;

namespace ConanExilesOptimizer
{
    internal static class Program
    {
        /// <summary>
        ///  Der Einstiegspunkt der Anwendung.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // High-DPI, Default Font etc.
            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}