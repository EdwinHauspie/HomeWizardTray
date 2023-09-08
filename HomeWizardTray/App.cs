using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.Assets;
using HomeWizardTray.DataProviders.HomeWizard;
using HomeWizardTray.DataProviders.SunnyBoy;
using Timer = System.Timers.Timer;

namespace HomeWizardTray
{
    internal sealed class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly HomeWizardDataProvider _homeWizardDataProvider;
        private readonly SunnyBoyDataProvider _sunnyBoyDataProvider;

        private ContextMenuStrip Menu { get; set; }
        private NotifyIcon TrayIcon { get; set; }
        private Timer UpdateTimer { get; set; }

        public App(AppSettings appSettings, HomeWizardDataProvider homeWizardDataProvider, SunnyBoyDataProvider sunnyBoyDataProvider)
        {
            _appSettings = appSettings;
            _homeWizardDataProvider = homeWizardDataProvider;
            _sunnyBoyDataProvider = sunnyBoyDataProvider;

            Menu = new ContextMenuStrip { ShowImageMargin = false };
            Menu.Items.Add(new ToolStripMenuItem("Quit", null, (s, e) => Exit()));

            TrayIcon = new NotifyIcon { Text = "SunnyTray", Icon = Resources.SunIcon, Visible = true, ContextMenuStrip = Menu };
            TrayIcon.Click += (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) Exit(); };

            UpdateTimer = new Timer();
            UpdateTimer.Elapsed += (sender, args) => UpdateIconText();
            UpdateTimer.Start();
        }

        private async void UpdateIconText()
        {
            try
            {
                UpdateTimer.Stop();

                var sunnyPower = await _sunnyBoyDataProvider.GetActivePower();
                var homeWizardPower = await _homeWizardDataProvider.GetActivePower();
                TrayIcon.Text = $"🔆   {sunnyPower} W\r\n💡 {(homeWizardPower < 0 ? "" : "  ")}{homeWizardPower.ToString().Replace("-", "–")} W";

                UpdateTimer.Interval = TimeSpan.FromSeconds(_appSettings.UpdateIntervalSeconds).TotalMilliseconds;
                UpdateTimer.Start();
            }
            catch (Exception ex)
            {
                TrayIcon.Text = "An error has occured. Please see the log file.";
                Log.Error(ex, ex.Message);
            }
        }

        public void Exit(int exitCode = 0)
        {
            if (TrayIcon != null) TrayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}