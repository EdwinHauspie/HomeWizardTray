using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.Assets;
using HomeWizardTray.DataProviders;
using Timer = System.Timers.Timer;

namespace HomeWizardTray
{
    internal class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly HomeWizardDataProvider _homeWizardDataProvider;
        private readonly NotifyIcon _trayIcon;
        private readonly Timer _timer;

        public App(AppSettings appSettings, HomeWizardDataProvider homeWizardDataProvider)
        {
            _appSettings = appSettings;
            _homeWizardDataProvider = homeWizardDataProvider;
            _trayIcon = CreateNotifyIcon();
            _timer = CreateTimer();
            UpdateIconText();
        }

        private NotifyIcon CreateNotifyIcon()
        {
            var icon = Resources.SunIcon;
            var menu = new ContextMenuStrip() { ShowImageMargin = false };
            menu.Items.Add(new ToolStripMenuItem("Quit", null, (s, e) => Exit()));
            var trayIcon = new NotifyIcon { Text = "SunnyTray", Icon = icon, Visible = true, ContextMenuStrip = menu };
            trayIcon.Click += (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) Exit(); };
            return trayIcon;
        }

        private Timer CreateTimer()
        {
            var timer = new Timer(TimeSpan.FromSeconds(_appSettings.UpdateIntervalSeconds).TotalMilliseconds);
            timer.Elapsed += (sender, args) => UpdateIconText();
            timer.Start();
            return timer;
        }

        private async void UpdateIconText()
        {
            try
            {
                _timer.Stop();
                var activePower = await _homeWizardDataProvider.GetActivePower();
                _trayIcon.Text = string.Format(_appSettings.Format, activePower);
                _timer.Start();
            }
            catch (Exception ex)
            {
                _trayIcon.Text = "An error has occured. See log file.";
                Log.Error(ex, ex.Message);
            }
        }

        public void Exit(int exitCode = 0)
        {
            if (_trayIcon != null) _trayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}