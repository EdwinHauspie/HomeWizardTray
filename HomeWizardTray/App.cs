using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using HomeWizardTray.Providers;
using Timer = System.Timers.Timer;

namespace HomeWizardTray
{
    internal class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly DataProvider _dataProvider;
        private NotifyIcon _trayIcon;
        private Timer _timer;

        public App(AppSettings appSettings, DataProvider dataProvider)
        {
            _appSettings = appSettings;
            _dataProvider = dataProvider;

            if (ValidateAppSettings())
            {
                SetupIcon();
                UpdateIcon();
            }
            else Exit(-1);
        }


        private bool ValidateAppSettings()
        {
            var valid = _appSettings.Validate(out string err);

            if (!valid)
            {
                var msg = $"{err}\r\nCheck appSettings.json";
                MessageBox.Show(msg, "Invalid settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return valid;
        }

        private void SetupIcon()
        {
            _trayIcon = new NotifyIcon
            {
                Icon = Icon.FromHandle(Resources.Sun.GetHicon()),
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip() { ShowImageMargin = false }

            };
            _trayIcon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("Quit", null, (s, e) => Exit()));
            _trayIcon.Click += (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) Exit(); };

            _timer = new Timer(_appSettings.UpdateInterval);
            _timer.Elapsed += (sender, args) => UpdateIcon();
            _timer.Start();
        }

        private async void UpdateIcon()
        {
            var data = await _dataProvider.GetData();
            _trayIcon.Text = string.Format(_appSettings.Format, data.ActivePower);
        }

        public void Exit(int exitCode = 0)
        {
            if (_trayIcon != null) _trayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}