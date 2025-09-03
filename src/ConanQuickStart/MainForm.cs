using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ConanQuickStart
{
    public class MainForm : Form
    {
        private TextBox txtServer;
        private Button btnApply;
        private Button btnStart;
        private Button btnStartAndConnect;
        private TextBox log;

        public MainForm()
        {
            Text = "Conan QuickStart";
            Width = 560; Height = 360; StartPosition = FormStartPosition.CenterScreen;

            txtServer = new TextBox { Left = 12, Top = 12, Width = 350, PlaceholderText = "IP:Port oder steam://connect/…" };
            btnApply = new Button { Left = 12, Top = 44, Width = 150, Text = "Tweaks anwenden" };
            btnStart = new Button { Left = 172, Top = 44, Width = 150, Text = "Conan starten" };
            btnStartAndConnect = new Button { Left = 332, Top = 44, Width = 200, Text = "Starten + Verbinden" };
            log = new TextBox { Left = 12, Top = 84, Width = 520, Height = 220, Multiline = true, ScrollBars = ScrollBars.Vertical, ReadOnly = true };

            Controls.AddRange(new Control[] { txtServer, btnApply, btnStart, btnStartAndConnect, log });

            btnApply.Click += (_, __) =>
            {
                try { ApplyClientTweaks(); Append("Tweaks angewendet."); }
                catch (Exception ex) { Append("Fehler: " + ex.Message); }
            };

            btnStart.Click += (_, __) =>
            {
                try { LaunchConan(); }
                catch (Exception ex) { Append("Fehler: " + ex.Message); }
            };

            btnStartAndConnect.Click += (_, __) =>
            {
                try
                {
                    ApplyClientTweaks();
                    var target = txtServer.Text.Trim();
                    if (string.IsNullOrWhiteSpace(target)) { LaunchConan(); return; }
                    LaunchConanAndConnect(target);
                }
                catch (Exception ex) { Append("Fehler: " + ex.Message); }
            };
        }

        private void Append(string line)
        {
            log.AppendText(line + Environment.NewLine);
        }

        private static string GetSavedCfg()
        {
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(local, "ConanSandbox", "Saved", "Config", "WindowsNoEditor");
            Directory.CreateDirectory(path);
            return path;
        }

        private void ApplyClientTweaks()
        {
            var cfg = GetSavedCfg();
            var engine = Path.Combine(cfg, "Engine.ini");
            var game = Path.Combine(cfg, "Game.ini");

            // backups
            Backup(engine, ".backup-qstart-" + Timestamp());
            Backup(game, ".backup-qstart-" + Timestamp());

            // Engine.ini tweaks
            SetIniValues(engine, "[/script/engine.physicssettings]", ("bEnableAsyncScene", "True"));
            SetIniValues(engine, "[/script/engine.renderersettings]", ("r.GraphicsAdapter", "-1"));

            // Game.ini tweaks
            SetIniValues(game, "[/script/engine.gamenetworkmanager]", ("MaxMoveDeltaTime", "0.033f"));
            SetIniValues(game, "[/script/engine.player]", ("ConfiguredInternetSpeed", "100000"));
            SetIniValues(game, "[/Script/MoviePlayer.MoviePlayerSettings]",
                ("bWaitForMoviesToComplete", "False"),
                ("bMoviesAreSkippable", "True"),
                ("StartupMovies", ""));
        }

        private static string Timestamp() => DateTime.Now.ToString("yyyyMMddHHmmss");

        private static void Backup(string path, string suffix)
        {
            if (File.Exists(path))
            {
                File.Copy(path, path + suffix, true);
            }
        }

        private static void SetIniValues(string path, string section, params (string Key, string Value)[] kvs)
        {
            if (!File.Exists(path)) File.WriteAllText(path, "", new UTF8Encoding(false));
            var lines = File.ReadAllText(path, Encoding.UTF8);
            var arr = lines.Length == 0 ? new string[0] : Regex.Split(lines, "\r?\n");

            int start = -1, end = arr.Length;
            for (int i = 0; i < arr.Length; i++) if (arr[i].Trim().Equals(section, StringComparison.OrdinalIgnoreCase)) { start = i; break; }
            if (start >= 0)
            {
                for (int j = start + 1; j < arr.Length; j++) { var t = arr[j].Trim(); if (t.StartsWith("[")) { end = j; break; } }
                // replace
                var set = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
                for (int k = start + 1; k < end; k++)
                {
                    // treat lines starting with ';' (optionally after whitespace) as comments
                    var line = arr[k]; if (Regex.IsMatch(line, @"^\s*;")) continue;
                    var idx = line.IndexOf('='); if (idx <= 0) continue;
                    var key = line.Substring(0, idx).Trim();
                    for (int x = 0; x < kvs.Length; x++)
                    {
                        if (string.Equals(key, kvs[x].Key, StringComparison.OrdinalIgnoreCase))
                        { arr[k] = kvs[x].Key + "=" + kvs[x].Value; set.Add(kvs[x].Key); break; }
                    }
                }
                // append missing
                var list = new System.Collections.Generic.List<string>();
                if (start >= 0) for (int i = 0; i <= start; i++) list.Add(arr[i]);
                for (int i = start + 1; i < end; i++) list.Add(arr[i]);
                for (int x = 0; x < kvs.Length; x++) if (!set.Contains(kvs[x].Key)) list.Add(kvs[x].Key + "=" + kvs[x].Value);
                for (int i = end; i < arr.Length; i++) list.Add(arr[i]);
                arr = list.ToArray();
            }
            else
            {
                var list = new System.Collections.Generic.List<string>(arr);
                if (list.Count > 0 && list[list.Count - 1].Trim().Length > 0) list.Add("");
                list.Add(section);
                for (int x = 0; x < kvs.Length; x++) list.Add(kvs[x].Key + "=" + kvs[x].Value);
                arr = list.ToArray();
            }

            var content = string.Join("\r\n", arr) + "\r\n";
            File.WriteAllText(path, content, new UTF8Encoding(false));
        }

        private void LaunchConan()
        {
            // Prefer Steam app launch
            Process.Start(new ProcessStartInfo
            {
                FileName = "steam://rungameid/440900",
                UseShellExecute = true
            });
            Append("Conan über Steam gestartet.");
        }

        private void LaunchConanAndConnect(string target)
        {
            // Accept "IP:Port" or full steam://connect/…
            string url = target.StartsWith("steam://", StringComparison.OrdinalIgnoreCase)
                ? target
                : ($"steam://connect/{target}");
            Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            Append("Verbinde zu " + url);
        }
    }
}
