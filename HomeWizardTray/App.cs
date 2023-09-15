using System;
using Serilog;
using System.Windows.Forms;
using HomeWizardTray.Assets;
using HomeWizardTray.DataProviders.HomeWizard;
using HomeWizardTray.DataProviders.Sma;
using Timer = System.Timers.Timer;
using System.Drawing;
using System.Threading.Tasks;
using Newtonsoft.Json;
using HomeWizardTray.DataProviders.Daikin;
using HomeWizardTray.Menu;

namespace HomeWizardTray
{
    internal sealed class App : ApplicationContext
    {
        private readonly AppSettings _appSettings;
        private readonly P1MeterDataProvider _homeWizardDataProvider;
        private readonly SunnyBoyDataProvider _sunnyBoyDataProvider;
        private readonly Ftxm25DataProvider _ftxm25DataProvider;

        private NotifyIcon TrayIcon { get; set; }
        private Timer UpdateTimer { get; set; }

        public App(AppSettings appSettings, P1MeterDataProvider homeWizardDataProvider, SunnyBoyDataProvider sunnyBoyDataProvider, Ftxm25DataProvider ftxm25DataProvider)
        {
            _appSettings = appSettings;
            _homeWizardDataProvider = homeWizardDataProvider;
            _sunnyBoyDataProvider = sunnyBoyDataProvider;
            _ftxm25DataProvider = ftxm25DataProvider;

            async void onQuit(object s, EventArgs e) => await Exit();
            async void onAircoStatus(object s, EventArgs e) { MessageBox.Show(await _ftxm25DataProvider.GetStatus(), "SunnyTray"); }

            var menu = new[]
            {
                new MenuItem("Airco", new []
                {
                    new MenuItem("Presets", new []
                    {
                        new MenuItem("Max", async (s, e) => { await _ftxm25DataProvider.SetMax(); }),
                        new MenuItem("Normal", async (s, e) => { await _ftxm25DataProvider.SetLevel2(); }),
                        new MenuItem("Eco", async (s, e) => { await _ftxm25DataProvider.SetEco(); }),
                        new MenuItem("Dehumidify", async (s, e) => { await _ftxm25DataProvider.SetDehumidify(); }),
                    }),
                    new MenuItem("Power", new []
                    {
                        new MenuItem("On", async (s, e) => { await _ftxm25DataProvider.SetLevel2(); }),
                        new MenuItem("Off", async (s, e) => { await _ftxm25DataProvider.SetOff(); })
                    }),
                    new MenuItem("Status", onAircoStatus)
                }),
                new MenuItem("Quit", onQuit)
            };

            TrayIcon = new NotifyIcon { Text = "SunnyTray", Icon = Resources.SunIcon, Visible = true, ContextMenuStrip = MenuBuilder.Build(menu) };
            TrayIcon.Click += async (s, e) => { if (((MouseEventArgs)e).Button == MouseButtons.Middle) await Exit(); };

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

                TrayIcon.Text = $"""
                🔆 {FormatPower(sunnyPower)}
                🔌 {FormatPower(homeWizardPower)}
                💡 {FormatPower(sunnyPower + homeWizardPower)}
                """;

                UpdateTimer.Interval = TimeSpan.FromSeconds(_appSettings.UpdateIntervalSeconds).TotalMilliseconds;
                UpdateTimer.Start();
            }
            catch (Exception ex)
            {
                TrayIcon.Text = "An error has occured. Please see the log file.";
                Log.Error(ex, ex.Message);
            }
        }

        private static string FormatPower(int power)
        {
            // Justify text with repeated "thick space" character
            // https://unicode-explorer.com/c/2004
            var fillout = 6 - power.ToString().Length;
            return $"{new string(' ', fillout)} {power} W".Replace("-", "–");
        }

        public async Task Exit(int exitCode = 0)
        {
            UpdateTimer.Stop();
            await _sunnyBoyDataProvider.Logout(); // TODO make disposable instead?
            if (TrayIcon != null) TrayIcon.Visible = false;
            Environment.Exit(exitCode);
        }
    }
}