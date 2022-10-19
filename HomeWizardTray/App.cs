using System;
using System.Linq;
using System.Windows.Forms;
using HomeWizardTray.Providers;
using Timer = System.Timers.Timer;

namespace HomeWizardTray
{
    internal class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly DataProvider _dataProvider;
        private readonly BarIcons _barIcons;
        private NotifyIcon _trayIcon;
        private Timer _timer;

        public App(AppSettings appSettings, DataProvider dataProvider, BarIcons barIcons)
        {
            _appSettings = appSettings;
            _dataProvider = dataProvider;
            _barIcons = barIcons;

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
                var msg = $"{err}\r\nCheck the settings file.";
                MessageBox.Show(msg, "Invalid settings", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return valid;
        }

        private void SetupIcon()
        {
            _trayIcon = new NotifyIcon { Icon = _barIcons.NoBars, Visible = true };
            _trayIcon.Click += (object sender, EventArgs e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) Exit(); };

            _timer = new Timer(2000);
            _timer.Elapsed += (sender, args) => UpdateIcon();
            _timer.Start();
        }

        private async void UpdateIcon()
        {
            var data = await _dataProvider.GetData();

            var iconIndex = 0;
            _appSettings.Thresholds.ToList().ForEach(threshold => iconIndex += data.ActivePower > threshold ? 1 : 0);

            _trayIcon.Text = string.Format(_appSettings.Format, data.ActivePower);
            _trayIcon.Icon = _barIcons.Icons[iconIndex];
        }

        public void Exit(int exitCode = 0)
        {
            if (_trayIcon != null) _trayIcon.Visible = false;
            //Application.Exit();
            Environment.Exit(exitCode);
        }
    }
}